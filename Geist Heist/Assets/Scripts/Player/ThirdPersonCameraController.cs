/*
 * Contributors: Josh
 * Creation Date: 9/16/25
 * Last Modified: 9/17/25
 * 
 * Brief Description: interface for anything that can handle player input.
 *  To be placed on any possessible object, and the player ghost.
 *  Handles camera stuff.
 *  
 *  TODO: this should probably need a reference to the camera and an anchor point (for 3rd person).
 *  Also a reference to the thing the camera is orbiting
 */
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera
{
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 5f;

    private PlayerControls controls;

    private CinemachineCamera camera;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currentZoom;

    void Start()
    {
        controls = new PlayerControls();
        controls.Enable();
        controls.CameraControls.MouseZoom.performed += HandleMouseScroll;

        Cursor.lockState = CursorLockMode.Locked;

        camera = GetComponent<CinemachineCamera>();
        orbital = camera.GetComponent<CinemachineOrbitalFollow>();

        targetZoom = currentZoom = orbital.Radius;
    }

    public void HandleMouseScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
        Debug.Log($"Scroll Delta:   {scrollDelta}");
    }


    void Update()
    {
        // Zoom
        if (scrollDelta.y != 0)
        {
            if (orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance);
                scrollDelta = Vector2.zero;
            }
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }
}
