/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/18/25
 * Last Edited: 9/18/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [Tooltip("The index of the behavior to activate when the player is seen. WILL REPLACE WITH BETTER SYSTEM WHEN I THINK OF ONE")]
    [SerializeField] private int behaviorIndex;
    [Tooltip("The index of the behavior to activate when the enemy loses track of the player during a chase.")]
    [SerializeField] private int recoveryBehaviorIndex;

    [SerializeField] private EnemyController parentController;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("PLAYER SEEN");
        parentController.ChangeBehavior(behaviorIndex);
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("PLAYER UNSEEN");
        parentController.ChangeBehavior(recoveryBehaviorIndex); // (Will swap to a recovery behavior once that is programmed)
    }
}
