using System.Collections;
using System.Xml.Serialization;
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
    private float currentStrength;
    [SerializeField] private float maxStrength;
    [SerializeField] private float chargeRate;
    private bool IsMoving = false;

    private Rigidbody rb;
    private Coroutine freezeCoroutine;

    private void Start()
    {
        thirdPersoncinemachineCamera.SetActive(false);
        rb = gameObject.GetComponent<Rigidbody>();
    }

    #region action
    public override void OnActionStarted()
    {
        if (rb.linearVelocity == Vector3.zero)
        {
            currentStrength = minStrength;
            IsMoving = true;
        }
    }

    public override void WhileActionHeld()
    {
        if (rb.linearVelocity == Vector3.zero)
        {
            currentStrength += chargeRate * Time.deltaTime;

            if (currentStrength > maxStrength)
            {
                currentStrength = maxStrength;
            }
        }
    }
    public override void WhileActionNotHeld()
    {
        if (rb.linearVelocity == Vector3.zero)
        {
            if (freezeCoroutine == null)
            {
                freezeCoroutine = StartCoroutine(ReFreezeConstraints());
            }
            else
            {
                StopCoroutine(freezeCoroutine);
            }
        }
    }

    public override void OnActionCanceled()
    {
        if (rb.linearVelocity == Vector3.zero)
        {
            UnFreezePosition();
            rb.AddForce(gameObject.transform.forward * currentStrength, ForceMode.Impulse);
        }
    }

    public IEnumerator ReFreezeConstraints()
    {
        while (rb.linearVelocity == Vector3.zero)
        {
            yield return new WaitForSeconds(1);

            if (rb.linearVelocity != Vector3.zero)
                yield break;

            rb.constraints = RigidbodyConstraints.FreezeAll;
            IsMoving = false;
        }

        freezeCoroutine = null;
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

    public void UnFreezePosition()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        Debug.Log("i'm running");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward);
    }
}

