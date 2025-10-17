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

using NaughtyAttributes;
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public abstract class IBillboardUI : MonoBehaviour
{
    [Header("Billboard Settings")]
    [SerializeField] public bool MirrorBillboard = true;

    [SerializeField, Foldout("Scale by proximity")] private float minScale = 0.7f;
    [SerializeField, Foldout("Scale by proximity")] private float maxScale = 1f;
    [Tooltip("Distance for camera to be when the UI object will be its smallest")]
    [SerializeField, Foldout("Scale by proximity")] private float minScaleDistance = 15;
    [Tooltip("Distance for camera to be when the UI object will be its largest")]
    [SerializeField, Foldout("Scale by proximity")] private float maxScaleDistance = 7;

    [SerializeField, Range(0, 1), Foldout("Alpha by proximity")] private float minOpacity = 0f;
    [SerializeField, Range(0, 1), Foldout("Alpha by proximity")] private float maxOpacity = 1f;
    [Tooltip("Distance for camera to be when the UI object will be its smallest")]
    [SerializeField, Foldout("Alpha by proximity")] private float minOpacityDistance = 15;
    [Tooltip("Distance for camera to be when the UI object will be its largest")]
    [SerializeField, Foldout("Alpha by proximity")] private float maxOpacityDistance = 7.5f;

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

    #region Visibility
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
    #endregion

    public virtual void SetScale(float playerDistance)
    {
        float t = Mathf.InverseLerp(maxScaleDistance, minScaleDistance, playerDistance);
        transform.localScale = Vector3.one * t;
    }
}
