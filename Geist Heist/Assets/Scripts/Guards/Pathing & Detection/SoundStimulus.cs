/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/07/25
 * Summary: The stimulus trigger for a sound or distractable-based stimulus.
 */

using System.Collections;
using UnityEngine;

public class SoundStimulus : Stimulus
{
    private GuardController contactedGuard;

    [SerializeField] private float soundLifetime;

    [SerializeField] private bool makeSoundWaveVFX = true;

    private void Start()
    {
        // expand the target scale cus it looks cool (and so it lines up with the particle)
        Vector3 targetScale = transform.localScale;

        if (makeSoundWaveVFX)
            SoundWaveManager.Instance.CreateSoundWaveAtPosition(transform.position, targetScale.x, soundLifetime, collision: false);

                                                                        // keep it there for just a lil longer
        StaticUtilities.AnimateScale(transform, Vector3.zero, targetScale, soundLifetime + 0.25f, unscaledTime: false);

        StartCoroutine(SoundLength());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out GuardController guard))
        {
            contactedGuard = guard;
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

    /// <summary>
    /// Controls the lifetime of the sound.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SoundLength()
    {
        yield return new WaitForSeconds(soundLifetime);
        Destroy(gameObject);
    }
}
