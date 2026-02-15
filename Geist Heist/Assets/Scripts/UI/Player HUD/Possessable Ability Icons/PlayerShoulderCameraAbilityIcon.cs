/*
 * Contributors: Toby
 * Creation Date: 2/15/2026
 * Last Modified: 2/15/2026
 * 
 * Brief Description: Applies the render texture from PlayerShoulderCamera.cs
 * provides a 3rd person view of the player ghost.
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShoulderCameraAbilityIcon : PossessionAbilityIcon
{
    [SerializeField, Required] private RawImage outputImage;

    private static PlayerShoulderCamera playerShoulderCamera;

    public override void OnPossessionStarted(PossessableObject possessable)
    {
        if (playerShoulderCamera == null)
            playerShoulderCamera = FindFirstObjectByType<PlayerShoulderCamera>();

        outputImage.texture = playerShoulderCamera.OutputRenderTexture;
    }
}
