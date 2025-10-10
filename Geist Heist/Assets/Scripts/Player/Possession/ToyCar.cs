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
public class ToyCar : IInputHandler
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;
    [Tooltip("Location where the ghost spawns after leaving the car.")]
    [Required][SerializeField] private Transform ghostSpawnPoint;

    [Header("Design Variables")]
    [Tooltip("Strength that a tap would do- the LEAST the car can move forward when interacting.")]
    [SerializeField] private float minStrength;
    [Tooltip("Strength that a full hold would do- the MOST the car can move forward when interacting.")]
    [SerializeField] private float maxStrength;
    [Tooltip("How much hold charges up by per second.")]
    [SerializeField] private float chargeRate;
    //realtime hold strength
    private float currentStrength;

    private Rigidbody rb;
    private Coroutine freezeCoroutine;
    //activates when ghost is leaving an object
    private bool IsLeaving = false;


    [SerializeField] private Slider timerSlider => GameManager.Instance.TimerSlider;

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
    public override void OnInteractStarted()
    {
        if (thirdPersoncinemachineCamera.activeSelf)
        {
            //evil bandaid
            timerSlider.gameObject.SetActive(false);
            PlayerManager.Instance.PlayerGhostObject.transform.position = ghostSpawnPoint.position;
            PlayerManager.Instance.PossessGhost(GetComponent<PossessableObject>());
            IsLeaving = true;

            if (freezeCoroutine == null)
            { 
                freezeCoroutine = StartCoroutine(ReFreezeConstraints());
            }
        }
    }

    public override void WhileInteractHeld()
    { }

    public override void OnInteractCanceled()
    {
    }

    public override void OnPossessionStarted()
    {
        IsLeaving = false;
    }

    public override void OnPossessionEnded()
    {
        Debug.Log("meow meow");
        PlayerManager.Instance.PlayerGhostObject.transform.position = ghostSpawnPoint.position;
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

