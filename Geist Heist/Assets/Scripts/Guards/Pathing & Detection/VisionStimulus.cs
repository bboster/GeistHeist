/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/02/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using UnityEngine;
using GuardUtilities;

public class VisionStimulus : Stimulus
{
    private bool hasSeenPlayer = false;

    [Tooltip("The index of the behavior to activate when the player is seen. WILL REPLACE WITH BETTER SYSTEM WHEN I THINK OF ONE")]
    [SerializeField] private int behaviorIndex;
    [Tooltip("The index of the behavior to activate when the enemy loses track of the player during a chase.")]
    [SerializeField] private int recoveryBehaviorIndex;

    [SerializeField] private GuardController parentController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("1st Person Player") && hasSeenPlayer == false)
        {
            //Debug.Log("PLAYER SEEN");
            hasSeenPlayer = true;
            TriggerStimulus();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("1st Person Player") && hasSeenPlayer == true)
        {
            //Debug.Log("PLAYER UNSEEN");
            hasSeenPlayer = false;
            parentController.ChangeBehavior(GuardStates.patrol); // (Will swap to a proper recovery behavior once that is programmed)
        }
    }

    /// <summary>
    /// Sends the stimulus to the guard recieving it
    /// </summary>
    public override void TriggerStimulus()
    {
        parentController.RecieveStimulus(this, stateToChangeTo);
        //parentController.ChangeBehavior(GuardUtilities.GuardData.GuardStates.surprised);
    }
}
