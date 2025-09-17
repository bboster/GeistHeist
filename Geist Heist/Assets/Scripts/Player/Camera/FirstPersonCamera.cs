/*
 * Contributors: Toby, Alec P, Clare G, Sky B, Tyler B
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * Connects to PlayerInput map actions and invokes static UnityEvents.
 * Use other scripts to connect to the unityevents.
 */

using UnityEngine;

public class FirstPersonCamera : ICameraInputHandler
{
    public override void OnMouseMove(Vector2 mouseDelta)
    {
        // tbh i dont know how to make a first person camera (that is good)
    }

    public override void SetCameraTransform(Camera camera)
    {
    }
}
