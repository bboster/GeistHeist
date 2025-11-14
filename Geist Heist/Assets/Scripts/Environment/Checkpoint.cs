/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/21/25
 * Last Edited: 10/21/25
 * Summary: Handles in-level checkpoint logic
 */
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform spawnLocation;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj == PlayerManager.Instance.CurrentObject)
            {
                Debug.Log($"Checkpoint: '{gameObject.name}' Reached");
                LevelManager.Instance.UpdateCheckpoint(spawnLocation.position);
                PlayerHUDManager.Instance.CheckpointAnimationRef.OpenCheckpointAnimation();
            }
        }
    }
}
