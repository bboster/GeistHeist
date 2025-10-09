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
    [SerializeField] private Canvas billboardUICanvas;

    //          World Point, UI object
    HashSet<Tuple<Transform, RectTransform>> billboardUIPoints = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterBillboardUIPoint(Transform worldPoint, RectTransform UIElement)
    {
        if (billboardUIPoints.Select(b=>b.Item1).Contains(worldPoint))
        {
            Debug.LogWarning($"Two billboard ui elements are initialized for point: {worldPoint.name}.");
            // Dont return tho... lets see where this goes.
        }

        UIElement.SetParent(billboardUICanvas.transform);

        billboardUIPoints.Add(new Tuple<Transform, RectTransform>(worldPoint, UIElement));
    }
}