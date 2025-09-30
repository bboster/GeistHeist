/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/18/25
 * Last Edited: 9/30/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using UnityEngine;

public class VisionCone : MonoBehaviour
{
    private bool hasSeenPlayer = false;

    [Tooltip("The index of the behavior to activate when the player is seen. WILL REPLACE WITH BETTER SYSTEM WHEN I THINK OF ONE")]
    [SerializeField] private int behaviorIndex;
    [Tooltip("The index of the behavior to activate when the enemy loses track of the player during a chase.")]
    [SerializeField] private int recoveryBehaviorIndex;

    [SerializeField] private GuardController parentController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("1st Person Player") && hasSeenPlayer == false)
        {
            //Debug.Log("PLAYER SEEN");
            hasSeenPlayer = true;
            parentController.ChangeBehavior(GuardUtilities.GuardData.GuardStates.chase);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name.Equals("1st Person Player") && hasSeenPlayer == true)
        {
            //Debug.Log("PLAYER UNSEEN");
            hasSeenPlayer = false;
            parentController.ChangeBehavior(GuardUtilities.GuardData.GuardStates.patrol); // (Will swap to a proper recovery behavior once that is programmed)
        }
    }
}
