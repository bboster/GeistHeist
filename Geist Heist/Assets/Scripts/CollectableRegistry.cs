/*
 * Contributors:Josh
 * Creation: 10/22/205
 * Last Edited: 10/27/205
 * Summary: collectableRegistry of collectable types to their mesh prefabs.
 * 
 * TODO: make 
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CollectableRegistry", menuName = "Data/Collectable Registry")]
public class CollectableRegistry : ScriptableObject
{
    public const string RESOURCE_PATH = "CollectableRegistry";

    [System.Serializable]
    public class CollectableEntry
    {
        public Collectable collectable;

        [FormerlySerializedAs("meshPrefab")]
        public GameObject wearablePrefab;
        [SerializeField, HideInInspector, FormerlySerializedAs("displayPrefab")] private GameObject legacyDisplayPrefab;

        [Header("Wearable Offsets")]
        public Vector3 wearableLocalPosition = Vector3.zero;
        public Vector3 wearableLocalEulerAngles = Vector3.zero;
        public Vector3 wearableLocalScale = Vector3.one;

        [Header("Display Offsets")]
        public Vector3 displayLocalPosition = Vector3.zero;
        public Vector3 displayLocalEulerAngles = Vector3.zero;
        public Vector3 displayLocalScale = Vector3.one;

        public GameObject WearablePrefab => wearablePrefab != null ? wearablePrefab : legacyDisplayPrefab;
        public GameObject DisplayPrefab => wearablePrefab != null ? wearablePrefab : legacyDisplayPrefab;

        private static MeshRenderer GetPrimaryRenderer(GameObject prefab)
        {
            if (prefab == null)
                return null;

            MeshRenderer renderer = prefab.GetComponent<MeshRenderer>();
            if (renderer != null)
                return renderer;

            return prefab.GetComponentInChildren<MeshRenderer>(true);
        }

        public MeshRenderer GetWearableRenderer() => GetPrimaryRenderer(WearablePrefab);
        public MeshRenderer GetDisplayRenderer() => GetPrimaryRenderer(DisplayPrefab);

        public Vector3 GetWearableScaleOrDefault() => wearableLocalScale == Vector3.zero ? Vector3.one : wearableLocalScale;
        public Vector3 GetDisplayScaleOrDefault() => displayLocalScale == Vector3.zero ? Vector3.one : displayLocalScale;

        public Material[] GetMaterials()
        {
            return GetWearableRenderer()?.sharedMaterials;
        }
    }

    [SerializeField] private List<CollectableEntry> entries = new();

    // Runtime-safe read-only access
    public IReadOnlyList<CollectableEntry> Entries => entries;

    private CollectableEntry GetEntry(Collectable collectable)
    {
        if ((int)collectable <= 0)
            return null;

        return entries.Find(e => e.collectable == collectable);
    }

    /// Get the mesh renderer for a given collectable enum
    public MeshRenderer GetMeshRenderer(Collectable collectable)
    {
        return GetWearableMeshRenderer(collectable);
    }

    public MeshRenderer GetWearableMeshRenderer(Collectable collectable)
    {
        return GetEntry(collectable)?.GetWearableRenderer();
    }

    public MeshRenderer GetDisplayMeshRenderer(Collectable collectable)
    {
        return GetEntry(collectable)?.GetDisplayRenderer();
    }

    public Vector3 GetWearableLocalPosition(Collectable collectable)
    {
        return GetEntry(collectable)?.wearableLocalPosition ?? Vector3.zero;
    }

    public Vector3 GetWearableLocalEulerAngles(Collectable collectable)
    {
        return GetEntry(collectable)?.wearableLocalEulerAngles ?? Vector3.zero;
    }

    public Vector3 GetWearableLocalScale(Collectable collectable)
    {
        return GetEntry(collectable)?.GetWearableScaleOrDefault() ?? Vector3.one;
    }

    public Vector3 GetDisplayLocalPosition(Collectable collectable)
    {
        return GetEntry(collectable)?.displayLocalPosition ?? Vector3.zero;
    }

    public Vector3 GetDisplayLocalEulerAngles(Collectable collectable)
    {
        return GetEntry(collectable)?.displayLocalEulerAngles ?? Vector3.zero;
    }

    public Vector3 GetDisplayLocalScale(Collectable collectable)
    {
        return GetEntry(collectable)?.GetDisplayScaleOrDefault() ?? Vector3.one;
    }

    public Material[] GetMaterials(Collectable collectable)
    {
        return GetEntry(collectable)?.GetMaterials();
    }

    public Mesh GetMesh(Collectable collectable)
    {
        MeshRenderer renderer = GetWearableMeshRenderer(collectable);
        if (renderer == null)
            return null;

        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        return filter != null ? filter.sharedMesh : null;
    }

#if UNITY_EDITOR
    public bool TrySetDisplayOffsets(Collectable collectable, Vector3 localPositionOffset, Vector3 localEulerOffset, Vector3 localScaleOffset)
    {
        CollectableEntry existing = entries.Find(e => e.collectable == collectable);
        if (existing == null)
            return false;

        existing.displayLocalPosition = localPositionOffset;
        existing.displayLocalEulerAngles = localEulerOffset;
        existing.displayLocalScale = localScaleOffset;

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        return true;
    }

    public bool TrySetWearableOffsets(Collectable collectable, Vector3 localPositionOffset, Vector3 localEulerOffset, Vector3 localScaleOffset)
    {
        CollectableEntry existing = entries.Find(e => e.collectable == collectable);
        if (existing == null)
            return false;

        existing.wearableLocalPosition = localPositionOffset;
        existing.wearableLocalEulerAngles = localEulerOffset;
        existing.wearableLocalScale = localScaleOffset;

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        return true;
    }

    /// Add or update an entry in the collectableRegistry (editor-only)
    public void AddOrUpdate(Collectable collectable, GameObject meshPrefab)
    {
        var existing = entries.Find(e => e.collectable == collectable);
        if (existing != null)
        {
            existing.wearablePrefab = meshPrefab;
        }
        else
        {
            entries.Add(new CollectableEntry
            {
                collectable = collectable,
                wearablePrefab = meshPrefab
            });
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    /// Remove an entry (editor-only)
    public void Remove(Collectable collectable)
    {
        if(entries.Find(e => e.collectable == collectable) != null)
        {
            entries.RemoveAll(e => e.collectable == collectable);
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
#endif
}
