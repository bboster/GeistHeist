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

public class FirstPersonInputHandler : IInputHandler
{
    private Rigidbody rigidbody;

    [SerializeField] private float speed = 3;
    [SerializeField] private float maxVelocity = 10;
    [Tooltip("Higher number: reaches desired speed faster")]
    [SerializeField] private float speedPickup = 3;
    [Tooltip("Multiply speed by this number when player is not holding any move keys")]
    [SerializeField] private float slowDownFactor = 0.1f;
    [SerializeField] private GameObject firstPersoncinemachineCamera;
    [SerializeField] private float sphereCastRadius = 10;
    [SerializeField] private float sphereCastDistance = 100000000;

    // Scene transition specific variables
    [SerializeField] private string sceneName;
    [Tooltip("Higher number: longer interactable distance from object")]
    [SerializeField] private float rayLength = 10;
    [SerializeField] private LayerMask raycastLayer;
    [SerializeField] GameObject canvas;

    public static Action<int> OnPossessObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TransitionScene();
    }

    #region action
    public override void OnActionStarted()
    {
        throw new System.NotImplementedException();
    }

    public override void WhileActionHeld()
    {
        SceneManager.LoadScene(sceneName);
    }

    public override void OnActionCanceled()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Possess
    public override void OnPossessStarted()
    {
        LayerMask lm = LayerMask.GetMask("PossessableObject");
        var sphereCastResult = Physics.SphereCastAll(firstPersoncinemachineCamera.transform.position, sphereCastRadius, firstPersoncinemachineCamera.transform.forward, sphereCastDistance, lm);

        foreach (var results in sphereCastResult)
        {
            if (results.transform.GetComponent<PossessableObject>() && results.transform != this.transform)
            {
                PlayerManager.Instance.PossessObject(results.transform.GetComponent<PossessableObject>());
                OnPossessObject?.Invoke(0);
                break;
            }
        }
    }



    public override void WhilePossessHeld()
    { }

    public override void OnPossessCanceled()
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
    public void TransitionScene()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = Camera.main.transform.forward; // moves the raycast with the camera, since the player remains still

        Debug.DrawRay(origin, direction * rayLength, Color.red);

        if (Physics.Raycast(origin, direction, out hit, rayLength, raycastLayer))
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }
    }

    #endregion
}
