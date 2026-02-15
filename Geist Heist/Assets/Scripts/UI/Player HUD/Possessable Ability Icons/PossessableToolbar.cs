/*
 * Contributors: Toby
 * Creation Date: 2/12/2026
 * Last Modified: 2/12/2026
 * 
 * The possessable UI controller that is operated by every possessable.
 * Has wrapper functions for the charge bar, timer spiral.
 * Initializes ability icons and controls text.
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PossessableToolbar : Singleton<PossessableToolbar>
{
    [Header("Unique Possessable Icons")]
    [SerializeField, Required] private RectTransform uniqueIconPossessableParent;
    [SerializeField, Required] private RectTransform uniqueTextPossessableParent; //@TODO

    [Header("Cooldown Wheel")]
    [SerializeField, Required] private Slider cooldownSlider;
    [SerializeField, Required] private CanvasGroup cooldownGroup;

    [Header("Charge Bar")]
    [SerializeField, Required] private Slider chargeSlider;
    [SerializeField, Required] private CanvasGroup chargeGroup;

    private GameObject currentIcon;
    private PossessableObject currentPossessable;

    /// <summary>
    /// GameManager -> PlayerHUD -> (this) PossessableToolbar
    /// </summary>
    public void Initialize()
    {
        HideCooldownTimer();
        SetCooldownTimerValue(1);

        HideChargeSliderBar();
        SetChargeBarValue(1);

        PlayerManager.Instance.OnPossessionObjectChanged.AddListener(OnPossessableObjectChanged);

        OnPossessableObjectChanged(PlayerManager.Instance.CurrentObject);
    }

    public void OnPossessableObjectChanged(PossessableObject possessable)
    {
        // Initialize Cooldown timer wheel
        if (currentPossessable != null)
            currentPossessable.OnTimerUpdate.RemoveListener(SetCooldownTimerValue);

        if (possessable == null)
            return;

        possessable.OnTimerUpdate.AddListener(SetCooldownTimerValue);
        if (possessable.hasTimer)
            ShowCooldownTimer();
        else
            HideCooldownTimer();

        // Init charge bar
        if (possessable.HasChargeAbility)
            ShowChargeSliderBar();
        else
            HideChargeSliderBar();

        currentPossessable = possessable;
        SetAbilityIcon(possessable.AbilityIconPrefab, possessable);
    }

    #region Unique Ability Icon UI initialization

    public void SetAbilityIcon(GameObject iconPrefab, PossessableObject sourcePossessable)
    {
        if(currentIcon != null)
            Destroy(currentIcon);
            
        if(iconPrefab == null)
        {
            Debug.LogWarning($"{sourcePossessable.gameObject.name} does not have a set ability icon for the possession toolbar");
            return;
        }

        // Childed to uniqueIconPossessableParent
        currentIcon = Instantiate(iconPrefab, uniqueIconPossessableParent);

        // check if icon has set functionality
        if(currentIcon.TryGetComponent<PossessionAbilityIcon>(out PossessionAbilityIcon abilityIcon))
        {
            abilityIcon.OnPossessionStarted(sourcePossessable);
        }
    }

    #endregion

    #region Cooldown Timer
    public void SetCooldownTimerValue(float timeRemainingPercent)
    {
        cooldownSlider.value = timeRemainingPercent;
    }
    public void ShowCooldownTimer()
    {
        //StaticUtilities.EnableCanvasGroup(cooldownGroup, interactable: false);

        StaticUtilities.FadeOpacity(cooldownGroup, 1, seconds: 0.5f);
    }
    public void HideCooldownTimer()
    {
        //StaticUtilities.DisableCanvasGroup(cooldownGroup);

        StaticUtilities.FadeOpacity(cooldownGroup, 0, seconds: 0.5f);
    }
    #endregion

    #region Charge Slider Bar
    public void SetChargeBarValue(float chargePercent)
    {
        chargeSlider.value = chargePercent;
    }
    public void ShowChargeSliderBar()
    {
        //StaticUtilities.EnableCanvasGroup(chargeGroup, interactable: false);

        StaticUtilities.FadeOpacity(chargeGroup, 1, seconds: 0.25f);
    }
    public void HideChargeSliderBar()
    {
        //StaticUtilities.DisableCanvasGroup(chargeGroup);

        StaticUtilities.FadeOpacity(chargeGroup, 0, seconds: 0.25f);
    }
    #endregion
}
