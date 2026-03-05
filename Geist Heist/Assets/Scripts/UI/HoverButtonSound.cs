/*
 * Contributors: Toby. Josh
 * Creation: 12/2/2025
 * Last Edited: 12/2/25
 * 
 * Description: Plays a sound when user hovers over a button with this script on it.
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButtonSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public void OnPointerEnter(PointerEventData eventData) => PlayHover();
    public void OnSelect(BaseEventData eventData) => PlayHover();
    public void PlayHover()
    {
        // tysm Toby

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIHover);
    }
}
