/*
 * Contributors: Toby
 * Creation Date: 2/15/2026
 * Last Modified: 2/15/2026
 * 
 * Brief Description: Creates a render texture with a little 
 * view of the player. Works by following the player and trying 
 * to keep within a certain rotation range.
 */

using NaughtyAttributes;
using UnityEngine;

public class PlayerShoulderCamera : MonoBehaviour
{
    [InfoBox("This transform will dechild itself at Start")]

    [Header("Settings")]
    [SerializeField] private float hoverHeight = 1;
    [Tooltip("Max angle difference between player's forward and this anchor points forward vector")]
    [SerializeField] private float maxAnglesDifference = 20;
    [SerializeField] private float fastRotationSpeed = 100;
    [SerializeField] private float slowRotationSpeed = 10;
    [Tooltip("Size of the render canvas, in pixels")]
    [SerializeField] private int renderSize = 128;

    [Header("Components")]
    [SerializeField, Required] private Transform anchorPoint;
    [SerializeField, Required] private Transform playerModel;
    [SerializeField, Required] private Camera renderCamera;

    [ReadOnly] public RenderTexture OutputRenderTexture;

    private Transform followObject;
    private Vector3 followObjectPositionDifference;

    void Start()
    {
        // Create rendertexture with code so we dont have to deal with github bulk.
        OutputRenderTexture = new RenderTexture(renderSize, renderSize, depth: 1, RenderTextureFormat.ARGB32);
        OutputRenderTexture.Create();
        renderCamera.targetTexture = OutputRenderTexture;

        followObject = transform.parent;
        followObjectPositionDifference = followObject.position - anchorPoint.position;

        // De-child this so it doesnt use the parents rotation.
        //transform.SetParent(null);
        // ^ uncomment this when proper rotation code happens
    }

    /// <summary>
    /// this update still runs even if the player is possessing something else.
    /// </summary>
    void Update()
    {
        // Move to player
        anchorPoint.position = followObject.position + followObjectPositionDifference
            - new Vector3(0, StaticUtilities.SinRange(Time.time, 0, hoverHeight)); // This is the hover bob

        RotateRenderCamera();
    }

    /// <summary>
    /// Rotate the camera to match the direction the player is facing
    /// </summary>
    private void RotateRenderCamera()
    {
        // Do nothing for now, until beta. I couldnt get it looking good in time.
        return;

        // Rotate camera
        float thisAngle = anchorPoint.eulerAngles.y;
        float thatAngle = playerModel.eulerAngles.y;
        float difference = Mathf.DeltaAngle(thatAngle, thisAngle);

        Debug.Log(difference);

        float newAngle;

        // if its close enough
        if (Mathf.Abs(difference)-maxAnglesDifference < 5)
            return;

        // if the camera is close enough to the players look rotation
        if (Mathf.Abs(difference) < maxAnglesDifference)
        {
            // Look left if already mostly looking left
            if (difference < 0)
                newAngle = Mathf.MoveTowardsAngle(thisAngle, thatAngle - maxAnglesDifference, Time.deltaTime * slowRotationSpeed);
            // look right if already mostly looking right
            else
                newAngle = Mathf.MoveTowardsAngle(thisAngle, thatAngle + maxAnglesDifference, Time.deltaTime * slowRotationSpeed);
        }
        // If player is looking really far away from the render camera
        else
            newAngle = Mathf.MoveTowardsAngle(thisAngle, thatAngle, Time.deltaTime * fastRotationSpeed);

        anchorPoint.eulerAngles = new Vector3(0, newAngle, 0);
    }
}
