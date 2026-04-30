/*
 * Contributors: Toby, Joshua Kelly
 * Creation: 11/15/2025
 * Last Edited: 3/1/2026
 * 
 * Description: Manages UI elements and settings data.
 * Settings variables are stored and accessed in SettingsProfile.cs
 */

using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsTab : PauseMenuTab
{
    [Header("Non-settings buttons")]
    //[SerializeField, Required] private Button exitSettingsButton;
    [SerializeField, Required] private Button resetGameplayToDefaultsButton;
    [SerializeField, Required] private Button resetAudioToDefaultsButton;
    [SerializeField] private string resetToDefaultsConfirmationText = "Reset all settings?";

    [Header("Individual Settings attributes")]
    [SerializeField] private SliderSettingsAttributes lookSensitivityAttributes;
    [SerializeField] private ToggleSettingsAttributes invertYLookAttributes; 
    [SerializeField] private ToggleSettingsAttributes invertXLookAttributes; 
    [SerializeField] private SliderSettingsAttributes brightnessAttributes;

    [SerializeField] private SliderSettingsAttributes masterVolumeAttributes;
    [SerializeField] private SliderSettingsAttributes musicVolumeAttributes;
    [SerializeField] private SliderSettingsAttributes sfxVolumeAttributes;
    [SerializeField] private SliderSettingsAttributes vocalsVolumeAttributes;

    [Header("Other")]
    [SerializeField] private bool resetConfirmation = false;
    [ShowIf(nameof(resetConfirmation)), SerializeField, Required] private ConfirmationPopup confirmationPopup;

    private PostProcessingManager ppManager; // lol peepeeManager

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected /*override*/ void Start()
    {
        SettingsProfile.ReadSavedSettings();

        //base.Start();
        ppManager = Camera.main.GetComponentInChildren<PostProcessingManager>();
        AddComponentListeners();
        //exitSettingsButton.onClick.AddListener(() => CloseTab());
        resetGameplayToDefaultsButton.onClick.AddListener(OnResetGameplayToDefaultsButtonPressed);
        resetAudioToDefaultsButton.onClick.AddListener(OnResetAudiToDefaultsButtonPressed);
    }

    public override void OpenTab()
    {
        base.OpenTab();
        EventSystem.current?.SetSelectedGameObject(lookSensitivityAttributes.SliderComponent.gameObject);
    }

    public override void CloseTab()
    {
        SettingsProfile.SaveCurrentSettings();
        base.CloseTab();
    }

    #region Misc Buttons

    private void OnResetGameplayToDefaultsButtonPressed()
    {
        if (resetConfirmation)
        {
            confirmationPopup.gameObject.SetActive(true);
            confirmationPopup.OpenConfirmationPopup(resetToDefaultsConfirmationText, OnConfirmationButtonClicked: OnConfirmResetGameplayToDefaultsButtonPressed);
        }
        else
            OnConfirmResetGameplayToDefaultsButtonPressed();
    }

    private void OnResetAudiToDefaultsButtonPressed()
    {
        if (resetConfirmation)
        {
            confirmationPopup.gameObject.SetActive(true);
            confirmationPopup.OpenConfirmationPopup(resetToDefaultsConfirmationText, OnConfirmationButtonClicked: OnConfirmResetAudioToDefaultsButtonPressed);
        }
        else
            OnConfirmResetAudioToDefaultsButtonPressed();
    }

    private void OnConfirmResetGameplayToDefaultsButtonPressed()
    {
        SettingsProfile.ResetGameplayToDefaults();
        RefreshUI();
    }

    private void OnConfirmResetAudioToDefaultsButtonPressed()
    {
        SettingsProfile.ResetAudioToDefaults();
        RefreshUI();
    }

    #endregion

    #region Input Handling
    private void AddComponentListeners()
    {
        UnityAction onSensitivitySettingsUpdatedCallback = PlayerManager.Instance == null ? null : PlayerManager.Instance.UpdateCamerasSensitivity;
        lookSensitivityAttributes.SliderComponent.onValueChanged.AddListener((float _) => OnSliderValueChanged(lookSensitivityAttributes, ref SettingsProfile.LookSensitivity, 
            minValue:SettingsProfile.MIN_LOOK_SENSITIVITY, maxValue:SettingsProfile.MAX_LOOK_SENSITIVITY,
            onSettingsUpdatedCallback: onSensitivitySettingsUpdatedCallback));

        UnityAction onCameraLookSettingsUpdatedCallback = PlayerManager.Instance == null ? null : PlayerManager.Instance.UpdateCamerasInvertLook;
        invertYLookAttributes.ToggleComponent.onValueChanged.AddListener((bool _) => OnToggleValueChanged(invertYLookAttributes, ref SettingsProfile.InvertYLook,
            onSettingsUpdatedCallback: onCameraLookSettingsUpdatedCallback));
        invertXLookAttributes.ToggleComponent.onValueChanged.AddListener((bool _) => OnToggleValueChanged(invertXLookAttributes, ref SettingsProfile.InvertXLook,
            onSettingsUpdatedCallback: onCameraLookSettingsUpdatedCallback));

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

        if (onSettingsUpdatedCallback != null && SceneManager.GetActiveScene().name != "Main Menu")
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
        invertYLookAttributes.RefreshComponent(SettingsProfile.InvertYLook);
        invertXLookAttributes.RefreshComponent(SettingsProfile.InvertXLook);
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
    public bool DisplayAsPercentage = true;

    public void RefreshComponent(float currentValue, float currentValueTransformed)
    {
        SliderComponent.value = currentValueTransformed;
        RefreshTextOnly(currentValue);
    }

    public void RefreshTextOnly(float currentValue)
    {
        OutputTextComponent.text = Mathf.RoundToInt(currentValue).ToString() + (DisplayAsPercentage ? "%" : "");
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
