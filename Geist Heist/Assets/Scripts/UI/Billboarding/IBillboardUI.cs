/*
 * Contributors:  Toby
 * Creation Date: 10/9/25
 * Last Modified: 10/9/25
 * 
 * Brief Description: Interface for Billboard UI objects.
 * Billboard ui objects do the following:
 * - stays in a single world point, 
 * - remains a consistent size, 
 * - always faces the player
 */

using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public abstract class IBillboardUI : MonoBehaviour
{
    public CanvasGroup canvasGroup => GetCanvasGroup();
    public RectTransform rectTransform  => GetRectTransform();

    #region Getters Setters

    private CanvasGroup _canvasGroup;
    private CanvasGroup GetCanvasGroup()
    {
        _canvasGroup = _canvasGroup == null ? GetComponent<CanvasGroup> () : _canvasGroup;
        return _canvasGroup;
    }

    private RectTransform _rectTransform;
    private RectTransform GetRectTransform()
    {
        _rectTransform = _rectTransform == null ? GetComponent<RectTransform>() : _rectTransform;
        return _rectTransform;
    }

    #endregion

    // sourceGameObject is the object that contains the data for the billboard UI.    
    // For example, if this UI element is a health bar for an enemy, sourceGameObject would be an enemy.
    // It is expected that you would use GetComponent to get the data that you need.
    public abstract void OnInitialize(GameObject sourceGameObject);


    public virtual void ToggleVisibility(bool isVisible)
    {
        if (isVisible) Show();
        else  Hide();
    }
    public virtual void Hide()
    {
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

    public virtual void Show()
    {
        StaticUtilities.EnableCanvasGroup(canvasGroup);
    }
}
