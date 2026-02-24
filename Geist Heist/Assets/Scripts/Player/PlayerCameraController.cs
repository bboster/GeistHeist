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
    private CinemachineOrbitalFollow orbitalFollow;

    private float minFov;

    [Tooltip("Maximum FOV when camera reaches max vertical height.")]
    [SerializeField] private float maxFov = 75f;

    [SerializeField] private float fovSmoothSpeed = 5f;

    [SerializeField] private SpecialCameraType specialCameraType;

    void Start()
    {
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        inputAxisController = GetComponent<CinemachineInputAxisController>();
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();

        if(cinemachineCamera == null)
        {
            Debug.LogError("cinemachineCamera is null");
            minFov = 75;
        }
        else
            minFov = cinemachineCamera.Lens.FieldOfView;

        tempCameraPivot = new GameObject("Temp camera pivot").transform;
    }

    private void Update()
    {
        if (specialCameraType == SpecialCameraType.VendingMachine)
        {
            UpdateFOVFromHeight();
        }
    }

    /// <summary>
    /// Updates FOV dynamically based on the vertical axis range of the orbital follow.
    /// </summary>
    /// A bit of a lazy solution but it works for the use case, if other possessables need similar camera behavior
    /// I can make this method more universal.
    private void UpdateFOVFromHeight()
    {
        if (orbitalFollow == null)
            return;

        var verticalAxis = orbitalFollow.VerticalAxis;

        float currentHeight = verticalAxis.Value;
        float minHeight = verticalAxis.Range.x;
        float maxHeight = verticalAxis.Range.y;

        // Normalize height based on axis-defined range
        float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, currentHeight);

        float targetFov = Mathf.Lerp(minFov, maxFov, normalizedHeight);

        float currentFov = cinemachineCamera.Lens.FieldOfView;
        float smoothedFov = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * fovSmoothSpeed);

        cinemachineCamera.Lens.FieldOfView = smoothedFov;
    }

    /// <summary>
    /// Sets camera follow point to a temporary transform and lerps that anchor to the new one.
    /// </summary>
    public void SetAnchorPoint(Transform cameraAnchor)
    {
        StaticUtilities.StopAndStartCoroutine(ref moveTrackingPointCoroutine, SmoothSetAnchorPoint(cameraAnchor));
    }

    private IEnumerator SmoothSetAnchorPoint(Transform cameraAnchor)
    {
        tempCameraPivot.position = cinemachineCamera.Follow.position;
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

    public enum SpecialCameraType
    {
        VendingMachine
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

        foreach (var c in inputAxisController.Controllers)
        {
            var axisName = c.Name;

            if (axisName == "Look Orbit Y" || axisName == "Mouse Y" || axisName == "Gamepad Right Stick Y")
            {
                c.Input.Gain = (SettingsProfile.InvertLook ? 1 : -1) * SettingsProfile.LookSensitivityTransformed;
                c.Input.LegacyGain = (SettingsProfile.InvertLook ? -1 : 1) * SettingsProfile.LookSensitivityTransformed;
            }
        }
    }

    #endregion
}
