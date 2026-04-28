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

    private WearableCollectible wearableCollectible;
    private ButtonPromptInteractable buttonPrompt;

    #region Unity Lifecycle
    private void OnValidate()
    {
#if UNITY_EDITOR
        LoadRegistry();
        EnsureTriggerCollider();
        EnsureButtonPromptSetup();
#endif
    }

    private void Awake()
    {
        LoadRegistry();
        EnsureTriggerCollider();
        EnsureButtonPromptSetup();
    }

    private void Start()
    {
        spawnMesh(ThisCollectable);
        UpdateVisibility();
    }
    #endregion


    #region Interaction
    void IInteractable.Interact()
    {
        if (SaveDataManager.Instance.IsCollectableCollected(ThisCollectable) == false)
            return;

        if (wearableCollectible == null)
            wearableCollectible = FindAnyObjectByType<WearableCollectible>();

        if (wearableCollectible == null)
        {
            Debug.LogWarning("No WearableCollectible instance found in the scene.");
            return;
        }

        //bald logic below
        bool isThisHatEquipped = SaveDataManager.Instance.EquipedHat() == (int)ThisCollectable;

        if (isThisHatEquipped)
        {
            wearableCollectible.EquipHat(Collectable.None);
            Debug.Log($"{ThisCollectable} placed back in display.");
        }
        else
        {
            MeshRenderer meshPrefab = Registry.GetWearableMeshRenderer(ThisCollectable);
            if (meshPrefab == null)
            {
                Debug.LogWarning($"No prefab found in Registry for {ThisCollectable}.");
                return;
            }

            wearableCollectible.EquipHat(ThisCollectable);
            Debug.Log($"{ThisCollectable} equipped successfully!");
        }

        UpdateVisibility();
    }

    bool IInteractable.IsInteractable()
    {
        return SaveDataManager.Instance.IsCollectableCollected(ThisCollectable);
    }
    #endregion


    #region Mesh Spawning
    public void spawnMesh(Collectable collectableToRespawn)
    {
        if (ThisCollectable != collectableToRespawn)
            return;

        MeshRenderer newMeshPrefab = Registry.GetDisplayMeshRenderer(ThisCollectable);
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

        target.transform.localPosition = source.transform.localPosition + Registry.GetDisplayLocalPosition(ThisCollectable);
        target.transform.localEulerAngles = source.transform.localEulerAngles + Registry.GetDisplayLocalEulerAngles(ThisCollectable);
        target.transform.localScale = Vector3.Scale(source.transform.localScale, Registry.GetDisplayLocalScale(ThisCollectable));
        target.gameObject.SetActive(true);
    }
    private void LoadRegistry()
    {
        if (Registry == null)
            Registry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);

        if (Registry == null)
            Debug.LogWarning("CollectableRegistry not found in Resources.");
    }

    private void EnsureTriggerCollider()
    {
        if (TryGetComponent<Collider>(out var coll) && !coll.isTrigger)
            coll.isTrigger = true;
    }

    private void EnsureButtonPromptSetup()
    {
        if (buttonPrompt == null)
            buttonPrompt = GetComponentInChildren<ButtonPromptInteractable>(true);

        if (buttonPrompt == null)
            return;

        buttonPrompt.buttonKey = ButtonType.Interact;
        buttonPrompt.additionalButtonText = string.Empty;
    }

    public void UpdateVisibility()
    {
        bool isCollected = SaveDataManager.Instance.IsCollectableCollected(ThisCollectable);
        bool isThisHatEquipped = SaveDataManager.Instance.EquipedHat() == (int)ThisCollectable;

        gameObject.SetActive(DebugAlwaysDisplay || isCollected);
        MeshRenderer existingMesh = GetComponentInChildren<MeshRenderer>();
        if (existingMesh != null)
                existingMesh.enabled = isCollected && !isThisHatEquipped;
    }
    #endregion


    #region Debug Tools
#if UNITY_EDITOR
    [Button("Save Scene Mesh As Display Offset")]
    private void SaveSceneMeshAsDisplayOffset()
    {
        LoadRegistry();
        if (Registry == null)
        {
            Debug.LogWarning("CollectableRegistry not found in Resources.");
            return;
        }

        MeshRenderer sceneMesh = GetComponentInChildren<MeshRenderer>(true);
        if (sceneMesh == null)
        {
            Debug.LogWarning($"[{name}] No scene mesh found to read transform from.");
            return;
        }

        MeshRenderer sourceMesh = Registry.GetDisplayMeshRenderer(ThisCollectable);
        if (sourceMesh == null)
        {
            Debug.LogWarning($"[{name}] No prefab is set in CollectableRegistry for {ThisCollectable}.");
            return;
        }

        Vector3 positionOffset = sceneMesh.transform.localPosition - sourceMesh.transform.localPosition;
        Vector3 rotationOffset = new Vector3(
            Mathf.DeltaAngle(sourceMesh.transform.localEulerAngles.x, sceneMesh.transform.localEulerAngles.x),
            Mathf.DeltaAngle(sourceMesh.transform.localEulerAngles.y, sceneMesh.transform.localEulerAngles.y),
            Mathf.DeltaAngle(sourceMesh.transform.localEulerAngles.z, sceneMesh.transform.localEulerAngles.z));
        Vector3 scaleOffset = new Vector3(
            SafeDivide(sceneMesh.transform.localScale.x, sourceMesh.transform.localScale.x),
            SafeDivide(sceneMesh.transform.localScale.y, sourceMesh.transform.localScale.y),
            SafeDivide(sceneMesh.transform.localScale.z, sourceMesh.transform.localScale.z));

        if (!Registry.TrySetDisplayOffsets(ThisCollectable, positionOffset, rotationOffset, scaleOffset))
        {
            Debug.LogWarning($"[{name}] Could not update offsets. No registry entry exists for {ThisCollectable}.");
            return;
        }

        Debug.Log($"[{name}] Saved display offsets for {ThisCollectable}. Position: {positionOffset}, Rotation: {rotationOffset}, Scale: {scaleOffset}");
    }

    private float SafeDivide(float numerator, float denominator)
    {
        if (Mathf.Approximately(denominator, 0f))
            return 1f;

        return numerator / denominator;
    }

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
