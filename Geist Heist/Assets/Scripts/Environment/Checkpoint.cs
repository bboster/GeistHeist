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
        if(other.gameObject.TryGetComponent(out ThirdPersonInputHandler handler))
        {
            //GameManager.CurrentSpawnLocation = spawnLocation.position;
        }
    }
}
