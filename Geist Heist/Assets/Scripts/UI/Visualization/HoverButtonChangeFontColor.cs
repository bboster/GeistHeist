/*
 * Contributors: Toby Schamberger
 * Creation: 11/6/25
 * Last Edited: 11/20/25
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
        OnPointerExit();
    }

    public void OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();


    public void OnPointerExit()
    {
        VertexGradient vg;
        vg.topLeft = unhoveredTopColor;
        vg.topRight = unhoveredTopColor;

        vg.bottomLeft = unhoveredBottomColor;
        vg.bottomRight = unhoveredBottomColor;
        
        targetText.colorGradient = vg;
    }

    void OnPointerEnter()
    {
        VertexGradient vg;
        vg.topLeft = hoveredTopColor;
        vg.topRight = hoveredTopColor;

        vg.bottomLeft = hoveredBottomColor;
        vg.bottomRight = hoveredBottomColor;

        targetText.colorGradient = vg;
    }

    [Button]
    void PreviewUnhoveredState()
    {
        OnPointerExit(null);
    }

    [Button]
    void PreviewHoveredState()
    {
        OnPointerEnter(null);
    }

}
