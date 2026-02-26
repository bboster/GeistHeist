/*
 * Contributors: Brenden(?), Toby
 * Creation Date: ?
 * Last Modified: 12/3/2025
 * 
 * Brief Description: 
 */

using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class VendingCanController : MonoBehaviour
{       
    [SerializeField] private GameObject soundStimulus;

    [Header("Onomatopoeia")]
    [SerializeField] private string OnomatopoeiaText = "clank!";

    [Header("Sound Wave")]
    [SerializeField] private float soundWaveRadius = 3f;
    [SerializeField] private float soundWaveLifetime = 0.75f;

    bool firstTime = true;

    [SerializeField] float CanDespawnTimer;
    private SuddenVelocityChangeDetector velocityChangeDetector;

    private void Start()
    {
        velocityChangeDetector = GetComponent<SuddenVelocityChangeDetector>();
        velocityChangeDetector.OnBounceDetected.AddListener(OnCrashOrBounceDetected); //These listeners should probably be removed if they aren't removed elsewhere
        velocityChangeDetector.OnStopDetected.AddListener(OnCrashOrBounceDetected);
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.CanBounce, transform.position);

        if (firstTime)
        {
            Instantiate(soundStimulus, transform.position, Quaternion.identity);
            Debug.Log("Stimulus");
            firstTime = false;
            OnCrashOrBounceDetected(collision.contacts[0].point);
        }
    }

    private IEnumerator DeleteCanClutter()
    {
        yield return new WaitForSeconds(CanDespawnTimer);
        Destroy(this.gameObject);
    }

    private void OnCrashOrBounceDetected(Vector3 impactPoint)
    {
        Vector3 spawnPoint = impactPoint + (Vector3.up * 2);
        BillboardUIManager.Instance.SpawnOnomatopoeia(OnomatopoeiaText, spawnPoint, randomRotationRange: 25, bold: true, fontScale:0.4f);
        Debug.Log("Clank Spawned");
        //TODO: add clank sound

        // TODO: add particle

        SoundWaveManager.Instance.CreateSoundWaveAtPosition(impactPoint, soundWaveRadius, soundWaveLifetime, collision: false);
    }
}
