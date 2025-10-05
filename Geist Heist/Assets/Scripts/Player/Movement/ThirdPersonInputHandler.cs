/*
 * Contributors: Toby, Jacob, Brooke, Sky, Josh
 * Creation Date: 9/16/25
 * Last Modified: 10/1/2025
 * Last Modified: 10/1/25
 * 
 * Brief Description: Handles third person movement and interaction
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using GuardUtilities;
//using UnityEditor.UIElements; had to comment this out as they were causing build errors, UIElements does not exist in namespace UnityEditor

public class ThirdPersonInputHandler : IInputHandler
{
    private Rigidbody rigidbody;

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

    [Header("Scene Transition Variables")]
    // Scene transition specific variables
    [Tooltip("Higher number: longer interactable distance from object")]
    [SerializeField] private float interactableRayLength = 10;
    [SerializeField, Required] private GameObject interactableCanvas;

    public static Action<GuardStates> OnPossessObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        layerToInclude = LayerMask.GetMask("Interactable");

        if(interactableCanvas == null)
        {
            Debug.LogError("No interactable canvas has been set");
        }
        GameManager.Instance.LoadCurrentLevel();
    }

    // Update is called once per frame
    void Update()
    {
        TurnOnInteractableCanvas();
    }

    // for the player / ghost: this means ENTERING ghost mode
    public override void OnPossessionStarted()
    {
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
        throw new NotImplementedException();
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
                interactable.Interact(/*result.transform.GetComponent<PossessableObject>()*/);

                OnPossessObject?.Invoke(0);
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

    #region  SceneTransition
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

        if (Physics.Raycast(interactableOrigin, interactableDirection, out hit, interactableRayLength, layerToInclude))
        {
            interactableCanvas.SetActive(true);
        }
        else
        {
            interactableCanvas.SetActive(false);
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
