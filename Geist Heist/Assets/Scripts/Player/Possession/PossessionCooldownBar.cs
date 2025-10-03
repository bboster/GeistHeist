using UnityEngine;
using UnityEngine.UI;

public class PossessionCooldownBar : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 5f;
    private float currentCooldownTime;
    private Slider cooldownSlider;
    private bool isCooldownActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cooldownSlider = gameObject.GetComponent<Slider>();
        currentCooldownTime = cooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCooldownActive)
        {
            currentCooldownTime -= Time.deltaTime;
            cooldownSlider.value = currentCooldownTime / cooldownTime;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            isCooldownActive = true;
        }
    }
}
