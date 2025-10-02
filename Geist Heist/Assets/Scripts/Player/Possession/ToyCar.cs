using UnityEngine; 
/*
 * Contributors: Sky
 * Creation Date: 9/17/25
 * Last Modified: 9/30/25
 * 
 * Brief Description: Input Handler for the Vase, handles movement and actions for the vase
 */
public class ToyCar : IInputHandler, IInteractable
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;

    [Header("Design Variables")]
    [SerializeField] private float minStrength;
    [SerializeField] private float currentStrength;
    [SerializeField] private float maxStrength;
    [SerializeField] private float chargeRate;

    private Rigidbody rb;

    private void Start()
    {
        thirdPersoncinemachineCamera.SetActive(false);
        rb = gameObject.GetComponent<Rigidbody>();
    }

    #region action
    public override void OnActionStarted()
    {
        if ()
        {
            rb.AddForce(gameObject.transform.forward * minStrength);
        }
        else
        {
            currentStrength = minStrength;
        }
    }

    public override void WhileActionHeld()
    {
            currentStrength += currentStrength;

            if (currentStrength > maxStrength)
            {
                currentStrength = maxStrength;
            }
    }

    public override void OnActionCanceled()
    {
        if (!Tap)
        {
            rb.AddForce(gameObject.transform.forward * currentStrength);
        }
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
        if (thirdPersoncinemachineCamera.activeSelf)
        {
            PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());
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
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled() { }
    #endregion

    void IInteractable.Interact(PossessableObject possessable)
    {
        PlayerManager.Instance.PossessObject(possessable);
    }
}

