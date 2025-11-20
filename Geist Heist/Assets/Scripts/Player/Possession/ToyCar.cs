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
    [SerializeField] private float chargeLossRate;
    [Tooltip("Force there to be time between zooms")]
    [SerializeField] private float delayBetweenZooms = 1;

    [Header("Speedometer seconds")]
    [SerializeField] private float delayToUpdateChargeMeter = 0.25f;

    [Tooltip("How much moving rotates by per second.")]
    [SerializeField] private float rotationRate = 30;
    //realtime hold strength
    private float currentStrength;

    private Rigidbody rb;
    private bool physicsEnabled = false;
    private PossessableObject possessableObject;

    private Coroutine freezeCoroutine;
    //activates when ghost is leaving an object
    private bool IsLeaving = false;
    private bool hasLaunchedThisPossession = false;

    [SerializeField] private PossessableChargeMeterUI chargeMeter;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        possessableObject = GetComponent<PossessableObject>();

        if (chargeMeter == null)
            chargeMeter = GetComponentInChildren<ToyCarSpeedometerUI>();
    }

    public override void OnPossessionStart()
    {
        hasLaunchedThisPossession = false;
        chargeMeter.OnPossessionStarted();
    }

    public override void OnPossessionEnded()
    {
        currentStrength = minStrength;
    }

    // Called every frame while player is possessing.
    public override void WhilePossessingUpdate()
    {
        chargeMeter.UpdateCharge(currentStrength, maxStrength);

        //pause timer if car is moving
        if (rb.linearVelocity == Vector3.zero)
        {
            possessableObject.PauseDischargeTimer = false;

            if (possessableObject.UnpossessedMaterial != null)
            {
                possessableObject.meshRenderer.material = possessableObject.UnpossessedMaterial;
            }
        }
        else
        {
            possessableObject.PauseDischargeTimer = true;
        }
    }
    
    private void FixedUpdate()
    {
        //consistent speed for car
        if (physicsEnabled)
        {
            Debug.Log("clamping strength");
            currentStrength = Mathf.Clamp(currentStrength, minStrength, maxStrength);
            rb.AddForce(gameObject.transform.forward * currentStrength, ForceMode.Impulse);
            
            if (possessableObject.VisiblePossessionMaterial != null)
            {
                possessableObject.meshRenderer.material = possessableObject.VisiblePossessionMaterial;
            }

            possessableObject.PauseDischargeTimer = true;
            physicsEnabled = false;
            hasLaunchedThisPossession = true;
        }
    }

    #region action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld(float secondsHeld)
    {
        if (secondsHeld < delayBetweenZooms && hasLaunchedThisPossession)
        {
            currentStrength = Mathf.Max(
                currentStrength - (Time.deltaTime * chargeLossRate),
                minStrength);
            return;
        }

        if (rb.linearVelocity.magnitude <= 0.5f)
        {
            // Will be clamped later
            currentStrength += chargeRate * Time.deltaTime;
        }
    }

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
        if (rb.linearVelocity.magnitude <= maxSpeedToZoom)
        {
            if (freezeCoroutine == null)
            {
                freezeCoroutine = StartCoroutine(ReFreezeConstraints());
            }
        }

        // dont update the speedometer for a sec..
        if (secondsNotHeld < delayToUpdateChargeMeter && hasLaunchedThisPossession)
            return;

        currentStrength = Mathf.Max(
            currentStrength - (Time.deltaTime * chargeLossRate),
            minStrength);
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
        if (possessableObject.CanUnPossess && rb.linearVelocity == Vector3.zero) 
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
        var direction = InputEvents.Instance.InputDirection2D.x;
        var rotation = rotationRate * direction;

        if (rb.linearVelocity == Vector3.zero)
        {
            transform.Rotate(new Vector3(rotation, 0, 0) * Time.deltaTime);
        }
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

