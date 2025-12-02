/*
 * Contributors: Toby Schamberger
 * Creation: 11/6/25
 * Last Edited: 11/6/25
 * Summary: Change the color of a font text when user hover overs a selectable (button).
 * Designed for vertical gradients rn. Can be updated to do multiple colors with an enum.
 */

using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButtonChangeFontColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, Required] private TMP_Text targetText;

    [Header("Unhovered color")]
    [SerializeField] private Color unhoveredTopColor = Color.white;
    [SerializeField] private Color unhoveredBottomColor = Color.white;

    [Header("Hovered color")]
    [SerializeField] private Color hoveredTopColor = Color.white;
    [SerializeField] private Color hoveredBottomColor = Color.white;

    void Start()
    {
        OnPointerExit(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        VertexGradient vg;
        vg.topLeft = unhoveredTopColor;
        vg.topRight = unhoveredTopColor;

        vg.bottomLeft = unhoveredBottomColor;
        vg.bottomRight = unhoveredBottomColor;
        
        targetText.colorGradient = vg;
    }

    void OnPointerEnter(PointerEventData data)
    {
        VertexGradient vg;
        vg.topLeft = hoveredTopColor;
        vg.topRight = hoveredTopColor;

        vg.bottomLeft = hoveredBottomColor;
        vg.bottomRight = hoveredBottomColor;

        targetText.colorGradient = vg;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter(eventData);
    }
}
