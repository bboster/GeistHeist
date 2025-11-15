/*
 * Contributors: Toby
 * Creation: 11/15/2025
 * Last Edited: 11/15/25
 * 
 * Description: Manages UI elements and settings data.
 */

using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    #region Attribute variables
    /*
    // Master Volume
    [BoxGroup("Master Volume"), Label("Default Value"), Range(0, 100), SerializeField] private float defaultMasterVolume = 100;
    [BoxGroup("Master Volume"), Label("Slider"), Required, SerializeField] private Slider masterVolumeSlider;
    [BoxGroup("Master Volume"), Label("Output Text"), Required, SerializeField] private TMP_Text masterVolumeOutputText;
    [SerializeField, ReadOnly] private float currentMasterVolume = 0;
    private const string MASTER_VOLUME_PLAYER_PREF_KEY = "Master Volume";

    // Music Volume
    [BoxGroup("Music Volume"), Label("Default Value"), Range(0, 100), SerializeField] private float defaultMusicVolume = 100;
    [BoxGroup("Music Volume"), Label("Slider"), Required, SerializeField] private Slider MusicVolumeSlider;
    [BoxGroup("Music Volume"), Label("Output Text"), Required, SerializeField] private TMP_Text MusicVolumeOutputText;
    [SerializeField, ReadOnly] private float currentMusicVolume = 0;
    private const string MUSIC_VOLUME_PLAYER_PREF_KEY = "Music Volume";

    // SFX Volume
    [BoxGroup("SFX Volume"), Label("Default Value"), Range(0, 100), SerializeField] private float defaultSFXVolume = 100;
    [BoxGroup("SFX Volume"), Label("Slider"), Required, SerializeField] private Slider SFXVolumeSlider;
    [BoxGroup("SFX Volume"), Label("Output Text"), Required, SerializeField] private TMP_Text SFXVolumeOutputText;
    [SerializeField, ReadOnly] private float currentSFXVolume = 0;
    private const string SFX_VOLUME_PLAYER_PREF_KEY = "SFX Volume";

    // Vocals Volume
    [BoxGroup("Vocals Volume"), Label("Default Value"), Range(0, 100), SerializeField] private float defaultVocalsVolume = 100;
    [BoxGroup("Vocals Volume"), Label("Slider"), Required, SerializeField] private Slider VocalsVolumeSlider;
    [BoxGroup("Vocals Volume"), Label("Output Text"), Required, SerializeField] private TMP_Text VocalsVolumeOutputText;
    [SerializeField, ReadOnly] private float currentVocalsVolume = 0;
    private const string VOCALS_VOLUME_PLAYER_PREF_KEY = "Music Volume";*/

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class SettingsAttributes
{
    //[Range(0, 100)] public float DefaulValue = 100;
    [Required] public Slider SliderComponent;
    [Required] public TMP_Text OutputTextComponent;
}