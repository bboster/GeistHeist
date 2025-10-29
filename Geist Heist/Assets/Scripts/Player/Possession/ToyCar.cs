using NaughtyAttributes;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
/*
 * Contributors: Sky, Toby
 * Creation Date: 10/2/25
 * Last Modified: 10/27/25
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
    [Tooltip("Max speed for car to be already be going able to go.")]
    [SerializeField] private float maxSpeedToZoom = 1;
    [Tooltip("How much hold charges up by per second.")]
    [SerializeField] private float chargeRate;
    [Tooltip("When not held, how much hold charges down by per second.")]
    [SerializeField] private float chargeLossRates;
    [SerializeField] private float delayToUpdateSpeedometer = 0.5f;

    private float secondsToCharge; // calculated in start => (maxStrength-minStrength) / chargeRate
    //realtime hold strength
    private float currentStrength;

    private Rigidbody rb;
    private bool physicsEnabled = false;
    private PossessableObject possessableObject;

    private Coroutine freezeCoroutine;
    //activates when ghost is leaving an object
    private bool IsLeaving = false;


    [SerializeField] private PossessableChargeMeterUI chargeMeter;

    private void Start()
    {
        secondsToCharge = (maxStrength- minStrength) / chargeRate;
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
        if (InputEvents.ActionPressed)
        {
            chargeMeter.UpdateCharge(InputEvents.ActionHeldTime, secondsToCharge);
        }
        else
        {
            if (InputEvents.ActionReleasedTime < delayToUpdateSpeedometer)
                return;

            float t = (currentStrength - minStrength) / (maxStrength - minStrength);
            chargeMeter.UpdateCharge(t*secondsToCharge, secondsToCharge);
        }
    }
    
    private void FixedUpdate()
    {
        //consistent speed for car
        if (physicsEnabled)
        {
            rb.AddForce(gameObject.transform.forward * currentStrength, ForceMode.Impulse);
            physicsEnabled = false;
        }
    }

    #region action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld(float secondsHeld)
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

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
        if (rb.linearVelocity.magnitude <= 0)
        {
            if (freezeCoroutine == null)
            {
                freezeCoroutine = StartCoroutine(ReFreezeConstraints());
            }
        }

        // dont update the speedometer for a sec..
        if (secondsNotHeld < delayToUpdateSpeedometer)
            return;

        currentStrength -= Time.deltaTime * chargeLossRates;
    }

    public override void OnActionCanceled(float secondsHeld)
    {
        // Fake charge amount calculation (this is a failsafe, sanity thing)
        //currentStrength = Mathf.Min((secondsHeld * chargeRate) + minStrength, maxStrength);

        if (rb.linearVelocity.magnitude <= maxSpeedToZoom)
        {
            UnFreezePosition();
            //for fixed update to handle physics better
            physicsEnabled = true;
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

