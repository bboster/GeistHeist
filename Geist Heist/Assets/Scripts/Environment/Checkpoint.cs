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
        // dont even bother if player already has this checkpoint
        if (LevelManager.Instance.IsCheckpointCurrent(this))
            return;

        if(other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj == PlayerManager.Instance.CurrentObject)
            {
                Debug.Log($"Checkpoint: '{gameObject.name}' Reached by {other.gameObject.name}");
                LevelManager.Instance.UpdateCheckpoint(spawnLocation.position, this);
                PlayerHUDManager.Instance.CheckpointAnimationRef.OpenCheckpointAnimation();
            }
        }
    }
}
