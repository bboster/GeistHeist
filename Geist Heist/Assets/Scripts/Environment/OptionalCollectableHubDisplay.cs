/*
 * Contributors: Toby, Josh
 * Creation: 10/21/25
 * Last Edited: 10/27/25
 * Summary: Mirrors a collectable object, appears in the hub if the save data has marked the collectable as got.
 * 
 */

using NaughtyAttributes;
using System;
using UnityEditor;  
using UnityEngine;

public class OptionalCollectableHubDisplay : MonoBehaviour, IInteractable
{
    [InfoBox("Set this gameobject to active by default, it will be disabled based on whether or not the player has save data for the relevant collectable")]
    [InfoBox("New collectable enums can be added from an object with the OptionalCollectable script")]

    [SerializeField] private Collectable ThisCollectable;
    private CollectableRegistry Registry;

    [Header("Debug")]
    [SerializeField, OnValueChanged(nameof(UpdateVisibility))] private bool DebugAlwaysDisplay;

    #region Unity Lifecycle
    private void OnValidate()
    {
#if UNITY_EDITOR
        LoadRegistry();
#endif
    }

    private void Awake() => LoadRegistry();

    private void Start()
    {
        spawnMesh(ThisCollectable);
        UpdateVisibility();
    }
    #endregion


    #region Interaction
    void IInteractable.Interact()
    {
        if (SaveDataManager.Instance.EquipedHat() == (int)ThisCollectable)
        {
            Debug.Log($"{ThisCollectable} is already equipped.");
            return;
        }

        MeshRenderer meshPrefab = Registry.GetMesh(ThisCollectable);
        if (meshPrefab == null)
        {
            Debug.LogWarning($"No prefab found in Registry for {ThisCollectable}.");
            return;
        }

        Debug.Log($"Attempting to wear: {ThisCollectable}");

        /*
         * Note: This should be cached, to avoid calling FindAnyObjectByType multiple times.
         * -Toby
         */
        WearableCollectible wearableCollectible = FindAnyObjectByType<WearableCollectible>();
        if (wearableCollectible == null)
        {
            Debug.LogWarning("No WearableCollectible instance found in the scene.");
            return;
        }

        wearableCollectible.EquipHat(ThisCollectable);
        SaveDataManager.Instance.MarkCollectableAsWorn(ThisCollectable);

        Debug.Log($"{ThisCollectable} equipped successfully!");

        // Hide this mesh to indicate it's now equipped
        MeshRenderer existingMesh = GetComponentInChildren<MeshRenderer>();
        if (existingMesh != null)
            existingMesh.enabled = false;
    }
    #endregion


    #region Mesh Spawning
    public void spawnMesh(Collectable collectableToRespawn)
    {
        if (ThisCollectable != collectableToRespawn)
            return;

        MeshRenderer newMeshPrefab = Registry.GetMesh(ThisCollectable);
        if (newMeshPrefab == null)
        {
            Debug.LogWarning($"[{name}] No CollectableModel to respawn for {ThisCollectable}");
            return;
        }

        // Try to reuse an existing mesh
        MeshRenderer existingMesh = GetComponentInChildren<MeshRenderer>(true);
        if (existingMesh == null)
        {
            existingMesh = Instantiate(newMeshPrefab, transform);
            Debug.Log($"[{name}] Spawned new mesh for {ThisCollectable}");
        }
        else
        {
            Debug.Log($"[{name}] Updated existing mesh for {ThisCollectable}");
        }

        ApplyMeshData(existingMesh, newMeshPrefab);
    }
    #endregion


    #region Helpers
    private void ApplyMeshData(MeshRenderer target, MeshRenderer source)
    {
        // Copy mesh and materials
        MeshFilter targetFilter = target.GetComponent<MeshFilter>();
        MeshFilter sourceFilter = source.GetComponent<MeshFilter>();

        if (targetFilter != null && sourceFilter != null)
            targetFilter.sharedMesh = sourceFilter.sharedMesh;

        target.sharedMaterials = source.sharedMaterials;

        // Reset transform and ensure visibility
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = Vector3.one;
        target.gameObject.SetActive(true);
    }
    private void LoadRegistry()
    {
        if (Registry == null)
            Registry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);

        if (Registry == null)
            Debug.LogWarning("CollectableRegistry not found in Resources.");
    }

    private void UpdateVisibility()
    {
        gameObject.SetActive(DebugAlwaysDisplay || SaveDataManager.Instance.IsCollectableCollected(ThisCollectable));
    }
    #endregion


    #region Debug Tools
#if UNITY_EDITOR
    [Button("Preview Collectable")]
    private void PreviewCollectable()
    {
        LoadRegistry();
        if (Registry != null)
            spawnMesh(ThisCollectable);
    }
#endif
    #endregion
}
