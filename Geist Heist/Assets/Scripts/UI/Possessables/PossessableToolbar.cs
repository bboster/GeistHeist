/*
 * Contributors: Toby
 * Creation Date: 2/12/2026
 * Last Modified: 2/12/2026
 * 
 * The possessable UI controller that is operated by every possessable.
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PossessableToolbar : Singleton<PossessableToolbar>
{
    [Header("Unique Possessable Icons")]
    [SerializeField, Required] private RectTransform uniqueIconPossessableParent;
    [SerializeField, Required] private RectTransform uniqueTextPossessableParent;

    [Header("Cooldown Wheel")]
    [SerializeField, Required] private Slider cooldownSlider;
    [SerializeField, Required] private CanvasGroup cooldownGroup;

    [Header("Charge Bar")]
    [SerializeField, Required] private Slider chargeSlider;
    [SerializeField, Required] private CanvasGroup chargeGroup;

    private GameObject currentIcon;

    /// <summary>
    /// GameManager -> PlayerHUD -> (this) PossessableToolbar
    /// </summary>
    public void Initialize()
    {
        HideCooldownTimer();
        SetCooldownTimerValue(1);

        HideChargeSliderBar();
        SetChargeBarValue(1);
    }

    #region Unique UI initialization

    public void SetIcon(GameObject iconPrefab, PossessableObject sourcePossessable)
    {
        if(currentIcon != null)
            Destroy(currentIcon);

        Instantiate(iconPrefab, uniqueIconPossessableParent);


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

        StaticUtilities.FadeOpacity(cooldownGroup, 1, seconds: 0.25f);
    }
    public void HideCooldownTimer()
    {
        //StaticUtilities.DisableCanvasGroup(cooldownGroup);

        StaticUtilities.FadeOpacity(cooldownGroup, 0, seconds: 0.25f);
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
