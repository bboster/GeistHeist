/*
 * Contributors: Toby
 * Creation: 12/2/2025
 * Last Edited: 12/2/25
 * 
 * Description: Plays a sound when user hovers over a button with this script on it.
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class HoverButtonSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // tysm Toby

        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIHover);
    }
}
