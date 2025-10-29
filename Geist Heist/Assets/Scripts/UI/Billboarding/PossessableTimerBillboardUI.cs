/*
 * Contributors: Toby
 * Creation Date: 10/28/2025
 * Last Modified: 10/28/2025
 * 
 * Brief Description: 
 * 
 * TODO: it would be cool if the timer got bigger when its almost out
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class PossessableTimerBillboardUI : IBillboardUI
{
    [Header("Possessable timer settings")]
    [SerializeField] private float hideSeconds = 0.2f;
    [SerializeField] private float opacityWhenUnpossessed = 0.5f;
    [SerializeField] private float percentToHide = 0.08f;

    private float targetOpacity => PlayerManager.Instance.CurrentObject == possessable ? 1 : opacityWhenUnpossessed;
    private float opacityByTimeRemaining => (t <= percentToHide || t>= 1 - percentToHide) ? 0 : 1; // dont show if percent is almost 0 or almost full.

    private PossessableObject possessable;
    private CanvasGroup group;
    private Slider slider;

    float t;
    public override void OnInitialize(GameObject sourceGameObject)
    {
        group = GetComponent<CanvasGroup>();
        slider = GetComponent<Slider>();
        possessable = sourceGameObject.GetComponent<PossessableObject>();
        possessable.OnTimerUpdate.AddListener(OnTimerUpdate);

        group.alpha = 0;
    }

    private void OnTimerUpdate(float percentage)
    {
        t = percentage / possessable.maxChargePercentage;
        slider.value = t;
    }

    protected override float CalculateOpacity(float playerDistance, Vector3 UIPosition)
    {
        float a = base.CalculateOpacity(playerDistance, UIPosition);

        // This sounds harsh, but CalculateAndSetOpacity smooths the opacity so its okay
        return a * targetOpacity * opacityByTimeRemaining;
    }
}
