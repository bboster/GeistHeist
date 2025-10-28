/*
 * Contributors: Toby
 * Creation Date: 10/28/2025
 * Last Modified: 10/28/2025
 * 
 * Brief Description: 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class PossessableTimerBillboardUI : IBillboardUI
{
    [Header("Possessable timer settings")]
    [SerializeField] private float hideSeconds = 0.2f;

    private PossessableObject possessable;
    private CanvasGroup group;
    private Slider slider;

    private Coroutine hideTimerCoroutine;
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

        float t = percentage / possessable.maxChargePercentage;
        slider.value = t;

        // start hiding if going down
        if(t <= 0.05)
        {
            if (hideTimerCoroutine == null)
                hideTimerCoroutine = StartCoroutine(HideTimer());
        }
        // Show wheel if high value
        else
        {
            if (hideTimerCoroutine != null)
                StopCoroutine(hideTimerCoroutine);

            group.alpha = Mathf.MoveTowards(group.alpha, 1, Time.deltaTime * 30);
        }
    }

    private IEnumerator HideTimer()
    {
        while(group.alpha > 0)
        {

            group.alpha = Mathf.MoveTowards(group.alpha, 0, Time.deltaTime / hideSeconds);
            yield return null;
        }
    }
}
