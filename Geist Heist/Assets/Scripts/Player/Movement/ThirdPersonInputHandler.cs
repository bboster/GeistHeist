/*
 * Contributors: Toby, Jacob, Brooke, Sky, Josh, Skylar
 * Creation Date: 9/16/25
 * Last Modified: 10/27/25
 * 
 * Brief Description: Handles third person movement and interaction. 
 * This script should only be used for the ghost
 */

using GuardUtilities;
using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEditor.UIElements; had to comment this out as they were causing build errors, UIElements does not exist in namespace UnityEditor

public class ThirdPersonInputHandler : IInputHandler
{
    [Header("Design Variables")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float maxVelocity = 10;
    [Tooltip("Higher number: reaches desired speed faster")]
    [SerializeField] private float speedPickup = 3;
    [Tooltip("Multiply speed by this number when player is not holding any move keys")]
    [SerializeField] private float slowDownFactor = 0.1f;
    
    [Header("Interaction")]
    // Scene transition specific variables
    [SerializeField, Foldout("Interaction")] private GameObject thirdPersonCinemachineCamera;
    [SerializeField, Foldout("Interaction")] private float interactSphereCastRadius = 3;
    [Tooltip("Higher number: longer interactable distance from object")]
    [SerializeField, Foldout("Interaction")] private float interactRayLength = 5;
    [SerializeField, Foldout("Interaction")] LayerMask layerToInclude;

    [Header("Between Possession Cooldown Variables")]
    [SerializeField] private Canvas cooldownCanvas => CooldownManager.Instance?.CooldownCanvas.GetComponent<Canvas>();

    [Foldout("Debug"), SerializeField] private bool drawInteractRay=true;

    private Rigidbody rigidbody;

    public static Action<GuardStates> OnPossessObject;

    private GameObject lastObjectLookedAt;
    private Vector3 sphereCastDirection => thirdPersonCinemachineCamera.transform.forward;
    private float frameCountSinceLastInteraction;

    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //layerToInclude = LayerMask.GetMask("Interactable");
        //CooldownManager.Instance.OnCooldownFinished += OnCooldownFinished;
    }

    // WhilePossessingUpdate is called once per frame
    public override void WhilePossessingUpdate()
    {
        TryTurnOnInteractablePrompt();
    }

    // for the player / ghost: this means ENTERING ghost mode
    public override void OnPossessionStart()
    {
        //CooldownManager.Instance.StartCooldown();
        //TurnOnCooldownCanvas();
    }

    // for the player / ghost: this means EXITING ghost mode
    public override void OnPossessionEnded()
    {
    }

