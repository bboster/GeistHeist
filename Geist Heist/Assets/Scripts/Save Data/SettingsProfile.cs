/*
 * Contributors: Toby
 * Creation: 11/15/2025
 * Last Edited: 11/15/25
 * 
 * Description: Static class that holds settings data for all scripts to use. 
 * Saves and loads its variables via PlayerPrefs
 */

using UnityEngine;

public static class SettingsProfile
{
    // hard coded values: 
    private const float DEFAULT_LOOK_SENSITIVITY = 100;
    private const bool DEFAULT_INVERT_LOOK = false;
    private const float DEFAULT_BRIGHTNESS = 80; 

    private const float DEFAULT_MASTER_VOLUME = 100;
    private const float DEFAULT_MUSIC_VOLUME = 100;
    private const float DEFAULT_SFX_VOLUME = 100;
    private const float DEFAULT_VOCALS_VOLUME = 100;

    // Current variables
    public static bool Invert_Look;

    // TODO: BRIGHTNESS NOT IMPLEMENTED
    private static float lookSensitivy, brightness, 
        masterVolume, musicVolume, sfxVolume, vocalVolume;
}
