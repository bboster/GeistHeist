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
    private Slider cooldownSlider;
    private float currentCooldownTime;
    private bool isCooldownActive = false;

    [HideInInspector] public GameObject CooldownCanvas;
    public bool IsCooldownActive => currentCooldownTime > 0;

    // Called in GameManager
    public void Start()
    {
        CooldownCanvas = Instantiate(CooldownCanvasPrefab);
        cooldownSlider = CooldownCanvas.GetComponentInChildren<Slider>();

        CooldownCanvas.SetActive(false);
    }

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
}
