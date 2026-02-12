using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

/*
 * Contributors: Joshua Kelly, Toby
 * Creation Date: 10/23/25
 * Last Modified: 11/18/25
 * 
 * Brief Description: Handles the display of the currently equipped wearable (like hats).
 * Do NOT attach this to the player prefab directly.
 */

/*
 * Looked it up on the goog, "Collectible" is technically correct spelling, but we spell it "Collectable" everywhere else in the game.
 * 
 * I wonder if we should add a position offset variable to the collectable collectableRegistry, so we can have more control over where each hat goes.
 * 
 * Also, this script should be moved to environment folder or the interactables folder, since it is an interactable.
 * 
 * -Toby
 */

public class WearableCollectible : MonoBehaviour
{
    [SerializeField, Required] private GameObject wearableNode;

    [Header("Debug")]
    private CollectableRegistry Registry;
    [ReadOnly] public Collectable currentHat; 
    private Collectable previousHat;

    private OptionalCollectableHubDisplay[] allHubDisplays;

    private void OnValidate()
    {
    }

    private void Awake()
    {
        if (Registry == null)
            Registry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);
    }

    private void Start()
    {
        // Load what the player had equipped last
        currentHat = GetEquippedCollectable(SaveDataManager.Instance.EquipedHat());
        allHubDisplays = FindObjectsByType<OptionalCollectableHubDisplay>(FindObjectsSortMode.None);

#if UNITY_EDITOR
        PreviewHat();
#endif
    }

    public void EquipHat(Collectable newCollectable, bool debug=false)
    {
        if(!debug)
            SaveDataManager.Instance.MarkCollectableAsWorn(newCollectable);

        previousHat = currentHat;
        // Update the current reference
        currentHat = newCollectable;

        // Find the Wearable node
        if (wearableNode == null)
        {
            wearableNode = this.gameObject;
            //Debug.LogError("No 'Wearable' object found in the scene or player hierarchy!");
            //return;
        }

        Transform wearableTransform = wearableNode.transform;
        int childCount = wearableTransform.childCount;
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
            children[i] = wearableTransform.GetChild(i);

        /*
         * Why are we destroying children just to replace them immediately? 
         * Just replace the mesh in the current mesh renderer?
         * -Toby
         * 
         *  Leaving this note here because I think it raises a valid point. That being said,
         *  assuming I understand how this system works, I would have to do a substantial rework
         *  to realize this change.
         *  -Jacob
         */
        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject); // or Destroy(child.gameObject) at runtime
        }

        // Instantiate the new hat if it isnt none

        // Get the correct mesh *each time*
        MeshRenderer meshPrefab = Registry.GetMeshRenderer(currentHat);
        if (meshPrefab == null)
        {
            Debug.LogWarning($"No mesh prefab found for {currentHat}.");
            return;
        }

        MeshRenderer hat = Instantiate(meshPrefab, wearableTransform);
        hat.transform.localPosition = Vector3.zero;
        hat.transform.localRotation = Quaternion.identity;

        if (Application.isPlaying)
        {
            ReplaceHat(previousHat);
        }
    }

    public void ReplaceHat(Collectable previousHat)
    {
        // Move the current hat back to hub instead of destroying
        if (previousHat != Collectable.None)
        {            
            foreach (var display in allHubDisplays)
            {
                Debug.Log($"Attempting to call display.spawnMesh({previousHat}) in hub display.");
                display.spawnMesh(previousHat); // currentHat = Collectable currently equipped
            }

            // No need to destroy currentHat, as it's an enum
        }
    }

    public Collectable GetEquippedCollectable(int collectableValue)
    {
        if (collectableValue < 0 || !Enum.IsDefined(typeof(Collectable), collectableValue))
            return Collectable.None;

        return (Collectable)collectableValue;
    }

    #region Debug Tools
#if UNITY_EDITOR
    [Header("Debug Only")]
    [SerializeField] private Collectable PreviewCollectable;

    [Button("Preview Hat")]
    private void PreviewHat()
    {
        if (Registry == null)
        {
            Registry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);
            if (Registry == null)
            {
                Debug.LogWarning("Registry not found.");
                return;
            }
        }

        EquipHat(PreviewCollectable, debug:true);
    }

    [Button("Manual Equip Hat")]
    private void MarkHatAsWorn()
    {
        PreviewHat();
        SaveDataManager.Instance.MarkCollectableAsWorn(PreviewCollectable);
    }
#endif
    #endregion
}
