/*
 * Contributors: Toby
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * Brief Description: For prototyping first person gameplay, for now
 */

using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region action
    public override void OnActionStarted()
    {
        throw new System.NotImplementedException();
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
    public override void OnPossessStarted()
    {
        LayerMask lm = LayerMask.GetMask("PossessableObject");
        var sphereCastResult = Physics.SphereCastAll(firstPersoncinemachineCamera.transform.position, sphereCastRadius, firstPersoncinemachineCamera.transform.forward, sphereCastDistance, lm);

        foreach (var results in sphereCastResult)
        {
            if (results.transform.GetComponent<PossessableObject>() && results.transform != this.transform)
            {
                PlayerManager.Instance.PossessObject(results.transform.GetComponent<PossessableObject>());
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
}