    #region Action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld(float secondsHeld)
    {
    }

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
    }

    public override void OnActionCanceled(float secondsHeld)
    {
    }

    #endregion

    #region Interact

    private List<RaycastHit> GetAllInteractablesSphereCast()
    {
        var sphereCastResults = Physics.SphereCastAll(gameObject.transform.position, interactSphereCastRadius, sphereCastDirection, interactRayLength, layerToInclude);

        if (sphereCastResults.IsNullOrEmpty())
            return null;

        List<RaycastHit> filteredResults = new();

        // Filter all gameobjects
        foreach (var result in sphereCastResults)
        {
            PossessableObject possessableObject;
            IInteractable interactable;

            // if object can even be interacted with
            if (result.transform.TryGetComponent(out possessableObject) == false
                && result.transform.TryGetComponent(out interactable) == false)
            {
                continue;
            }

            // change this when every interactable has its own cooldown
            //if (CooldownManager.Instance.IsCooldownActive)
            //    continue;

            if (result.transform.gameObject == this.gameObject)
                continue;

            // Test if there is a wall between player and the object
            Vector3 playerPos = gameObject.transform.position;
            Vector3 interactPos = result.transform.position;
            bool ray = Physics.Raycast(playerPos, interactPos - interactPos, out RaycastHit hit, Vector3.Distance(playerPos, interactPos), layerToInclude);
            if (drawInteractRay) Debug.DrawLine(playerPos, interactPos,
                                ray && hit.transform.gameObject != result.transform.gameObject ? Color.red : Color.green);
            if (ray && hit.transform.gameObject != result.transform.gameObject)
                continue;

            filteredResults.Add(result);
        }

        if (filteredResults.IsNullOrEmpty()) return null;

        return filteredResults;
    }

    private GameObject GetBestInteractableSphereCast()
    {
        // Filter interactables in spherecast
        var filteredSphereCastResults = GetAllInteractablesSphereCast();

        if(filteredSphereCastResults.IsNullOrEmpty()) return null;

        // Sort by which one the player is looking at most. 
        return filteredSphereCastResults
            .Where(r => r.transform.gameObject != this.transform.gameObject)
            .OrderBy(r => 
                // Ref: dot product returns value -1 to 1. -1 for completely opposite directions and 1 for perfectly perpendicular.
                Vector3.Dot(
                    thirdPersonCinemachineCamera.transform.forward, 
                    r.transform.position - gameObject.transform.position
                ))
           .Last()
           .transform.gameObject;
    }

    public override void OnInteractStarted()
    {
        if (Time.frameCount - frameCountSinceLastInteraction <= 3)
            return;

        var result = GetBestInteractableSphereCast();
        if (result == null) return;

        frameCountSinceLastInteraction = Time.time;

        var allInteractables = result.GetComponentsInChildren<IInteractable>();

        foreach(var interactable in allInteractables)
        {
            if (interactable == null)
                continue;

            interactable.Interact();
            
            if(interactable is PossessableObject)
                OnPossessObject?.Invoke(GuardStates.returnToPath);
        }
        LookAtInteractableStop(lastObjectLookedAt);
        lastObjectLookedAt = null;
    }

    /// <summary>
    /// performs spherecast looking for interactable. Same spherecast as interact button.
    /// Opens button prompts if possible (through Hide/DisplayInteractUI functions on IInteractable)
    /// </summary>
    private void TryTurnOnInteractablePrompt()
    {
        var result = GetBestInteractableSphereCast();

        // if looking at something different than last frame
        if (lastObjectLookedAt != result)
        {
            if (lastObjectLookedAt != null)
                LookAtInteractableStop(lastObjectLookedAt);

            if (result != null)
                LookAtInteractableStart(result);
        }
        lastObjectLookedAt = result;
    }

    private void LookAtInteractableStart(GameObject obj)
    {
        if(obj == null) return;

        if (obj.TryGetComponent<Outline>(out Outline outline))
            outline.enabled = true;

        var allInteractables = obj.GetComponentsInChildren<IInteractable>();
        foreach (var interactable in allInteractables)
        {
            interactable.DisplayInteractUI();
        }
    }

    // These two could have been 1 function with a boolean parameter but I like the intuitivity with the names.
    // They can be condensed tho :P idc that much
    private void LookAtInteractableStop(GameObject obj)
    {
        if (obj == null) return;

        if (obj.TryGetComponent<Outline>(out Outline outline))
            outline.enabled = false;

        var allInteractables = obj.GetComponentsInChildren<IInteractable>();
        foreach (var interactable in allInteractables)
        {
            interactable.HideInteractUI();
        }
    }

    public override void WhileInteractHeld(float secondsHeld)
    {
    }

    public override void OnInteractCanceled(float secondsHeld)
    {
    }

    #endregion

    #region Move
    public override void OnMoveStarted()
    {
        
    }
    public override void WhileMoveHeld(float secondsHeld)
    {
        var direction = InputEvents.Instance.FirstPersonInputDirection;

        var a = rigidbody.linearVelocity.WithY(0);
        var b = (direction * speed);

        var horizontalVelocity = Vector3.Lerp(rigidbody.linearVelocity.WithY(0), (direction* speed), speedPickup*Time.fixedDeltaTime);
        Vector3.ClampMagnitude(horizontalVelocity, maxVelocity);

        rigidbody.linearVelocity = horizontalVelocity.WithY(rigidbody.linearVelocity.y);
    }

    public override void WhileMoveNotHeld()
    {
        // Maintains y velocity
        rigidbody.linearVelocity = Vector3.MoveTowards(rigidbody.linearVelocity, new Vector3(0, rigidbody.linearVelocity.y, 0), slowDownFactor * Time.fixedDeltaTime);
    }


    public override void OnMoveCanceled(float secondsHeld) {}
    #endregion

    private void OnDrawGizmos()
    {
        //var filteredSphereCastResults = Physics.SphereCastAll(thirdPersonCinemachineCamera.transform.position, interactSphereCastRadius, thirdPersonCinemachineCamera.transform.forward, sphereCastDistance, layerToInclude);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, interactSphereCastRadius);
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + (thirdPersonCinemachineCamera.transform.forward * interactRayLength));
        Gizmos.DrawWireSphere(gameObject.transform.position + (thirdPersonCinemachineCamera.transform.forward * interactRayLength), interactSphereCastRadius);
  
    }

   
}
