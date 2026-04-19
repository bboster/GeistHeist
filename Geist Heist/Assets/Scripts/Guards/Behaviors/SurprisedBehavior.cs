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
using FMOD.Studio;
using FMODUnity;

[CreateAssetMenu(fileName = "New Surprised Behavior", menuName = "Guard Behaviors/New Surprised Behavior")]
public class SurprisedBehavior : Behavior
{
    [Tooltip("The length that the guard will pause before chasing after seeing the player")]
    [SerializeField] private float reactionLength;

    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        contRef.GetAnimator().SetBool("isShocked", true);
    }

    public override void StopBehavior()
    {
        base.StopBehavior();
        contRef.GetAnimator().SetBool("isShocked", false);
    }

    private void Start()
    {
        //guardGasp = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.GuardReactions);
        //guardGasp.set3DAttributes(RuntimeUtils.To3DAttributes(this.GetComponent<transform>(), this.GetComponent<Rigidbody>()));
    }

    /// <summary>
    /// Runs the logic for the behavior.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        //AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GuardReactions);
        
        contRef.PlayVoiceline(1);
        contRef.GuardTalking("Ghost! WHAT IS A GHOST DOING HERE?");

        NavMeshAgent thisAgent = selfRef.GetComponent<NavMeshAgent>();

        thisAgent.isStopped = true;

/*#if UNITY_EDITOR
        selfRef.GetComponent<GuardDebugger>().StartDebugProgress(reactionLength, this);
#endif*/

        yield return new WaitForSeconds(reactionLength); //REPLACE THIS WITH SOMETHING TO TIE IN ANIMATIONS LATER
        thisAgent.isStopped = false;

        contRef.ChangeBehavior(GuardStates.chase);

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerSpotted);
    }
}
