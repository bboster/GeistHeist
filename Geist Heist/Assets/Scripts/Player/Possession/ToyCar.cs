using NaughtyAttributes;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine; 
/*
 * Contributors: Sky
 * Creation Date: 10/2/25
 * Last Modified: 10/6/25
 * 
 * Brief Description: Input Handler for the Toy Car, handles movement and actions for the Toy Car
 */
public class ToyCar : IInputHandler
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;
    [Required][SerializeField] private Transform ghostSpawnPoint;

    [Header("Design Variables")]
    [SerializeField] private float minStrength;
    private float currentStrength;
    [SerializeField] private float maxStrength;
    [Tooltip("How much hold charges up by per second")]
    [SerializeField] private float chargeRate;

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
        }

        freezeCoroutine = null;
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
        if (thirdPersoncinemachineCamera.activeSelf)
        {
            PlayerManager.Instance.PlayerGhostObject.transform.position = ghostSpawnPoint.position;
            PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());
        }
    }

    public override void WhileInteractHeld()
    { }

    public override void OnInteractCanceled()
    {
    }

    public override void OnPossessionStarted()
    {
    }

    public override void OnPossessionEnded()
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


    public void UnFreezePosition()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward);
    }
}

