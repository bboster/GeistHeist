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
    [Tooltip("Max angle difference between player's forward and this anchor points forward vector")]
    [SerializeField] private float maxAnglesDifference = 20;
    [SerializeField] private float rotationSpeed = 5;
    [Tooltip("Size of the render canvas, in pixels")]
    [SerializeField] private int renderSize = 128;

    [Header("Components")]
    [SerializeField, Required] private Transform anchorPoint;
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
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        // Move to player
        anchorPoint.position = followObject.position + followObjectPositionDifference;

        float thisAngle = anchorPoint .eulerAngles.y;
        float thatAngle = followObject.eulerAngles.y;
        float difference = Mathf.DeltaAngle(thisAngle, thatAngle);

        // return if the camera is close enough to the players look rotation
        if (difference < maxAnglesDifference)
            return;

        float newAngle = Mathf.MoveTowardsAngle(thisAngle, thatAngle, Time.deltaTime * rotationSpeed);
    }
}
