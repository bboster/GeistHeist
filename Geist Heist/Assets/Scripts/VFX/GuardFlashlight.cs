/*
 * Contributors: Toby
 * Creation Date: 12/3/2025
 * Last Modified: 12/3/2025
 * 
 * Brief Description: Currently just spawns an Onomatopoeia when the guard drops it.
 * Guard doesnt drop the flashlight yet tbh. Just gonna trust this works
 */

using UnityEngine;

public class GuardFlashlight : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private string onomatopoeiaText = "Thunk!"; //Private variables should use camelCase

    private SuddenVelocityChangeDetector velocityChangeDetector;

    void Start()
    {
        velocityChangeDetector = GetComponent<SuddenVelocityChangeDetector>();
        velocityChangeDetector.OnStopDetected.AddListener(OnCrashOrBounceDetected);
        velocityChangeDetector.OnBounceDetected.AddListener(OnCrashOrBounceDetected);
    }

    void OnCrashOrBounceDetected(Vector3 impactPoint)
    {
        Vector3 spawnPoint = impactPoint + (Vector3.up * 2);
        BillboardUIManager.Instance.SpawnOnomatopoeia(onomatopoeiaText, spawnPoint, randomRotationRange: 25, bold: true, fontScale: 0.4f);

        //TODO: add thunk sound

        // TODO: add particle
    }
}