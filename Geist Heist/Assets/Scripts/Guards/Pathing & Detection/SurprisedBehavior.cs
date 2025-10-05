/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/01/25
 * Last Edited: 10/01/25
 * Summary: Runs behavior for when the guard sees the player.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using GuardUtilities;

[CreateAssetMenu(fileName = "New Surprised Behavior", menuName = "Guard Behaviors/New Surprised Behavior")]
public class SurprisedBehavior : Behavior
{
    [Tooltip("The length that the guard will pause before chasing after seeing the player")]
    [SerializeField] private float reactionLength;

    /// <summary>
    /// Runs the logic for the behavior.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        NavMeshAgent thisAgent = selfRef.GetComponent<NavMeshAgent>();

        thisAgent.isStopped = true;
        yield return new WaitForSeconds(reactionLength); //REPLACE THIS WITH SOMETHING TO TIE IN ANIMATIONS LATER
        thisAgent.isStopped = false;
        selfRef.GetComponent<GuardController>().ChangeBehavior(GuardStates.chase);
    }
}
