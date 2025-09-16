using UnityEngine;

public class FirstPersonInputHandler : IInputHandler
{
    private Rigidbody rigidbody;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float slowDownFactor = 0.1f;

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
        throw new System.NotImplementedException();
    }

    public override void WhilePossessHeld()
    {
        throw new System.NotImplementedException();
    }

    public override void OnPossessCanceled()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region jump
    public override void OnJumpCanceled()
    {
        throw new System.NotImplementedException();
    }

    public override void WhileJumpHeld()
    {
        throw new System.NotImplementedException();
    }

    public override void OnJumpStarted()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Move
    public override void OnMoveStarted()
    {
        
    }
    public override void WhileMoveHeld()
    {
        var direction = InputEvents.Instance.FirstPersonInputDirection;


    }

    public override void WhileMoveNotHeld()
    {
           
    }


    public override void OnMoveCanceled()
    {
        
    }
    #endregion



}
