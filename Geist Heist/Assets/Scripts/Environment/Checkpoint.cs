/*
 * Author: Jacob Bateman
 * Contributors: Toby
 * Creation: 10/21/25
 * Last Edited: 11/14/25
 * Summary: Handles in-level checkpoint logic
 */
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private string checkpointStateIdOverride;

    private string cachedCheckpointStateId;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj == PlayerManager.Instance.CurrentObject)
            {
                Vector3 checkpointSpawnLocation = spawnLocation != null ? spawnLocation.position : transform.position;
                if (!LevelManager.Instance.UpdateCheckpoint(checkpointSpawnLocation, this))
                    return;

                Debug.Log($"Checkpoint: '{gameObject.name}' Reached by {other.gameObject.name}");
                PlayerHUDManager.Instance.CheckpointAnimationRef.OpenCheckpointAnimation();
            }
        }
    }

    public string GetCheckpointStateId()
    {
        if (!string.IsNullOrWhiteSpace(checkpointStateIdOverride))
            return checkpointStateIdOverride;

        if (!string.IsNullOrEmpty(cachedCheckpointStateId))
            return cachedCheckpointStateId;

        cachedCheckpointStateId = BuildHierarchyStateId();
        return cachedCheckpointStateId;
    }

    private string BuildHierarchyStateId()
    {
        List<string> segments = new();
        Transform current = transform;

        while (current != null)
        {
            segments.Add($"{current.name}[{current.GetSiblingIndex()}]");
            current = current.parent;
        }

        segments.Reverse();
        return $"{gameObject.scene.buildIndex}:{string.Join("/", segments)}";
    }
}
