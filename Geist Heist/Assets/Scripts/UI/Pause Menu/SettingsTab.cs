/*
 * Contributors: Toby
 * Creation: 11/15/2025
 * Last Edited: 11/24/25
 * 
 * Description: Manages UI elements and settings data.
 * Settings variables are stored and accessed in SettingsProfile.cs
 */

using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsTab : PauseMenuTab
{
    [Header("Non-settings buttons")]
    //[SerializeField, Required] private Button exitSettingsButton;
    [SerializeField, Required] private Button resetToDefaultsButton;
    [SerializeField] private string resetToDefaultsConfirmationText = "Reset all settings?";

    [Header("Individual Settings attributes")]
    [SerializeField] private SliderSettingsAttributes lookSensitivityAttributes;
    [SerializeField] private ToggleSettingsAttributes invertLookAttributes; 
    [SerializeField] private SliderSettingsAttributes brightnessAttributes;

    [SerializeField] private SliderSettingsAttributes masterVolumeAttributes;
    [SerializeField] private SliderSettingsAttributes musicVolumeAttributes;
    [SerializeField] private SliderSettingsAttributes sfxVolumeAttributes;
    [SerializeField] private SliderSettingsAttributes vocalsVolumeAttributes;

    private PostProcessingManager ppManager; // lol peepeeManager

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
    protected /*override*/ void Start()
    {
        //base.Start();
        ppManager = Camera.main.GetComponentInChildren<PostProcessingManager>();

        AddComponentListeners();
        //exitSettingsButton.onClick.AddListener(() => CloseTab());
        resetToDefaultsButton.onClick.AddListener(OnResetToDefaultsButtonPressed);
    }

    public override void OpenTab()
    {
        base.OpenTab();
    }

    public override void CloseTab()
    {
        SettingsProfile.SaveCurrentSettings();
        base.CloseTab();
    }

    #region Misc Buttons

    private void OnResetToDefaultsButtonPressed()
    {
        pauseMenu.confirmationPopup.OpenConfirmationPopup(resetToDefaultsConfirmationText, OnConfirmationButtonClicked: OnConfirmResetToDefaultsButtonPressed);
    }

    private void OnConfirmResetToDefaultsButtonPressed()
    {
        SettingsProfile.ResetToDefaults();
        RefreshUI();
    }

    #endregion

    #region Input Handling
    private void AddComponentListeners()
    {
        lookSensitivityAttributes.SliderComponent.onValueChanged.AddListener((float _) => OnSliderValueChanged(lookSensitivityAttributes, ref SettingsProfile.LookSensitivity, 
            minValue:SettingsProfile.MIN_LOOK_SENSITIVITY, maxValue:SettingsProfile.MAX_LOOK_SENSITIVITY,
            onSettingsUpdatedCallback:PlayerManager.Instance.UpdateCamerasSensitivity));

        invertLookAttributes.ToggleComponent.onValueChanged.AddListener((bool _) => OnToggleValueChanged(invertLookAttributes, ref SettingsProfile.InvertLook,
            onSettingsUpdatedCallback: PlayerManager.Instance.UpdateCamerasInvertLook));

        brightnessAttributes.SliderComponent.onValueChanged.AddListener((float _) => OnSliderValueChanged(brightnessAttributes, ref SettingsProfile.Brightness,
            minValue: SettingsProfile.MIN_BRIGHTNESS, maxValue: SettingsProfile.MAX_BRIGHTNESS,
            onSettingsUpdatedCallback: ppManager.UpdateBrightness));

        masterVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => OnSliderValueChanged(masterVolumeAttributes, ref SettingsProfile.MasterVolume));
        masterVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => AudioManager.Instance.UpdateMusicVolume());

        musicVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => OnSliderValueChanged(musicVolumeAttributes, ref SettingsProfile.MusicVolume));
        musicVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => AudioManager.Instance.UpdateMusicVolume());

        sfxVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => OnSliderValueChanged(sfxVolumeAttributes, ref SettingsProfile.SFXVolume));
        sfxVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => AudioManager.Instance.UpdateSFXVolume());

        vocalsVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => OnSliderValueChanged(vocalsVolumeAttributes, ref SettingsProfile.VocalsVolume));
        vocalsVolumeAttributes.SliderComponent.onValueChanged.AddListener((float _) => AudioManager.Instance.UpdateVocalsVolume());
    }

    /// <summary>
    /// Set settings variable in SettingsProfile
    /// </summary>
    private void OnSliderValueChanged(SliderSettingsAttributes sliderAttributes, ref float settingsProfileVariable, UnityAction onSettingsUpdatedCallback=null,
        float minValue=0, float maxValue=100)
    {
        // Extra math because you dont know what the max and min values of the slider (in unity inspector) are going to be:
        var slider = sliderAttributes.SliderComponent;
        float t = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
        float realValue = Mathf.Lerp(minValue, maxValue, t);

        // Apply it to settings profile.
        settingsProfileVariable = realValue;

        // sliderAttributes.RefreshComponent(realValue, t); // conflicts with current input
        sliderAttributes.RefreshTextOnly(realValue);

        if (onSettingsUpdatedCallback != null)
            onSettingsUpdatedCallback();

        SettingsProfile.SaveCurrentSettings();
    }

    /// <summary>
    /// Set settings variable in SettingsProfile
    /// </summary>
    private void OnToggleValueChanged(ToggleSettingsAttributes toggleAttributes, ref bool settingsProfileVariable, UnityAction onSettingsUpdatedCallback = null)
    {
        bool realValue = toggleAttributes.ToggleComponent.isOn;

        // apply to settings profile
        settingsProfileVariable = realValue;

        //toggleAttributes.RefreshComponent(realValue);

        if (onSettingsUpdatedCallback != null)
            onSettingsUpdatedCallback();
    }
    #endregion

    /// <summary>
    /// Makes all settings UI match their current values
    /// </summary>
    public override void RefreshUI()
    {
        // Assumes SettingsProfile.ReadSavedSettings has already run 

        lookSensitivityAttributes.RefreshComponent(SettingsProfile.LookSensitivity, SettingsProfile.LookSensitityScalar);
        invertLookAttributes.RefreshComponent(SettingsProfile.InvertLook);
        brightnessAttributes.RefreshComponent(SettingsProfile.Brightness, SettingsProfile.BrightnessScalar);

        masterVolumeAttributes.RefreshComponent(SettingsProfile.MasterVolume, SettingsProfile.MasterVolumeTransformed);
        musicVolumeAttributes.RefreshComponent(SettingsProfile.MusicVolume, SettingsProfile.MusicVolumeTransformed);
        sfxVolumeAttributes.RefreshComponent(SettingsProfile.SFXVolume, SettingsProfile.SFXVolumeTransformed);
        vocalsVolumeAttributes.RefreshComponent(SettingsProfile.VocalsVolume, SettingsProfile.VocalsVolumeTransformed);
    }
}

/// <summary>
/// Default values are stored in settings profile.
/// </summary>
[System.Serializable]
public class SliderSettingsAttributes
{
    //[Range(0, 100)] public float DefaulValue = 100;
    [AllowNesting, Required] public Slider SliderComponent;
    [AllowNesting, Required] public TMP_Text OutputTextComponent;

    public void RefreshComponent(float currentValue, float currentValueTransformed)
    {
        SliderComponent.value = currentValueTransformed;
        RefreshTextOnly(currentValue);
    }

    public void RefreshTextOnly(float currentValue)
    {
        OutputTextComponent.text = Mathf.RoundToInt(currentValue).ToString();
    }
}

[System.Serializable]
public class ToggleSettingsAttributes
{
    [AllowNesting, Required] public Toggle ToggleComponent;

    public void RefreshComponent(bool currentValue)
    {
        ToggleComponent.isOn = currentValue;
    }
}