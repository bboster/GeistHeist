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
    private CollectableRegistry Registry;

    [Header("Debug")]
    [SerializeField, OnValueChanged(nameof(UpdateVisibility))] private bool DebugAlwaysDisplay;


    public void Interact()
    {
        if (SaveDataManager.Instance.EquipedHat() == (int)ThisCollectable)
        {
            Debug.Log($"{ThisCollectable} is already equipped.");
            return;
        }

        else
        {
            GameObject meshPrefab = Registry.GetMesh(ThisCollectable);
            if (meshPrefab != null)
            {
                Debug.Log($"Attempting to wear: {ThisCollectable}");

                // Find an instance of WearableCollectible in the scene
                WearableCollectible wearableCollectible = FindAnyObjectByType<WearableCollectible>();
                if (wearableCollectible != null)
                {
                    wearableCollectible.EquipHat(ThisCollectable);
                    SaveDataManager.Instance.MarkCollectableAsWorn(ThisCollectable);

                    Debug.Log($"{ThisCollectable} equipped successfully!");

                    foreach (Transform child in this.transform)
                        Destroy(child.gameObject);
                }
                else
                {
                    Debug.LogWarning("No WearableCollectible instance found in the scene.");
                }
            }
            else
            {
                Debug.LogWarning($"No prefab found in Registry for {ThisCollectable}.");
            }
        }
    }

    public void spawnMesh(Collectable collectableToRespawn)
    {
        // Only respawn if this OptionalCollectable matches the collectible
        if (ThisCollectable != collectableToRespawn)
            return;
        GameObject meshPrefab = Registry.GetMesh(ThisCollectable);
        // Clear any existing children (optional)
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        if (meshPrefab == null)
        {
            Debug.LogWarning($"[{name}] No CollectableModel to respawn for {ThisCollectable}");
            return;
        }

        // Instantiate the mesh
        GameObject meshInstance = Instantiate(meshPrefab, transform);
        meshInstance.transform.localPosition = Vector3.zero;
        meshInstance.transform.localRotation = Quaternion.identity;
    }


    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Registry == null)
            Registry = Resources.Load<CollectableRegistry>("CollectableRegistry");
#endif
    }

    private void Awake()
    {
        if (Registry == null)
            Registry = Resources.Load<CollectableRegistry>("CollectableRegistry");
    }

    public void Start()
    {
        spawnMesh(ThisCollectable);
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        gameObject.SetActive(DebugAlwaysDisplay || SaveDataManager.Instance.IsCollectableCollected(ThisCollectable));

    }

    #region Debug Tools
#if UNITY_EDITOR

    [Button("Preview Collectable")]
    private void PreviewHat()
    {
        if (Registry == null)
        {
            Registry = Resources.Load<CollectableRegistry>("CollectableRegistry");
            if (Registry == null)
            {
                Debug.LogWarning("Registry not found.");
                return;
            }
        }
        spawnMesh(ThisCollectable);
    }
#endif
    #endregion
}