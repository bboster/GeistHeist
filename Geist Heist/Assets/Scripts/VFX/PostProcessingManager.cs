/*
 * Contributors: Toby
 * Creation Date: 11/17/2025
 * Last Modified: 11/17/2025
 * 
 * Brief Description: Manager for any post processing stuff. Listens to brightness value in settings menu.
 */

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    private Volume volume;
    private VolumeProfile volumeProfile;

    private void Awake()
    {
        GameManager.OnInitialize += Initialize;
    }

    public void Initialize()
    {
        UpdateBrightness();
        GameManager.OnInitialize -= Initialize;
    }


    /// <summary>
    /// Update game brightness to reflect value in SettingsProfile
    /// </summary>
    public void UpdateBrightness()
    {
        if(volume == null || volumeProfile == null)
        {
            volume = GetComponentInChildren<Volume>();

            // make a copy of the volume, so we can edit it in runtime and not get a million github changes
            volumeProfile = Instantiate(volume.profile);
            volume.profile = volumeProfile;

            //UpdateBrightness();
        }

        // old code that made ollie really glowy
        /*
        if(volumeProfile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustment))
        {
            colorAdjustment.postExposure.overrideState = true;
            colorAdjustment.postExposure.value = SettingsProfile.BrightnessTransformed;
        }
            // color adjustment -> post exposure
        */

        if (volumeProfile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustment))
        {
            colorAdjustment.postExposure.overrideState = true;
            colorAdjustment.postExposure.value = SettingsProfile.BrightnessTransformed;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnInitialize -= Initialize;
    }
}
