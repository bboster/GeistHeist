/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/02/25
 * Summary: The stimulus trigger for a sound or distractable-based stimulus.
 */

using UnityEngine;


public class SoundStimulus : Stimulus
{
    private GuardController contactedGuard;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<GuardController>(out GuardController controller))
        {
            contactedGuard = controller;
            TriggerStimulus();
        }
    }

    /// <summary>
    /// Sends the stimulus to the guard recieving it
    /// </summary>
    public override void TriggerStimulus()
    {
        contactedGuard.RecieveStimulus(this, stateToChangeTo, transform.position);
    }
}
