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

    [Tooltip("If true, hides this ui element at start")]
    [SerializeField] public bool HideByDefault = false;

    // lots of variables and attributes and theyre all important

    // Scale size by player distance
    [Tooltip("Distance for camera to be when the UI object will be its largest")]
    [SerializeField, Foldout("Scale by player proximity")] private float closeScaleDistance = 15;
    [SerializeField, Foldout("Scale by player proximity")] private float closeScale = 1f;
    [Tooltip("Distance for camera to be when the UI object will be its smallest")]
    [SerializeField, Foldout("Scale by player proximity")] private float farScaleDistance = 20;
    [SerializeField, Foldout("Scale by player proximity")] private float farScale = 0.9f;

    // Scale Opacity by position on screen
    [SerializeField, Foldout("Opacity by screen position")] private bool fadeInCorners = true;
    [Tooltip("If this element's x position on the screen is within the outer [X] percent of the screen, it will start to become semi-transparent")]
    [SerializeField, Range(0, 0.5f), Foldout("Opacity by screen position"), ShowIf(nameof(fadeInCorners))] private float closeXEdgePercentToFade = 0.05f;
    [Tooltip("If this element's x position on the screen is within the outer [X] percent of the screen, it will start to become semi-transparent")]
    [SerializeField, Range(0, 0.5f), Foldout("Opacity by screen position"), ShowIf(nameof(fadeInCorners))] private float farXEdgePercentToFade = 0.15f;

    [Tooltip("If this element's y position on the screen is within the outer [X] percent of the screen, it will start to become semi-transparent")]
    [SerializeField, Range(0, 0.5f), Foldout("Opacity by screen position"), ShowIf(nameof(fadeInCorners))] private float closeYEdgePercentToFade = 0.02f;
    [Tooltip("If this element's y position on the screen is within the outer [X] percent of the screen, it will start to become semi-transparent")]
    [SerializeField, Range(0, 0.5f), Foldout("Opacity by screen position"), ShowIf(nameof(fadeInCorners))] private float farYEdgePercentToFade = 0.10f;

    // Smooth opacity by player distance
    [SerializeField, Foldout("Opacity by player proximity")] 
    private bool opacityByPlayerDistance = true;
    [Tooltip("Distance for camera to be when the UI object will be its largest")]
    [SerializeField, Foldout("Opacity by player proximity"), ShowIf(nameof(opacityByPlayerDistance))] 
    private float closeOpacityDistance = 20;
    [Tooltip("The UI Objects max opacity")]
    [SerializeField, Range(0, 1), Foldout("Opacity by player proximity"), ShowIf(nameof(opacityByPlayerDistance))] 
    private float closeOpacity = 1f;
    [Tooltip("Distance for camera to be when the UI object will be its smallest")]
    [SerializeField, Foldout("Opacity by player proximity"), ShowIf(nameof(opacityByPlayerDistance))] 
    private float farOpacityDistance = 22;
    [Tooltip("The UI Objects min opacity")]
    [SerializeField, Range(0, 1), Foldout("Opacity by player proximity"), ShowIf(nameof(opacityByPlayerDistance))] 
    private float farOpacity = 0f;

    // Smooth opacity by main camera distance
    [SerializeField, Foldout("Opacity by camera proximity")]
    private bool opacityByCameraDistance = false;
    [Tooltip("Distance for camera to be when the UI object will be its largest")]
    [SerializeField, Foldout("Opacity by camera proximity"), ShowIf(nameof(opacityByCameraDistance))]
    private float closeCameraOpacityDistance = 20;
    [Tooltip("The UI Objects max opacity")]
    [SerializeField, Range(0, 1), Foldout("Opacity by camera proximity"), ShowIf(nameof(opacityByCameraDistance))]
    private float closeCameraOpacity = 1f;
    [Tooltip("Distance for camera to be when the UI object will be its smallest")]
    [SerializeField, Foldout("Opacity by camera proximity"), ShowIf(nameof(opacityByCameraDistance))]
    private float farCameraOpacityDistance = 22;
    [Tooltip("The UI Objects min opacity")]
    [SerializeField, Range(0, 1), Foldout("Opacity by camera proximity"), ShowIf(nameof(opacityByCameraDistance))]
    private float farCameraOpacity = 0f;

    public CanvasGroup canvasGroup => GetCanvasGroup();
    public RectTransform rectTransform  => GetRectTransform();

    [HideInInspector] public bool IsVisible = true;
    [HideInInspector] public float CurrentAlpha = 1;

    // My magic number
    private const float SMOOTH_SPEED =1;

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
    public void CalculateAndSetOpacity(float playerDistance, float cameraDistance, Vector3 UIPosition)
    {
        float a = CalculateOpacity(playerDistance, cameraDistance, UIPosition);
        // Smooth it
        CurrentAlpha = Mathf.MoveTowards(CurrentAlpha, a, Time.deltaTime * SMOOTH_SPEED);
        canvasGroup.alpha = CurrentAlpha;
    }

    protected virtual float CalculateOpacity(float playerDistance, float cameraDistance, Vector3 UIPosition)
    {
        float player_t = Mathf.InverseLerp(closeOpacityDistance, farOpacityDistance, playerDistance);
        float player_alpha = opacityByPlayerDistance ?
            Mathf.Lerp(closeOpacity, farOpacity, player_t) : 
            1;

        float camera_t = Mathf.InverseLerp(closeCameraOpacityDistance, farCameraOpacityDistance, playerDistance) ;
        float camera_alpha = opacityByCameraDistance ?
            Mathf.Lerp(closeCameraOpacity, farCameraOpacity, camera_t) :
            1;

        float baseAlpha = Mathf.Min(player_alpha, camera_alpha);

        // check if baseAlpha is 0 to avoid all the bs happening down there
        if (!fadeInCorners || baseAlpha == 0)
        {
            return baseAlpha;
        }

        float x_percent = UIPosition.x / Screen.width; // 0 (left) -> 1 (right)
        float y_percent = UIPosition.y / Screen.height;// 0 (top) -> 1 (bottom)4

        // 0 (edge of screen) -> 0.5 (center of screen)
        float x_edge_proximity = x_percent > 0.5f ? 1 - x_percent : x_percent;
        float y_edge_proximity = y_percent > 0.5f ? 1 - y_percent : y_percent;

        // 0 (within fade radius. Hide completely) -> 1 (not within fade radius)
        float x_edge_percent = Mathf.InverseLerp(closeXEdgePercentToFade, farXEdgePercentToFade, x_edge_proximity);
        float y_edge_percent = Mathf.InverseLerp(closeYEdgePercentToFade, farYEdgePercentToFade, y_edge_proximity);

        // Min between both axes
        float min_edge_percent = Mathf.Min(x_edge_percent, y_edge_percent);

        // Set opacity
        return Mathf.Min(baseAlpha, min_edge_percent);
       
    }
}
