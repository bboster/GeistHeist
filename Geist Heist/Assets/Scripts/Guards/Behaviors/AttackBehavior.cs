/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/18/25
 * Last Edited: 9/18/25
 * Summary: Handles behavior for the enemy when it is attacking the player.
 * TO DO: Replace marked lines when animations are implemented
 */

using UnityEngine;
using GuardUtilities;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Attack Behavior", menuName = "Guard Behaviors/New Attack Behavior")]
public class AttackBehavior : Behavior
{
    private bool performingAttack = true;
    private bool loopRunning = false;

    [SerializeField] private float attackLength; //REPLACE WITH ANIMATION STUFF LATER

    /// <summary>
    /// Controls the flow of the attack behavior for the enemy.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        NavMeshAgent thisAgent = selfRef.GetComponent<NavMeshAgent>();
        thisAgent.isStopped = true;

        for (; ; )
        {
            if (performingAttack == true && contRef.currentBehavior.StateName == GuardStates.attack)
            {
                //Debug.Log("Player Caught");
                if (!GameManager.Instance.fadingToBlack)
                    GameManager.Instance.DeathReset();
                performingAttack = false; //This should be removed later and the variable should be changed by an animation keyframe.
                yield return new WaitForSeconds(attackLength);
            }
            else
            {
                contRef.ChangeBehavior(GuardStates.returnToPath);
            }

            yield return new WaitForSeconds(attackLength);
        }


    }
}
