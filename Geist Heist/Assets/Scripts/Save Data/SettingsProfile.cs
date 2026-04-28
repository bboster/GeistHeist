/*
 * Contributors: Toby
 * Creation: 11/15/2025
 * Last Edited: 11/15/25
 * 
 * Description: Static class that holds settings data for all scripts to use. 
 * Saves and loads its variables via PlayerPrefs.
 * 
 * I want designers to touch this script
 */

using UnityEngine;

public static class SettingsProfile
{
    // Default Display values
    private const float DEFAULT_LOOK_SENSITIVITY = 100;
    private const bool DEFAULT_INVERT_X_LOOK = false;
    private const bool DEFAULT_INVERT_Y_LOOK = false;
    private const float DEFAULT_BRIGHTNESS = 50; // evaluates to 0

    private const float DEFAULT_MASTER_VOLUME = 100;
    private const float DEFAULT_MUSIC_VOLUME = 100;
    private const float DEFAULT_SFX_VOLUME = 100;
    private const float DEFAULT_VOCALS_VOLUME = 100;

    // other values
    public const float MIN_LOOK_SENSITIVITY = 1;
    public const float MAX_LOOK_SENSITIVITY = 150;
    private const float DEFAULT_LOOK_SENSITIVITY_TRANSFORMED = 1; // Real value, used in game

    public const float MIN_BRIGHTNESS = 0;
    public const float MAX_BRIGHTNESS = 100;
    private const float MIN_BRIGHTNESS_TRANSFORMED = -0.5f;
    private const float MAX_BRIGHTNESS_TRANSFORMED = 0.5f;

    #region Player Pref Keys

    /* These could be ints and that would be faster, but I'm keeping them 
       as strings for now for legibility and to avoid any mixups. */
    //TODO: if performance is a problem, convert string keys to ints.

    private const string LOOK_SENSITIVITY_KEY = "Look Sensitivity";
    private const string INVERT_X_LOOK_KEY = "Invert X Look";
    private const string INVERT_Y_LOOK_KEY = "Invert Y Look";
    private const string BRIGHTNESS_KEY = "Brightness";
    private const string MASTER_VOLUME_KEY = "Master volume";
    private const string MUSIC_VOLUME_KEY = "Music volume";
    private const string SFX_VOLUME_KEY = "SFX volume";
    private const string VOCALS_VOLUME_KEY = "Vocals volume";

    #endregion

    // Current variables
    public static bool InvertXLook;
    public static bool InvertYLook;

    public static float LookSensitivity, Brightness, 
        MasterVolume, MusicVolume, SFXVolume, VocalsVolume;

    // Technical values:
    public static float LookSensitityScalar =>
        Mathf.InverseLerp(MIN_LOOK_SENSITIVITY, MAX_LOOK_SENSITIVITY, LookSensitivity);
    public static float LookSensitivityTransformed =>
        Mathf.LerpUnclamped(0.1f, DEFAULT_LOOK_SENSITIVITY_TRANSFORMED,
            /* t: */ StaticUtilities.InverseLerpUnclamped(MIN_LOOK_SENSITIVITY, DEFAULT_LOOK_SENSITIVITY, LookSensitivity)); 
    public static float BrightnessScalar => Mathf.InverseLerp(MIN_BRIGHTNESS, MAX_BRIGHTNESS, Brightness);
    public static float BrightnessTransformed => Mathf.Lerp(MIN_BRIGHTNESS_TRANSFORMED, MAX_BRIGHTNESS_TRANSFORMED, BrightnessScalar);
    public static float MasterVolumeTransformed => MasterVolume / 100;
    public static float MusicVolumeTransformed => MusicVolume / 100;
    public static float SFXVolumeTransformed => SFXVolume / 100;
    public static float VocalsVolumeTransformed => VocalsVolume / 100;

    // Log Audio
    public static float MasterVolumeScaled => MasterVolume == 0 ? 0 : Mathf.Log10(MasterVolume) /2;
    public static float MusicVolumeScaled => MusicVolume == 0 ? 0 : Mathf.Log10(MusicVolume) /2;
    public static float SFXVolumeScaled => SFXVolume == 0 ? 0 : Mathf.Log10(SFXVolume) /2;
    public static float VocalsVolumeScaled => SFXVolume == 0 ? 0 : Mathf.Log10(SFXVolume) /2;

    /// <summary>
    /// Reads settings from PlayerPrefs and updates its public 
    /// variables. 
    /// Called in gamemanager, before managers are spawned.
    /// </summary>
    public static void ReadSavedSettings()
    {
        Debug.Log("Reading Settings Profile");

        LookSensitivity = PlayerPrefs.GetFloat(LOOK_SENSITIVITY_KEY, DEFAULT_LOOK_SENSITIVITY);
        InvertXLook = PlayerPrefs.GetInt(INVERT_X_LOOK_KEY, DEFAULT_INVERT_X_LOOK ? 1 : 0) == 1; // Playerprefs cant store bools, so just store an int
        InvertYLook = PlayerPrefs.GetInt(INVERT_Y_LOOK_KEY, DEFAULT_INVERT_Y_LOOK ? 1 : 0) == 1; 
        Brightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, DEFAULT_BRIGHTNESS);

        MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_MASTER_VOLUME);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_MUSIC_VOLUME);
        SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);
        VocalsVolume = PlayerPrefs.GetFloat(VOCALS_VOLUME_KEY, DEFAULT_VOCALS_VOLUME);
    }

    public static void SaveCurrentSettings()
    {
        Debug.Log("Saving current settings profile to settings profile");

        PlayerPrefs.SetFloat(LOOK_SENSITIVITY_KEY, LookSensitivity);
        PlayerPrefs.SetInt(INVERT_X_LOOK_KEY, InvertXLook ? 1 : 0); // Playerprefs cant store bools, so just store an int
        PlayerPrefs.SetInt(INVERT_Y_LOOK_KEY, InvertYLook ? 1 : 0); 
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, Brightness);

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, MasterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, MusicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SFXVolume);
        PlayerPrefs.SetFloat(VOCALS_VOLUME_KEY, VocalsVolume);
    }

    public static void ResetGameplayToDefaults()
    {
        Debug.Log("Reseting all gameplay settings to defaults");
        LookSensitivity = DEFAULT_LOOK_SENSITIVITY;
        InvertXLook = DEFAULT_INVERT_X_LOOK;
        InvertYLook = DEFAULT_INVERT_Y_LOOK;
        Brightness = DEFAULT_BRIGHTNESS;
        SaveCurrentSettings();
    }

    public static void ResetAudioToDefaults()
    {
        Debug.Log("Reseting all audio settings to defaults");
        MasterVolume = DEFAULT_MASTER_VOLUME;
        MusicVolume = DEFAULT_MUSIC_VOLUME;
        SFXVolume = DEFAULT_SFX_VOLUME;
        VocalsVolume = DEFAULT_VOCALS_VOLUME;
        SaveCurrentSettings();
    }
}
