/*
 * Contributors:Josh
 * Creation: 10/22/205
 * Last Edited: 10/27/205
 * Summary: Registry of collectable types to their mesh prefabs.
 * 
 * TODO: make 
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableRegistry", menuName = "Data/Collectable Registry")]
public class CollectableRegistry : ScriptableObject
{
    public const string RESOURCE_PATH = "CollectableRegistry";

    [System.Serializable]
    public class CollectableEntry
    {
        public Collectable collectable;
        public GameObject meshPrefab;
    }

    [SerializeField] private List<CollectableEntry> entries = new();

    // Runtime-safe read-only access
    public IReadOnlyList<CollectableEntry> Entries => entries;

    /// Get the mesh renderer for a given collectable enum
    public MeshRenderer GetMesh(Collectable collectable)
    {
        var entry = entries.Find(e => e.collectable == collectable);
        return entry?.meshPrefab?.GetComponent<MeshRenderer>();
    }

#if UNITY_EDITOR
    /// Add or update an entry in the registry (editor-only)
    public void AddOrUpdate(Collectable collectable, GameObject meshPrefab)
    {
        var existing = entries.Find(e => e.collectable == collectable);
        if (existing != null)
        {
            existing.meshPrefab = meshPrefab;
        }
        else
        {
            entries.Add(new CollectableEntry
            {
                collectable = collectable,
                meshPrefab = meshPrefab
            });
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    /// Remove an entry (editor-only)
    public void Remove(Collectable collectable)
    {
        entries.RemoveAll(e => e.collectable == collectable);
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}
