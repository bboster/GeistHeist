/*
 * Contributors: Toby
 * Creation Date: 2/15/2026
 * Last Modified: 2/15/2026
 * 
 * Brief Description: Applies the render texture from PlayerShoulderCamera.cs
 * provides a 3rd person view of the player ghost.
 */

using NaughtyAttributes;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShoulderCameraAbilityIcon : PossessionAbilityIcon
{
    [SerializeField, Required] private RawImage outputImage;

    private static PlayerShoulderCamera playerShoulderCamera;

    public override void OnPossessionStarted(PossessableObject possessable)
    {
        GetPlayerShoulderCamera();
        outputImage.texture = playerShoulderCamera.OutputRenderTexture;
    }

    private async void GetPlayerShoulderCamera()
    {
        while (playerShoulderCamera == null)
        {
            playerShoulderCamera = FindFirstObjectByType<PlayerShoulderCamera>();
            // wait to next frame pretty much
            await Task.Delay(1);
        }


        outputImage.texture = playerShoulderCamera.OutputRenderTexture;
    }
}
