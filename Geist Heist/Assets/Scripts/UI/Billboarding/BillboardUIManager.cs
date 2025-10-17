/*
 * Contributors:  Toby
 * Creation Date: 10/9/25
 * Last Modified: 10/16/25
 * 
 * Brief Description: Manages Billboard UI objects.
 * Put this script on a canvas
 * Billboard ui objects do the following:
 * - stays in/ follows a single world point, 
 * - changes scale and opacity based on player proximity
 * - always faces the player
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class BillboardUIManager : Singleton<BillboardUIManager>
{
    [Tooltip("If false, calculates by player position. If true, calculates by camera position.")]
    [SerializeField] bool CalculateScalingByCameraPosition = false;

    // NOT a dictionary because there could maybe be multiple ui elements at one anchor point
    //                    World Point, UI object
    private HashSet<Tuple<Transform, IBillboardUI>> billboardUIPoints = new();

    private Camera _camera;
    private Canvas billboardUICanvas;

    /// <summary>
    /// Called in GameManager, close to awake
    /// </summary>
    public void Initialize()
    {
        billboardUICanvas = GetComponent<Canvas>() ;
        _camera = Camera.main;
    }

    // Set the worldspace -> UI position of each billboard 
    void Update()
    {
        foreach(var uiAnchorPair in billboardUIPoints)
        {
            if(uiAnchorPair.Item2.IsVisible == false)
                continue;

            var elem = uiAnchorPair.Item2;
            var elemRectTransform = elem.rectTransform;
            var elemTransform = elem.transform;
            var anchor = uiAnchorPair.Item1;

            float playerDistance = CalculateScalingByCameraPosition ? 
                Vector3.Distance(anchor.position, _camera.transform.position) :
                Vector3.Distance(anchor.position, PlayerManager.Instance.CurrentObject.transform.position);
            float t;

            // Set Position
            elemTransform.position = anchor.position;

            // Set opacity
            Vector3 screenPos = _camera.WorldToScreenPoint(anchor.position);
            Vector3 uiPos = new Vector3(screenPos.x, /*Screen.height - */screenPos.y, screenPos.z);

            elem.CalculateAndSetOpacity(playerDistance, uiPos);
            //if (elem.CurrentAlpha == 0)
            //    continue; // dont bother with anything else if we dont need to.

            // Face camera
            if (elem.MirrorBillboard)
                elemTransform.LookAway(_camera.transform);
            else
                elemTransform.LookAt(_camera.transform);

            // Set scale
            elem.CalculateAndSetScale(playerDistance);
        }
    }


    public void RegisterAndInitializeBillboardUIPoint(Transform worldPoint, IBillboardUI UIElement, GameObject SourceGameObject)
    {
        if (billboardUIPoints.Select(b=>b.Item1).Contains(worldPoint))
        {
            Debug.LogWarning($"Two billboard ui elements are initialized for point: {worldPoint.name}.");
            // Dont return tho... lets see where this goes.
        }
        
        UIElement.rectTransform.SetParent(billboardUICanvas.transform);

        billboardUIPoints.Add(new Tuple<Transform, IBillboardUI>(worldPoint, UIElement));

        UIElement.OnInitialize(SourceGameObject);
        UIElement.ToggleVisibility(!UIElement.HideByDefault);
    }
}