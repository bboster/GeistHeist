/*
 * Contributors: Toby, Jacob, Brooke
 * Creation Date: 9/16/25
 * Last Modified: 9/29/25
 * 
 * Brief Description: For prototyping first person gameplay, for now
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEditor.UIElements;

public class ThirdPersonInputHandler : IInputHandler
{
    private Rigidbody rigidbody;

    [SerializeField] private float speed = 3;
    [SerializeField] private float maxVelocity = 10;
    [Tooltip("Higher number: reaches desired speed faster")]
    [SerializeField] private float speedPickup = 3;
    [Tooltip("Multiply speed by this number when player is not holding any move keys")]
    [SerializeField] private float slowDownFactor = 0.1f;
    [SerializeField] private GameObject thirdPersonCinemachineCamera;
    [SerializeField] private float sphereCastRadius = 10;
    [SerializeField] private float sphereCastDistance = 1000;
    private LayerMask layerToInclude;

    [Header("Scene Transition Variables")]
    // Scene transition specific variables
    [Tooltip("Higher number: longer interactable distance from object")]
    [SerializeField] private float interactableRayLength = 10;
    [SerializeField] private LayerMask raycastLayer;
    [SerializeField] private GameObject interactableCanvas;

    //?
    public static Action<int> OnPossessObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        layerToInclude = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        TurnOnInteractableCanvas();
    }

    #region action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld()
    {
        throw new System.NotImplementedException();
    }

    public override void OnActionCanceled()
    {
        throw new System.NotImplementedException();
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
                interactable.Interact(result.transform.GetComponent<PossessableObject>());

                //OnPossessObject?.Invoke(0);
                break;
            }
        }
    }



    public override void WhileInteractHeld()
    { }

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
        RaycastHit hit;
        Vector3 interactableOrigin = transform.position;
        Vector3 interactableDirection = Camera.main.transform.forward; // moves the raycast with the camera, since the player remains still

        Debug.DrawRay(interactableOrigin, interactableDirection * interactableRayLength, Color.red);

        if (Physics.Raycast(interactableOrigin, interactableDirection, out hit, interactableRayLength, raycastLayer))
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
