/*
 * Contributors: Toby
 * Creation Date: 12/2/2025
 * Last Modified: 12/2/2025
 * 
 * Brief Description: Reusable script. Fires events when a sudden stop, jult, or bounce is detected.
 * Bounce: sudden jult of velocity in opposite direction
 * Jult: when presumed stopped, and gains sudden velocity
 */

using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class SuddenVelocityChangeDetector : MonoBehaviour
{
    [Tooltip("Lowest change in velocity that would be considered a sudden change")]
    [SerializeField] private float minVelocityForRegister = 6;
    [Tooltip("The highest velocity that can still be considered \"stopped\"")]
    [SerializeField] private float maxVelocityToBeStopped = 0.3f;
    [SerializeField] private bool recordVelocityAtStart;
    //[SerializeField] private LayerMask collisionLayers;

    [SerializeField] private bool debugLogSpeeds;

    private const float MIN_SECONDS_BETWEEN_EVENTS = 0.15f;

    // vector3 in parameter is contact point
    public UnityEvent<Vector3> OnStopDetected = new();
    public UnityEvent<Vector3> OnBounceDetected = new();
    public UnityEvent<Vector3> OnJoltDetected = new();


    private bool activelyRecordVelocity;

    private Rigidbody rb;

    private Vector3 lastVelocity;
    private float timeOfLastStop, timeOfLastBounce, timeOfLastJolt = 0;
    private float timeOfLastSuddenChange => Mathf.Max(Mathf.Max(timeOfLastStop, timeOfLastBounce), timeOfLastJolt);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (recordVelocityAtStart)
            StartRecordingVelocity();
    }

    // Update is called once per frame
    IEnumerator RecordVelocity()
    {
        while (activelyRecordVelocity)
        {
            lastVelocity = rb.linearVelocity;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (activelyRecordVelocity == false) return;

        // check if collision layer is in walls layer mask using a BITWISE operation??? (what is happening!!)
        //int layer = collision.transform.gameObject.layer;
        //if (collisionLayers == (collisionLayers | (1 << layer)))
        //    return;

        Debug.Log("collision good start coroutine");
        StartCoroutine(AfterCollisionEnter(collision.contacts[0].point));
    }

    // one frame after, actually.
    private IEnumerator AfterCollisionEnter(Vector3 impactPoint)
    {
        if(Time.time - timeOfLastSuddenChange < MIN_SECONDS_BETWEEN_EVENTS)
        {
            Debug.Log("Duplicate collision detected.");
            yield break;
        }

        Vector3 cachedLastVelocity = lastVelocity;

        yield return new WaitForEndOfFrame();

        float speedBeforeCollision = cachedLastVelocity.magnitude;
        float speedAfterCollision = rb.linearVelocity.magnitude;

        if (debugLogSpeeds)
        {
            Debug.Log("speed before: " + speedBeforeCollision);
            Debug.Log("speed after:  " + speedBeforeCollision);
        }

        // no sighnificant change has happened
        if (Mathf.Abs(speedBeforeCollision - speedAfterCollision) < minVelocityForRegister)
        {
            yield break;
        }

        // detect jolt : if it was stopped and suddenly started
        if (speedBeforeCollision <= maxVelocityToBeStopped && speedAfterCollision >= minVelocityForRegister
            /*&& timeOfLastJolt - Time.time > MIN_SECONDS_BETWEEN_EVENTS*/)
        {
            Debug.Log("sudden jult on "+gameObject.name);
            timeOfLastJolt = Time.time;
            OnJoltDetected.Invoke(impactPoint);
            yield break;
        }

        // detect stop: if it was moving and suddenly stopped
        if (speedBeforeCollision >= minVelocityForRegister && speedAfterCollision <= maxVelocityToBeStopped
            /*&& timeOfLastStop - Time.time > MIN_SECONDS_BETWEEN_EVENTS*/)
        {
            Debug.Log("Stop occured on " + gameObject.name);
            //timeOfLastSuddenChange = Time.time;
            timeOfLastStop = Time.time;
            OnStopDetected.Invoke(impactPoint);
            yield break;
        }

        // detect bounce: if it was moving and suddenly starting moving in opposite direction
        float dot = Vector3.Dot(cachedLastVelocity.normalized, rb.linearVelocity.normalized); // dot returns 1 if angles are perfectly aligned, -1 if complete opposite directions.
        if (dot <= 0 && speedBeforeCollision >= minVelocityForRegister && speedAfterCollision >= minVelocityForRegister // "if the two directions are different, but also very fast"
            /*&& timeOfLastBounce - Time.time > MIN_SECONDS_BETWEEN_EVENTS*/) 
        {
            Debug.Log("bounce detected on "+gameObject.name);
            timeOfLastBounce = Time.time;
            OnBounceDetected.Invoke(impactPoint);
            yield break;
        }
    }

    public void StartRecordingVelocity()
    {
        lastVelocity = rb.linearVelocity;
        activelyRecordVelocity = true;
        StartCoroutine(RecordVelocity());
    }

    public void StopRecordingVelocity()
    {
        // this stops the coroutine
        activelyRecordVelocity = false;
    }

    private void OnDestroy()
    {
        OnBounceDetected.RemoveAllListeners();
        OnJoltDetected.RemoveAllListeners();
        OnStopDetected.RemoveAllListeners();
    }
}
