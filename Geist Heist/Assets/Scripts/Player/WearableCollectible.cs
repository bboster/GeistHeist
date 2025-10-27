using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

/*
 * Contributors: Joshua Kelly
 * Creation Date: 10/23/25
 * Last Modified: 10/23/25
 * 
 * Brief Description: Handles the display of the currently equipped wearable (like hats).
 * Do NOT attach this to the player prefab directly.
 */
public class WearableCollectible : MonoBehaviour
{
    private CollectableRegistry Registry;
    private Collectable currentHat;
    private Collectable previousHat;

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
        EquipHat(currentHat);
    }

    public void EquipHat(Collectable newCollectable)
    {
        previousHat = currentHat;
        // Update the current reference
        currentHat = newCollectable;

        // Get the correct mesh *each time*
        MeshRenderer meshPrefab = Registry.GetMesh(currentHat);
        if (meshPrefab == null)
        {
            Debug.LogWarning($"No mesh prefab found for {currentHat}.");
            return;
        }

        // Find the Wearable node
        GameObject wearableNode = GameObject.Find("Wearable");
        if (wearableNode == null)
        {
            Debug.LogError("No 'Wearable' object found in the scene or player hierarchy!");
            return;
        }

        // Destroy any existing hat
        Transform wearableTransform = wearableNode.transform;
        int childCount = wearableTransform.childCount;
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
            children[i] = wearableTransform.GetChild(i);

        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject); // or Destroy(child.gameObject) at runtime
        }

        // Instantiate the new hat
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
            // Find all OptionalCollectable objects in the scene
            var allHubDisplays = FindObjectsByType<OptionalCollectableHubDisplay>(FindObjectsSortMode.None);
            foreach (var display in allHubDisplays)
            {
                Debug.Log($"Attemptin to call display.RespawnMes.{previousHat} in hub display.");
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

        EquipHat(PreviewCollectable);
    }
#endif
    #endregion
}
