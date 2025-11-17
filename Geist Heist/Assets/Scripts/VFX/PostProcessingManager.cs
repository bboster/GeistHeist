/*
 * Contributors: Toby
 * Creation Date: 11/17/2025
 * Last Modified: 11/17/2025
 * 
 * Brief Description: Manager for any post processing stuff. Listens to brightness value in settings menu.
 */

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    private PostProcessVolume ppVolume;
    private PostProcessProfile ppProfile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ppVolume = GetComponent<PostProcessVolume>();
        ppProfile = ppVolume.profile;

    }

    /// <summary>
    /// Update game brightness to reflect value in SettingsProfile
    /// </summary>
    public void UpdateBrightness()
    {

    }

}
