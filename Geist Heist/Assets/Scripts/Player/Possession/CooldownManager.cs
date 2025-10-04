/*
 * Contributors: Skylar
 * Creation Date: 10/2/25
 * Last Modified: 10/3/25
 * 
 * Brief Description: Handles third person movement and interaction
 */
using UnityEngine;
using UnityEngine.UI;
using System;

public class CooldownManager : Singleton<CooldownManager>
{
    public event Action OnCooldownFinished;

    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private float cooldownTime;
    private float currentCooldownTime;
    private bool isCooldownActive = false;
    
    public bool IsCooldownActive => isCooldownActive;

    void Update()
    {
        if (isCooldownActive)
        {
            currentCooldownTime -= Time.deltaTime;
            if(currentCooldownTime <= 0)
            {
                currentCooldownTime = 0;
                isCooldownActive = false;
                OnCooldownFinished?.Invoke();
            }
            UpdateSlider();
        }
    }

    public void StartCooldown()
    {
        currentCooldownTime = cooldownTime;
        isCooldownActive = true;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if(cooldownSlider != null)
        {
            cooldownSlider.value = currentCooldownTime / cooldownTime;
        }
    }

    public void ResetCooldown()
    {
        currentCooldownTime = cooldownTime;
        isCooldownActive = true;
        UpdateSlider();
    }
}
