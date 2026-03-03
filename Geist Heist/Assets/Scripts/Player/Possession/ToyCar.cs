using NaughtyAttributes;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
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
    [Tooltip("How much moving rotates by per second.")]
    [SerializeField] private float rotationRate = 30;
    [BoxGroup("Gamepad Tuning"), Tooltip("Modifies the gamepad's sensitivity while rotating the toy car")]
    [SerializeField] private float rotationSensitivityMod = 0.01f;

    [Header("Speedometer seconds")]
    [SerializeField] private float delayToUpdateChargeMeter = 0.25f;

    [Header("VFX")]
    [SerializeField] private string OnomatopoeiaText = "Bonk!";
    [SerializeField] private float OnomatopoeiaScale = 1;
    [SerializeField] private float onomatopoeiaLifetime = 1;
    [SerializeField] private ParticleSystem possessableParticle;

    //realtime hold strength
    private float currentStrength;

    private Rigidbody rb;
    private bool physicsEnabled = false;
    private PossessableObject possessableObject;
    private SuddenVelocityChangeDetector velocityChangeDetector; 

    private Coroutine freezeCoroutine;
    //activates when ghost is leaving an object
    private bool IsLeaving = false;
    private bool hasLaunchedThisPossession = false;
    private float lastCrashOnomatopoeiaTimeStamp;


    private EventInstance carMoveSFX;
    private EventInstance carWindSFX;

    private void Start()
    {
        //Same note on sound as in PossessableObject.cs
        carMoveSFX = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.CarGo);
        carWindSFX = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.CarWind);

        rb = gameObject.GetComponent<Rigidbody>();
        possessableObject = GetComponent<PossessableObject>();
        velocityChangeDetector = GetComponent<SuddenVelocityChangeDetector>();

        velocityChangeDetector.OnBounceDetected.AddListener(OnCrashOrBounceDetected);
        velocityChangeDetector.OnStopDetected.AddListener(OnCrashOrBounceDetected);
    }

    public override void OnPossessionStart()
    {
        hasLaunchedThisPossession = false;

        possessableParticle.Play();
        velocityChangeDetector.StartRecordingVelocity();

        if (possessableObject.PossessedMaterial != null)
        {
            possessableObject.meshRenderer.material = possessableObject.PossessedMaterial;
        }
    }

    public override void OnPossessionEnded()
    {
        currentStrength = minStrength;
        possessableParticle.Stop();
        velocityChangeDetector.StopRecordingVelocity();

        if (possessableObject.UnpossessedMaterial != null)
        {
            possessableObject.meshRenderer.material = possessableObject.UnpossessedMaterial;
        }
    }

    // Called every frame while player is possessing.
    public override void WhilePossessingUpdate()
    {
        //Note for sound: Cases like this with repeating code should probably call another function that does the repeated bit and takes non-repeat info as parameters
        carMoveSFX.set3DAttributes(RuntimeUtils.To3DAttributes(transform, GetComponent<Rigidbody>()));
        carWindSFX.set3DAttributes(RuntimeUtils.To3DAttributes(transform, GetComponent<Rigidbody>()));

        //chargeMeter.UpdateCharge(currentStrength, maxStrength);
        PossessableToolbar.Instance.SetChargeBarValue(currentStrength / maxStrength);


        //pause timer if car is moving
        if (rb.linearVelocity == Vector3.zero)
        {
            carMoveSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            possessableObject.PauseDischargeTimer = false;

            if (possessableObject.PossessedMaterial != null)
            {
                possessableObject.meshRenderer.material = possessableObject.PossessedMaterial;
            }
        }
        else
        {
            //Car movement sound logic
            PLAYBACK_STATE playbackState;
            carMoveSFX.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                carMoveSFX.start();
            }

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
        carWindSFX.start();
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

        carWindSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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

            if (possessableObject.UnpossessedMaterial != null)
            {
                possessableObject.meshRenderer.material = possessableObject.UnpossessedMaterial;
            }

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
            if (InputEvents.Instance.IsMoveInputFromGamepad())
                rotation *= rotationSensitivityMod;
            transform.Rotate(new Vector3(rotation, 0, 0) * Time.deltaTime);
        }
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled(float secondsHeld) { }
    #endregion


    #region Onomatopoeias

    void OnCrashOrBounceDetected(Vector3 impactPoint)
    {
        if (Time.time - lastCrashOnomatopoeiaTimeStamp < 0.1f)
            return;

        Vector3 spawnPoint = impactPoint + (Vector3.up * 2);
        BillboardUIManager.Instance.SpawnOnomatopoeia(OnomatopoeiaText, spawnPoint, lifetime: onomatopoeiaLifetime,
            bold:true, fontScale:OnomatopoeiaScale, 
            animateRotationOverTime:true, randomRotationRange:15, 
            animateScaleOverTime:true);

        lastCrashOnomatopoeiaTimeStamp = Time.time;

        //TODO: add Bonk sound

        // TODO: add particle
    }

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

