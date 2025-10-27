using NaughtyAttributes;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
/*
 * Contributors: Sky
 * Creation Date: 10/2/25
 * Last Modified: 10/7/25
 * 
 * Brief Description: Input Handler for the Toy Car, handles movement and actions for the Toy Car
 */
[RequireComponent(typeof(PossessableObject))]
public class ToyCar : IInputHandler
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;

    [Header("Design Variables")]
    [Tooltip("Strength that a tap would do- the LEAST the car can move forward when interacting.")]
    [SerializeField] private float minStrength;
    [Tooltip("Strength that a full hold would do- the MOST the car can move forward when interacting.")]
    [SerializeField] private float maxStrength;
    [Tooltip("How much hold charges up by per second.")]
    [SerializeField] private float chargeRate;
    //realtime hold strength
    private float chargeAmount;

    private Rigidbody rb;
    private PossessableObject possessableObject;

    private Coroutine freezeCoroutine;
    //activates when ghost is leaving an object
    private bool IsLeaving = false;


    [SerializeField] private PossessableChargeMeterUI chargeMeter;

    private void Start()
    {
        thirdPersoncinemachineCamera.SetActive(false);
        rb = gameObject.GetComponent<Rigidbody>();
        possessableObject = GetComponent<PossessableObject>();  
    }

    public override void OnPossessionStart()
    {
        chargeMeter.OnPossessionStarted();
    }

    public override void OnPossessionEnded()
    {
    }

    // Called every frame while player is possessing.
    public override void WhilePossessingUpdate()
    {
        //chargeMeter.UpdateCharge
    }

    #region action
    public override void OnActionStarted()
    {
        if (rb.linearVelocity == Vector3.zero)
        {
            chargeAmount = minStrength;
            //ChargeUI.fillAmount = (chargeAmount - minStrength) / (maxStrength - minStrength);
            //Images.SetActive(true);
        }
    }

    public override void WhileActionHeld(float secondsHeld)
    {
        if (rb.linearVelocity == Vector3.zero)
        {
            chargeAmount += chargeRate * Time.deltaTime;

            if (chargeAmount > maxStrength)
            {
                chargeAmount = maxStrength;
            }

            //ChargeUI.fillAmount = (chargeAmount - minStrength) / (maxStrength - minStrength);
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
        }
    }

    public override void OnActionCanceled(float secondsHeld)
    {
        if (rb.linearVelocity == Vector3.zero)
        {
            UnFreezePosition();
            rb.AddForce(gameObject.transform.forward * chargeAmount, ForceMode.Impulse);
            //Images.SetActive(false);
        }
    }

    public IEnumerator ReFreezeConstraints()
    {
        //either stopped or exiting object
        while (rb.linearVelocity == Vector3.zero || IsLeaving)
        {
            //wait to see if still not moving
            yield return new WaitForSeconds(.2f);

            //if still moving, don't do anything YET
            if (rb.linearVelocity != Vector3.zero)
            {
                yield return new WaitForSeconds(0.1f);
            }

            //if stopped
            if (rb.linearVelocity == Vector3.zero)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                freezeCoroutine = null;
                yield break;
            }
        }
        freezeCoroutine = null;
    }

    #endregion

    #region Possess

    /// <summary>
    /// When player leaves toy car
    /// </summary>
    public override void OnInteractStarted()
    {
        if (thirdPersoncinemachineCamera.activeSelf && possessableObject.CanUnPossess) 
        {
            PlayerManager.Instance.PossessGhost(GetComponent<PossessableObject>());
            IsLeaving = true;
            if (freezeCoroutine == null)
            { 
                freezeCoroutine = StartCoroutine(ReFreezeConstraints());
            }
        }
    }

    public override void WhileInteractHeld(float secondsHeld)
    { }

    public override void OnInteractCanceled(float secondsHeld)
    {
    }

    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld(float secondsHeld)
    {
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled(float secondsHeld) { }
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

