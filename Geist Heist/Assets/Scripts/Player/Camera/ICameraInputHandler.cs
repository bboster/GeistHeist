/*
 * Contributors: Toby
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * Brief Description: interface for anything that can handle player input.
 *  To be placed on any possessible object, and the player ghost.
 *  Handles camera stuff.
 */

using UnityEngine;

public abstract class ICameraInputHandler : MonoBehaviour
{
    [SerializeField] protected Transform cameraAnchor;
    [SerializeField] public float sensitivityScalar = 1; // this variable should automatically get applied in PlayerManager

    //Avoid calling GetCameraTransform in this function. it is already called in PlayerManager
    public abstract void OnMouseMove(Camera camera, Vector2 mouseDelta); 

    /// <summary>
    /// Sets positon and rotation.
    /// To be called in fixedupdate.
    /// </summary>
    public abstract CameraProperties GetCameraTransform(Camera camera);

}

public struct CameraProperties
{
    public Vector3 position;
    public Quaternion rotation;

    public CameraProperties(Vector3 Position, Quaternion Rotation)
    {
        position = Position;
        rotation = Rotation;
    }

    public CameraProperties(Vector3 Position, Vector3 Rotation)
    {
        position = Position;
        rotation = Quaternion.Euler(Rotation);
    }
}