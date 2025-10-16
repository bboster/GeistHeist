/*
 * Contributors:  Toby
 * Creation Date: 10/9/25
 * Last Modified: 10/9/25
 * 
 * Brief Description: Manages Billboard UI objects.
 * Put this script on a canvas
 * Billboard ui objects do the following:
 * - stays in a single world point, 
 * - remains a consistent size, 
 * - always faces the player
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class BillboardUIManager : Singleton<BillboardUIManager>
{
    // NOT a dictionary because there could maybe be multiple ui elements at one anchor point
    //                    World Point, UI object
    private HashSet<Tuple<Transform, IBillboardUI>> billboardUIPoints = new();

    private Camera _camera;
    private Canvas billboardUICanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        billboardUICanvas = GetComponent<Canvas>() ;
        _camera = Camera.main;
    }

    // Set the worldspace -> UI position of each billboard 
    void Update()
    {
        foreach(var uiAnchorPair in billboardUIPoints)
        {
            var elemRectTransform = uiAnchorPair.Item2.rectTransform;
            var anchor = uiAnchorPair.Item1;

            Vector3 screenPos = _camera.WorldToScreenPoint(anchor.position);
            Vector3 uiPos = new Vector3(screenPos.x, Screen.height - screenPos.y, screenPos.z);

            elemRectTransform.position = uiPos;
        }
    }

    public void RegisterAndInitializeBillboardUIPoint(Transform worldPoint, IBillboardUI UIElement, GameObject SourceGameObject, bool HideByDefault)
    {
        if (billboardUIPoints.Select(b=>b.Item1).Contains(worldPoint))
        {
            Debug.LogWarning($"Two billboard ui elements are initialized for point: {worldPoint.name}.");
            // Dont return tho... lets see where this goes.
        }
        
        UIElement.rectTransform.SetParent(billboardUICanvas.transform);

        billboardUIPoints.Add(new Tuple<Transform, IBillboardUI>(worldPoint, UIElement));

        UIElement.OnInitialize(SourceGameObject);
    }
}