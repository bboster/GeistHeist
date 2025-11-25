/*
 * Contributors: Toby, Jacob, Brooke, Sky, Josh, Skylar
 * Creation Date: 9/16/25
 * Last Modified: 11/18/25
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
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEditor.UIElements; had to comment this out as they were causing build errors, UIElements does not exist in namespace UnityEditor

public class ThirdPersonInputHandler : IInputHandler
{
    [Header("Design Variables")]
    [SerializeField] public float speed = 3;
    [SerializeField] private float maxVelocity = 10;
    [Tooltip("Higher number: reaches desired speed faster")]
    [SerializeField] private float speedPickup = 3;
    [Tooltip("Multiply speed by this number when player is not holding any move keys")]
    [SerializeField] private float slowDownFactor = 0.1f;

    [Tooltip("Approximate degrees per second")]
    [Foldout ("Animation Settings"), SerializeField] private float rotationSpeed = 60f;
    [Tooltip("How much up/down player goes. value of 0.1 will go -0.1 to +0.1. total height of 0.2")]
    [Foldout ("Animation Settings"), SerializeField] private float hoverHeight = 0.2f;
    [Foldout ("Animation Settings"), SerializeField] private float hoverSpeed = 0.75f;

    [Header("Interaction")]
    // Scene transition specific variables
    [SerializeField, Foldout("Interaction")] private GameObject thirdPersonCinemachineCamera;
    [SerializeField, Foldout("Interaction")] private float interactSphereCastRadius = 3;
    [Tooltip("Higher number: longer interactable distance from object")]
    [SerializeField, Foldout("Interaction")] private float interactRayLength = 5;
    [SerializeField, Foldout("Interaction")] LayerMask layerToInclude;

    [Header("Components")]
    [SerializeField, Required] private MeshRenderer playerModel;
    [SerializeField] private ParticleSystem OllieParticles;

    [Foldout("Debug"), SerializeField] private bool drawInteractRay=true;

    private Rigidbody rigidbody;

    public static Action<GuardStates> OnPossessObject;

    private GameObject lastObjectLookedAt;
    private Vector3 sphereCastDirection => thirdPersonCinemachineCamera.transform.forward;
    private float frameCountSinceLastInteraction;
    private Vector3 positionLastFrame;
    private float modelStartYPosition;
    private Quaternion targetRotation;

    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    void Start()
    {
        targetRotation = transform.rotation;
        positionLastFrame = transform.position;
        rigidbody = GetComponent<Rigidbody>();
        modelStartYPosition = playerModel.transform.position.y;

        //layerToInclude = LayerMask.GetMask("Interactable");
        //CooldownManager.Instance.OnCooldownFinished += OnCooldownFinished;
    }

    // WhilePossessingUpdate is called once per frame
    public override void WhilePossessingUpdate()
    {
        TryTurnOnInteractablePrompt();

        RotatePlayer();
        HoverBob();
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
            if (result.transform.TryGetComponent(out interactable) == false &&
                result.transform.TryGetComponent(out possessableObject) == false
                )
            {
                continue;
            }

            if (interactable.IsInteractable() == false)
                continue;

            if (result.transform.gameObject == this.gameObject)
                continue;

            // Test if there is a wall between player and the object
            Vector3 playerPos = gameObject.transform.position;
            Vector3 interactPos = result.transform.position;
            Vector3 direction = (interactPos - playerPos).normalized;
            float distance = Vector3.Distance(playerPos, interactPos);

            //raycast is sent from the player 
            bool ray = Physics.Raycast(playerPos, direction, out RaycastHit hit, distance, layerToInclude);
            if (drawInteractRay) Debug.DrawLine(playerPos, interactPos,
                                ray && hit.transform.gameObject != result.transform.gameObject ? Color.red : Color.green);
            if (ray && hit.transform.gameObject != result.transform.gameObject)
            {
                Debug.Log("Raycast hit a wall");
                continue;
            }

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
    /// Opens button prompts if possible (through Hide/OnPlayerLookStart functions on IInteractable)
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
            // Display Interact UI, most of the time
            interactable.OnPlayerLookStart();
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
            // Hide interact UI, most of the time
            interactable.OnPlayerLookStop();
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
        OllieParticles.Play();
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


    public override void OnMoveCanceled(float secondsHeld) 
    {
        OllieParticles.Stop();
    }
    #endregion

    #region Other

    private void RotatePlayer()
    {
        if(transform.rotation != targetRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        Vector3 diff = (transform.position - positionLastFrame).WithY(0);

        // if not moved significantly enough. Intentionally don't record position last frame
        //if (Mathf.Approximately(diff.magnitude, 0) || transform.position == positionLastFrame)
        if (diff.magnitude < 0.1 || transform.position == positionLastFrame)
            return;

        targetRotation = Quaternion.LookRotation(diff);

        positionLastFrame = transform.position;
    }

    private void HoverBob() // squarepants
    {
        float height = modelStartYPosition + StaticUtilities.SinRange(Time.time * hoverSpeed / MathF.PI, -hoverHeight, hoverHeight);

        playerModel.transform.position = playerModel.transform.position.WithY(height);
    }

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
