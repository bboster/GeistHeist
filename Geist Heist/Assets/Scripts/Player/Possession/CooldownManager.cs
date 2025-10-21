/*
 * Contributors: Skylar
 * Creation Date: 10/2/25
 * Last Modified: 10/3/25
 * 
 * Brief Description: Handles possession cooldown
 */
using UnityEngine;
using UnityEngine.UI;
using System;

public class CooldownManager : Singleton<CooldownManager>
{
    public event Action OnCooldownFinished;

    [SerializeField] private GameObject CooldownCanvasPrefab;
    [Tooltip("Refers to the time between possessions before player can possess again")]
    [SerializeField] private float cooldownTime;
    private float currentCooldownTime=0;

    [HideInInspector] public GameObject CooldownCanvas;
    [HideInInspector] public Slider cooldownSlider;

    public bool IsCooldownActive => currentCooldownTime > 0;

    // Called in GameManager
    public void Start()
    {
        CooldownCanvas = Instantiate(CooldownCanvasPrefab);
        cooldownSlider = CooldownCanvas.GetComponentInChildren<Slider>();

        cooldownSlider.gameObject.SetActive(false);

        UpdateSlider();
    }

    void Update()
    {
        if (IsCooldownActive)
        {
            currentCooldownTime -= Time.deltaTime;
            if(currentCooldownTime <= 0)
            {
                StopCooldown();
                return;
            }

            UpdateSlider();
        }
    }

    public void StartCooldown()
    {
        currentCooldownTime = cooldownTime;
        UpdateSlider();
    }

    public void StopCooldown()
    {
        currentCooldownTime = 0;
        OnCooldownFinished?.Invoke();
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (cooldownSlider==null)
            return;
        
        if (IsCooldownActive)
        {
            cooldownSlider.gameObject.SetActive(true);
            cooldownSlider.value = currentCooldownTime / cooldownTime;
        }
        else
        {
            cooldownSlider.gameObject.SetActive(false);
        }
    }
}
