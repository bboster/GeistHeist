/*
 * Contributors: Toby
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * Brief Description: interface for anything that can handle player input.
 *  To be placed on any possessible object, and the player ghost.
 *  Handles camera stuff.
 *  
 *  TODO: this should probably need a reference to the camera and an anchor point (for 3rd person).
 *  Also a reference to the thing the camera is orbiting
 */

using UnityEngine;

public abstract class CameraInputHandler
{
    public abstract void OnMouseMove(Vector2 mouseDelta);

    /// <summary>
    /// Sets positon and rotation.
    /// To be called in fixedupdate.
    /// </summary>
    public abstract void SetCameraTransform(Camera camera);

}
