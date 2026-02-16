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
    private CinemachineInputAxisController inputAxisController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        inputAxisController = GetComponent<CinemachineInputAxisController>();

        // instantiate new empty
        tempCameraPivot = new GameObject("Temp camera pivot").transform;
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
        if (cameraAnchor != null)
            Debug.DrawLine(startPoint, cameraAnchor.position, Color.blue, transitionSeconds * 2);

        while (timeElapsed < transitionSeconds)
        {
            timeElapsed = Time.time - timeStarted;
            float t = timeElapsed / transitionSeconds;

            if (cameraAnchor != null)
                tempCameraPivot.position = Vector3.Lerp(startPoint, cameraAnchor.position, t);
            yield return null;
        }

        cinemachineCamera.Follow = cameraAnchor;
    }

    #region Settings

    public void UpdateAllSettings()
    {
        UpdateCameraInvertLook();
        UpdateCameraSensitivity();
    }

    public void UpdateCameraSensitivity()
    {
        if (inputAxisController == null)
        {
            Debug.LogWarning($"{gameObject.name} has not CinemachineInputAxisController. cant update sensitivity");
            return;
        }

        // apply sensitivity to every axis (yes it HAS to be iterated for some reason)
        foreach (var c in inputAxisController.Controllers)
        {
            c.Input.LegacyGain = Mathf.Sign(c.Input.LegacyGain) * SettingsProfile.LookSensitivityTransformed;
            c.Input.Gain = Mathf.Sign(c.Input.Gain) * SettingsProfile.LookSensitivityTransformed;
        }
    }

    public void UpdateCameraInvertLook()
    {
        if (inputAxisController == null)
        {
            Debug.LogWarning($"{gameObject.name} has no CinemachineInputAxisController. Can't update inverted look");
            return;
        }

        // apply sensitivity to every axis (yes it HAS to be iterated for some reason)
        foreach (var c in inputAxisController.Controllers)
        {
            var axisName = c.Name;
            // horrible and hard-coded but there is not a better way to do this (that I could find)
            if (axisName == "Look Orbit Y" || axisName == "Mouse Y" || axisName == "Gamepad Right Stick Y") // Adjust axis names as needed
            {
                c.Input.Gain = (SettingsProfile.InvertLook ? 1 : -1) * SettingsProfile.LookSensitivityTransformed;
                c.Input.LegacyGain = (SettingsProfile.InvertLook ? -1 : 1) * SettingsProfile.LookSensitivityTransformed;
            }
        }
    }
    #endregion
}
