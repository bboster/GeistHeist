/*
 * Contributors: Toby
 * Creation Date: 11/13/25
 * Last Modified: 11/13/25
 * 
 * Brief Description: manages camera transitions.
 */

using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private float transitionSeconds = 0.5f;

    private Transform tempCameraPivot;
    private Coroutine moveTrackingPointCoroutine;
    private CinemachineCamera cinemachineCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();

        // instantiate new empty
        tempCameraPivot = new GameObject("Temp camera pivot").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets camera follow point to a temporary transform and lerps that anchor to the new one.
    /// </summary>
    /// <param name="cameraAnchor"></param>
    public void SetAnchorPoint(Transform cameraAnchor)
    {
        StaticUtilities.StopAndStartCoroutine(ref moveTrackingPointCoroutine, SmoothSetAnchorPoint(cameraAnchor));
    }

    private IEnumerator SmoothSetAnchorPoint(Transform cameraAnchor)
    {

        // set temp anchor position to where current camera anchor is
        tempCameraPivot.transform.position = cinemachineCamera.Follow.position;
        cinemachineCamera.Follow = tempCameraPivot;

        Vector3 startPoint = tempCameraPivot.position;
        float timeStarted = Time.time;
        float timeElapsed = 0;
        Debug.DrawLine(startPoint, cameraAnchor.position, Color.blue, transitionSeconds * 2);
        while (timeElapsed < transitionSeconds)
        {
            timeElapsed = Time.time - timeStarted;
            float t = timeElapsed / transitionSeconds;

            tempCameraPivot.position = Vector3.Lerp(startPoint, cameraAnchor.position, t);
            yield return null;
        }

        cinemachineCamera.Follow = cameraAnchor;
    }
}
