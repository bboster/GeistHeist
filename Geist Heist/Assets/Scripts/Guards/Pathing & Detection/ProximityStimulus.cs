/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/21/25
 * Last Edited: 10/21/25
 * Summary: The stimulus trigger for the player entering close-proximity with the guard.
 */
using NaughtyAttributes;
using UnityEngine;

public class ProximityStimulus : Stimulus
{
    [SerializeField, Required] private GuardController parentGuard;

    public override void TriggerStimulus()
    {
        parentGuard.RecieveStimulus(this, stateToChangeTo, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ThirdPersonInputHandler handler))
        {
            TriggerStimulus();
        }
    }
}
