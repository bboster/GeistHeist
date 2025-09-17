/*
 * Contributors: Toby S
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * First person camera controller :P
 */

using UnityEngine;

public class FirstPersonCamera : ICameraInputHandler
{
    [SerializeField] private float maxPitch = 85;

    // pitch = up/down
    // yaw = left/right
    private float pitch, yaw = 0;

    public override void OnMouseMove(Camera camera, Vector2 mouseDelta)
    {
        // in theory, mouseDelta should already be accounting for deltatime, so we dont need to multiply that here, probably
        pitch += mouseDelta.y * -1; // FUCK subtraction
        yaw += mouseDelta.x; 

        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        yaw = (yaw + 360) % 360;
    }

    public override CameraProperties GetCameraTransform(Camera camera)
    {
        return new CameraProperties(cameraAnchor.position, new Vector3(pitch, yaw, 0));
    }
}
