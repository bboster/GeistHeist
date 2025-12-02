/*
 * Contributors:  Toby
 * Creation Date: 10/9/2025
 * Last Modified: 12/2/2025
 * 
 * Brief Description: Manages Billboard UI objects.
 * Put this script on a canvas
 * Billboard ui objects do the following:
 * - stays in/ follows a single world point, 
 * - changes scale and opacity based on player proximity
 * - always faces the player
 */

using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class BillboardUIManager : Singleton<BillboardUIManager>
{
    [Tooltip("If false, calculates by player position. If true, calculates by camera position.")]
    [SerializeField] bool CalculateScalingByCameraPosition = false;

    [SerializeField, Required] private GameObject onomatopoeiaPrefab;
    [SerializeField, Required] private GameObject onomatopoeiaPointPrefab;

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

        SpawnOnomatopoeia("test", PlayerManager.Instance.CurrentObject.transform.position, lifetime:10);
    }

    // Set the worldspace -> UI position of each billboard 
    void Update()
    {
        foreach (var uiAnchorPair in billboardUIPoints)
        {
            if (uiAnchorPair == null || uiAnchorPair.Item1 == null || uiAnchorPair.Item2 == null)
            {
                Debug.Log("removing null billboard ui element");
                billboardUIPoints.Remove(uiAnchorPair);

                // foreach loop freaks out when you remove stuff during it. so we just return here.
                // depending on where we are in the foreach loop, this may mean some billboard ui points literally just dont get updated.
                // but i think thats okay bc this loop runs every frame
                return; 
            }

            if (uiAnchorPair.Item2.IsVisible == false)
                continue;

            if (PlayerManager.Instance.CurrentObject == null)
                continue;

            var elem = uiAnchorPair.Item2;
            var elemRectTransform = elem.rectTransform;
            var elemTransform = elem.transform;
            var anchor = uiAnchorPair.Item1;

            float playerDistance = CalculateScalingByCameraPosition ? 
                Vector3.Distance(anchor.position, _camera.transform.position) :
                Vector3.Distance(anchor.position, PlayerManager.Instance.CurrentObject.transform.position);

            // Set Position
            elemTransform.position = anchor.position;

            // Set opacity
            Vector3 screenPos = _camera.WorldToScreenPoint(anchor.position);
            Vector3 uiPos = new Vector3(screenPos.x, /*Screen.height - */screenPos.y, screenPos.z);

            elem.CalculateAndSetOpacity(playerDistance, uiPos);
            if (elem.CurrentAlpha == 0)
                continue; // dont bother with anything else if we dont need to.

            // Face camera
            if (elem.MirrorBillboard)
                elemTransform.LookAway(_camera.transform);
            else
                elemTransform.LookAt(_camera.transform);

            // Set scale
            elem.CalculateAndSetScale(playerDistance);
        }
    }


    public Tuple<Transform, IBillboardUI> RegisterAndInitializeBillboardUIPoint(Transform worldPoint, IBillboardUI UIElement, GameObject SourceGameObject)
    {
        BillboardUIPoint billboardPoint = worldPoint.GetComponent<BillboardUIPoint>();
        if(billboardPoint != null)
            billboardPoint.billboardUI = UIElement;

        if (billboardUIPoints.Select(b=>b.Item1).Contains(worldPoint))
        {
            Debug.LogWarning($"Two billboard ui elements are initialized for point: {worldPoint.name}.");
            // Dont return tho... lets see where this goes.
        }
        
        UIElement.rectTransform.SetParent(billboardUICanvas.transform);

        var pair = new Tuple<Transform, IBillboardUI>(worldPoint, UIElement);
        billboardUIPoints.Add(pair);

        UIElement.OnInitialize(SourceGameObject);
        UIElement.ToggleVisibility(!UIElement.HideByDefault);

        return pair;
    }

    /// <summary>
    /// Spawns Onomatopoeia text at set position.
    /// </summary>
    /// <param name="randomRotationRange">Degrees that the Onomatopoeia can by randomly rotated by</param>
    /// <returns>Transform that the Onomatopoeia will be "childed" to.</returns>
    public Transform SpawnOnomatopoeia(string text, Vector3 worldPosition, 
                                       float lifetime = 1.5f, float scale = 1, float randomRotationRange=0,
                                       bool bold = true, bool italics = false)
    {
        // TODO: these could be object pooled (but tbh i dont think our games performance is that bad so im not going to bother)
        var point = Instantiate(onomatopoeiaPointPrefab, worldPosition, Quaternion.identity);
        var onomatopoeiaBillboard = Instantiate(onomatopoeiaPrefab).GetComponent<OnomatopoeiaBillboardUI>();

        onomatopoeiaBillboard.SetTextProperties(text, scale, randomRotationRange, bold, italics);

        var pair = RegisterAndInitializeBillboardUIPoint(point.transform, onomatopoeiaBillboard, null);
        StartCoroutine(DestroyBillboardAfterSeconds(pair, lifetime));

        return point.transform;
    }

    private IEnumerator DestroyBillboardAfterSeconds(Tuple<Transform, IBillboardUI> pointAndUI, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        billboardUIPoints.Remove(pointAndUI);

        Destroy(pointAndUI.Item1.gameObject);
        Destroy(pointAndUI.Item2.gameObject);

        // todo: make it fade out probably lol
    }
}