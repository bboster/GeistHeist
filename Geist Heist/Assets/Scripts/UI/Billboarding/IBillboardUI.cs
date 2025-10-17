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
using TMPro;
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public abstract class IBillboardUI : MonoBehaviour
{
    [Header("Billboard Settings")]
    [SerializeField] public bool MirrorBillboard = true;

    [Tooltip("If true, hides this ui element at start")]
    [SerializeField] public bool HideByDefault = false;

    // lots of variables and attributes and theyre all important

    // Scale size by player distance
    [Tooltip("Distance for camera to be when the UI object will be its largest")]
    [SerializeField, Foldout("Scale by proximity")] private float closeScaleDistance = 15;
    [SerializeField, Foldout("Scale by proximity")] private float closeScale = 1f;
    [Tooltip("Distance for camera to be when the UI object will be its smallest")]
    [SerializeField, Foldout("Scale by proximity")] private float farScaleDistance = 20;
    [SerializeField, Foldout("Scale by proximity")] private float farScale = 0.9f;

    // Scale Opacity by position on screen
    [SerializeField, Foldout("Alpha by proximity")] private bool fadeInCorners = true;
    [Tooltip("If this element's x position on the screen is within the outer [X] percent of the screen, it will start to become semi-transparent")]
    [SerializeField, Range(0, 0.5f), Foldout("Alpha by proximity"), ShowIf(nameof(fadeInCorners))] private float xEdgePercentToFade = 0.15f;
    [Tooltip("If this element's y position on the screen is within the outer [X] percent of the screen, it will start to become semi-transparent")]
    [SerializeField, Range(0, 0.5f), Foldout("Alpha by proximity"), ShowIf(nameof(fadeInCorners))] private float yEdgePercentToFade = 0.10f;

    // Smooth opacity by player distance
    [Tooltip("Distance for camera to be when the UI object will be its largest")]
    [SerializeField, Foldout("Alpha by proximity")] private float closeOpacityDistance = 20;
    [Tooltip("The UI Objects max opacity")]
    [SerializeField, Range(0, 1), Foldout("Alpha by proximity")] private float closeOpacity = 1f;
    [Tooltip("Distance for camera to be when the UI object will be its smallest")]
    [SerializeField, Foldout("Alpha by proximity")] private float farOpacityDistance = 22;
    [Tooltip("The UI Objects min opacity")]
    [SerializeField, Range(0, 1), Foldout("Alpha by proximity")] private float farOpacity = 0f;

    public CanvasGroup canvasGroup => GetCanvasGroup();
    public RectTransform rectTransform  => GetRectTransform();

    [HideInInspector] public bool IsVisible = true;
    [HideInInspector] public float CurrentAlpha = 1;

    // My magic number
    private const float SMOOTH_SPEED = 10;

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
        IsVisible = false;
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

    public virtual void Show()
    {
        IsVisible = true;
        StaticUtilities.EnableCanvasGroup(canvasGroup, alpha: CurrentAlpha);
    }
    #endregion

    // Can be overridden for custom behavior
    public virtual void CalculateAndSetScale(float playerDistance)
    {
        float t = Mathf.InverseLerp(closeScaleDistance, farScaleDistance, playerDistance);
        transform.localScale = Vector3.one * Mathf.Lerp(closeScale, farScale, t);
    }

    // Can be overridden for custom behavior
    /// <param name="UIPosition">The position of this UI element on the screen</param>
    public virtual void CalculateAndSetOpacity(float playerDistance, Vector3 UIPosition)
    {
        float t = Mathf.InverseLerp(closeScaleDistance, farScaleDistance, playerDistance);
        float baseAlpha = Mathf.Lerp(closeOpacity, farOpacity, t);

        // check if baseAlpha is 0 to avoid all the bs happening down there
        if (!fadeInCorners || baseAlpha == 0)
        {
            CurrentAlpha = Mathf.Lerp(CurrentAlpha, baseAlpha, Time.deltaTime * SMOOTH_SPEED);
            canvasGroup.alpha = baseAlpha;
            return;
        }

        float x_percent = UIPosition.x / Screen.width; // 0 (left) -> 1 (right)
        float y_percent = UIPosition.y / Screen.height;// 0 (top) -> 1 (bottom)4

        // 0 (edge of screen) -> 0.5 (center of screen)
        float x_edge_proximity = x_percent > 0.5f ? 1 - x_percent : x_percent; 
        float y_edge_proximity = y_percent > 0.5f ? 1 - y_percent : y_percent;

        // 0 (within fade radius. Hide completely) -> 1 (not within fade radius)
        float x_edge_percent = Mathf.InverseLerp(0, xEdgePercentToFade, x_edge_proximity);
        float y_edge_percent = Mathf.InverseLerp(0, yEdgePercentToFade, y_edge_proximity);

        // Min between both axes
        float min_edge_percent = Mathf.Min(x_edge_percent, y_edge_percent);

        // Debug
        //GetComponent<TextMeshProUGUI>().text = min_edge_percent.ToString();

        CurrentAlpha = Mathf.Lerp(CurrentAlpha, Mathf.Min(baseAlpha, min_edge_percent), Time.deltaTime * SMOOTH_SPEED);
        canvasGroup.alpha = CurrentAlpha;
    }
}
