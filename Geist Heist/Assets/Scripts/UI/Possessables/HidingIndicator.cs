/*
 * Contributors: Toby
 * Creation Date: 10/29/2025
 * Last Modified: 10/29/2025
 * 
 * Opens/closes the an icon based on how much time player has left in possessable. Intended for the vase.
 * For t: 0 means eye open (image shown). 1 means eye closed (image hidden).
 * 
 * TODO: it would be cool if it started pulsing when time is almost up
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class HidingIndicator : MonoBehaviour
{
    [SerializeField, Required] private Image hidingImage;
    [SerializeField, Required] private PossessableObject possessable;


    public void Start()
    {
        hidingImage.fillAmount = 0;
        possessable.OnTimerUpdate.AddListener(OnTimerUpdate);
    }

    private void OnTimerUpdate(float percentage)
    {
        float t = percentage / possessable.maxChargePercentage;
        hidingImage.fillAmount = 1 - t;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
            return;

        float t = (Time.time / 4) % 1;
        hidingImage.fillAmount = 1-t;
    }
}
