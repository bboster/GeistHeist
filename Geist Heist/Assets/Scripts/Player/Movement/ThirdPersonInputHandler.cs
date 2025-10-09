/*
 * Contributors: Toby, Jacob, Brooke, Sky, Josh, Skylar
 * Creation Date: 9/16/25
 * Last Modified: 10/7/25
 * 
 * Brief Description: Handles third person movement and interaction. 
 * This script should only be used for the ghost
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using GuardUtilities;
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

    [Header("Sphere Cast Variables")]
    [SerializeField] private GameObject thirdPersonCinemachineCamera;
    [SerializeField] private float sphereCastRadius = 10;
    [SerializeField] private float sphereCastDistance = 1000;
    private LayerMask layerToInclude;

    [Header("Interaction")]
    // Scene transition specific variables
    [Tooltip("Higher number: longer interactable distance from object")]
    [SerializeField] private float interactableRayLength = 10;
    private GameObject interactableCanvas => GameManager.Instance.InteractionCanvas;
    private GameObject lastObjectLookedAt;

    [Header("Between Possession Cooldown Variables")]
    [SerializeField] private Canvas cooldownCanvas => CooldownManager.Instance?.CooldownCanvas.GetComponent<Canvas>();

    private Rigidbody rigidbody;

    public static Action<GuardStates> OnPossessObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        layerToInclude = LayerMask.GetMask("Interactable");
        CooldownManager.Instance.OnCooldownFinished += OnCooldownFinished;
    }

    // Update is called once per frame
    void Update()
    {
        TurnOnInteractableCanvas();
    }

    // for the player / ghost: this means ENTERING ghost mode
    public override void OnPossessionStarted()
    {
        CooldownManager.Instance.StartCooldown();
        TurnOnCooldownCanvas();
    }

    // for the player / ghost: this means EXITING ghost mode
    public override void OnPossessionEnded()
    {
    }

    #region Action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld()
    {
    }

    public override void WhileActionNotHeld()
    {
        //throw new NotImplementedException();
    }

    public override void OnActionCanceled()
    {
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
        
        var sphereCastResults = Physics.SphereCastAll(gameObject.transform.position, sphereCastRadius, thirdPersonCinemachineCamera.transform.forward, sphereCastDistance, layerToInclude);

        foreach (var result in sphereCastResults)
        {
            if (result.transform.TryGetComponent(out IInteractable interactable) && result.transform != this.transform)
            {
                if (result.transform.TryGetComponent(out PossessableObject possessableObject) && CooldownManager.Instance.IsCooldownActive)
                {
                    return;
                }

                interactable.Interact(/*result.transform.GetComponent<PossessableObject>()*/);
                OnPossessObject?.Invoke(GuardStates.returnToPath);
                break;
            }
        }
    }

    public override void WhileInteractHeld()
    {
    }

    public override void OnInteractCanceled()
    {
    }

    private void OnCooldownFinished()
    {
        if(cooldownCanvas != null)
        {
            cooldownCanvas.gameObject.SetActive(false);
        }
    }

    private void TurnOnCooldownCanvas()
    {
        if (CooldownManager.Instance.IsCooldownActive && cooldownCanvas != null)
        {
            cooldownCanvas.gameObject.SetActive(true);
        }
    }
    #endregion

    #region Move
    public override void OnMoveStarted()
    {
        
    }
    public override void WhileMoveHeld()
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


    public override void OnMoveCanceled(){}
    #endregion

    #region  Interaction
    public void TurnOnInteractableCanvas()
    {
        if (interactableCanvas == null)
        {
            return;
        }

        RaycastHit hit;
        Vector3 interactableOrigin = transform.position;
        Vector3 interactableDirection = Camera.main.transform.forward; // moves the raycast with the camera, since the player remains still

        Debug.DrawRay(interactableOrigin, interactableDirection * interactableRayLength, Color.red);

        // TODO: make it a spherecast here.
        // If you could find a way to generalize this spherecast to be the same as the spherecast in the OnInteractStarted started function that would be huge!

        if (Physics.Raycast(interactableOrigin, interactableDirection, out hit, interactableRayLength, layerToInclude))
        {
            interactableCanvas.SetActive(true);

            // if switching what youre looking at
            if(hit.transform.gameObject != lastObjectLookedAt && lastObjectLookedAt != null)
            {
                if(lastObjectLookedAt.TryGetComponent<Outline>(out Outline outline)){
                    outline.enabled = false;
                }
            }

            if (hit.transform.TryGetComponent<Outline>(out Outline outline2))
            {
                outline2.enabled = true;
            }
            lastObjectLookedAt = hit.transform.gameObject;
        }
        else
        {
            interactableCanvas.SetActive(false);

            if (lastObjectLookedAt != null && lastObjectLookedAt.TryGetComponent<Outline>(out Outline outline))
            {
                outline.enabled = false;
            }
            lastObjectLookedAt = null;
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        //var sphereCastResults = Physics.SphereCastAll(thirdPersonCinemachineCamera.transform.position, sphereCastRadius, thirdPersonCinemachineCamera.transform.forward, sphereCastDistance, layerToInclude);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, sphereCastRadius);
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + (thirdPersonCinemachineCamera.transform.forward * sphereCastDistance));
        Gizmos.DrawWireSphere(gameObject.transform.position + (thirdPersonCinemachineCamera.transform.forward * sphereCastDistance), sphereCastRadius);
  
    }

   
}
