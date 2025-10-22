/*
 * Contributors: Toby, Josh
 * Creation: 10/21/25
 * Last Edited: 10/21/25
 * Summary: Mirrors a collectable object, appears in the hub if the save data has marked the collectable as got.
 * 
 * TODO: hat code >:)
 */

using NaughtyAttributes;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class OptionalCollectableHubDisplay : MonoBehaviour, IInteractable
{
    [InfoBox("Set this gameobject to active by default, it will be disabled based on whether or not the player has save data for the relevant collectable")]

    [InfoBox("New collectable enums can be added from an object with the OptionalCollectable script")]

    [SerializeField] private Collectable ThisCollectable;
    [SerializeField] private CollectableRegistry registry;

    [Header("Debug")]
    [SerializeField, OnValueChanged(nameof(UpdateVisibility))] private bool DebugAlwaysDisplay;
    PossessableObject player;
    Transform wearable;

    public void Interact()
    {
        GameObject meshPrefab = registry.GetMesh(ThisCollectable);
        if (meshPrefab != null)
        {
            Debug.Log($"Attempting to wear: {ThisCollectable}");

            // Remove any previously equipped hat(s)
            foreach (Transform child in wearable)
            {
                Destroy(child.gameObject);
            }

            // Instantiate the new hat
            GameObject hat = Instantiate(meshPrefab, wearable);
            hat.transform.localPosition = Vector3.zero;
            hat.transform.localRotation = Quaternion.identity;
            hat.transform.localScale = Vector3.one; // ensures correct size

            SaveDataManager.Instance.MarkCollectableAsWorn(ThisCollectable);

            Debug.Log($"{ThisCollectable} equipped successfully!");
        }
        else
        {
            Debug.LogWarning($"No prefab found in registry for {ThisCollectable}.");
        }
    }



    /*
     * maybe figure this out later
    private void Awake()
    {
        GameObject meshPrefab = registry.GetMesh(ThisCollectable);
        if (meshPrefab != null)
        {
            Instantiate(meshPrefab, transform);
        }
    }
    */
    public void Start()
    {
        // TODO: I think it might be better if the collectables model/mesh was pulled from some kind of table/dictionary?
        //       I only say that because of the way the player is going to need to switch out hats
        player = PlayerManager.Instance.PlayerGhostObject;
        wearable = player.transform.Find("Wearable");
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        gameObject.SetActive(DebugAlwaysDisplay || SaveDataManager.Instance.IsCollectableCollected(ThisCollectable));

    }
}