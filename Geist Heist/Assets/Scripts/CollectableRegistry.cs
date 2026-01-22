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

/*
* I still feel like the collectable registry was really overkill. It's fine and its good but like, wow.
* 
* Can we make it so the collectable registry only updates/saves to the file if an actual change is detected? Tbh im a little tired of see the changed file so often.
* 
* Also could this be moved to environment folder ?
* 
* -Toby
*/

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
        if((int)collectable <= 0) return null;

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

        /*
         * Only do this if a change actually happened (?)
         * -Toby
         */

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    /// Remove an entry (editor-only)
    public void Remove(Collectable collectable)
    {
        /*
         * Only do this if a change actually happened (?)
         * -Toby
         */

        entries.RemoveAll(e => e.collectable == collectable);
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}
