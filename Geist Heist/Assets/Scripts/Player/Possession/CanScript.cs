/*
 * Contributors: Brenden(?), Toby
 * Creation Date: ?
 * Last Modified: 12/3/2025
 * 
 * Brief Description: 
 */

using System.Collections;
using UnityEngine;

public class CanScript : MonoBehaviour
{

    [SerializeField] private GameObject soundStimulus;

    [Header("VFX")]
    [SerializeField] private string OnomatopoeiaText = "clank!";

    bool firstTime = true;

    private SuddenVelocityChangeDetector velocityChangeDetector;

    private void Start()
    {
        velocityChangeDetector = GetComponent<SuddenVelocityChangeDetector>();
        velocityChangeDetector.OnBounceDetected.AddListener(OnCrashOrBounceDetected);
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
        }
    }

    void OnCrashOrBounceDetected(Vector3 impactPoint)
    {
        Vector3 spawnPoint = impactPoint + (Vector3.up * 2);
        BillboardUIManager.Instance.SpawnOnomatopoeia(OnomatopoeiaText, spawnPoint, randomRotationRange: 25, bold: true, fontScale:0.4f);

        //TODO: add clank sound

        // TODO: add particle
    }
}
