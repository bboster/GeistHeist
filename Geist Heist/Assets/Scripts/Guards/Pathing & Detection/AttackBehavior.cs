/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/18/25
 * Last Edited: 9/18/25
 * Summary: Handles behavior for the enemy when it is attacking the player.
 */

using UnityEngine;
using GuardUtilities;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Behavior", menuName = "Guard Behaviors/New Attack Behavior")]
public class AttackBehavior : Behavior
{
    private bool performingAttack = true;

    /// <summary>
    /// Ensures that the performingAttack variable is reset before stopping the behavior.
    /// </summary>
    /*public override void StopBehavior()
    {
        performingAttack = true;
        base.StopBehavior();
    }*/

    /// <summary>
    /// Controls the flow of the attack behavior for the enemy.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            if(performingAttack == true)
            {
                Debug.Log("Player Caught");
                performingAttack = false; //This should be removed later and the variable should be changed by an animation keyframe.
            }
            else
            {
                selfRef.GetComponent<GuardController>().ChangeBehavior(GuardData.GuardStates.patrol);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
