/*
 * Author: Jacob Bateman
 * Contributors: Joshua Kelly
 * Creation: 10/02/25
 * Last Edited: 10/28/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using System.Collections;
using GuardUtilities;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class VisionStimulus : Stimulus
{
    #region Variable Declarations

    private bool hasSeenPlayer = false;
    private bool playerObjectSeen = false;
    private Coroutine timer;

    [Tooltip("The index of the behavior to activate when the player is seen. WILL REPLACE WITH BETTER SYSTEM WHEN I THINK OF ONE")]
    [Foldout("Programming Values")]
    [SerializeField] private int behaviorIndex;
    [Tooltip("The index of the behavior to activate when the enemy loses track of the player during a chase.")]
    [Foldout("Programming Values")]
    [SerializeField] private int recoveryBehaviorIndex;
    [Foldout("Programming Values")]
    [SerializeField] private float visionBreakTimer;
    [Foldout("Programming Values")]
    [SerializeField] private LayerMask raycastLayer;
    [Foldout("Programming Values")]
    [SerializeField] private Transform raycastSpawn;

    [Foldout("Programming Values")]
    [SerializeField] private Light spotLight;
    [Foldout("Programming Values")]
    [SerializeField] private Collider visionCollider;

    [Foldout("Programming Values")]
    [SerializeField] private GuardController parentController;

    #endregion

    private void Awake()
    {
        PossessableObject.OnActionPerformed += ActionDetected;
        PossessableObject.OnObjectLeft += ObjectLeft;
    }

    private void OnValidate()
    {
        SyncLightToCollider();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == false)
            {
                if (timer != null)
                {
                    StopCoroutine(timer);
                    timer = null;
                }

                hasSeenPlayer = true;
                Vector3 spawnLocation = new Vector3(raycastSpawn.position.x, other.gameObject.transform.position.y, raycastSpawn.position.z);

                Vector3 direction = -(spawnLocation - other.gameObject.transform.position);
                float distance = Vector3.Distance(raycastSpawn.position, other.gameObject.transform.position) + 2;

                if (!Physics.Raycast(spawnLocation, direction, out RaycastHit info, distance, raycastLayer))
                {
                    hasSeenPlayer = true;
                    TriggerStimulus();
                }

                if (info.collider != null)
                    Debug.Log(info.collider.gameObject.name);
            }
            else if (obj.Equals(PlayerManager.Instance.CurrentObject) && playerObjectSeen == false)
            {
                if (timer != null)
                {
                    StopCoroutine(timer);
                    timer = null;
                }

                if (obj.gameObject.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 velocityCheck = rb.linearVelocity.Abs();

                    //If there's a better way to check if a possessable is moving please leave a note in the review
                    if (velocityCheck.x > 1 || velocityCheck.y > 1 || velocityCheck.z > 1)
                    {
                        if (timer != null)
                        {
                            StopCoroutine(timer);
                            timer = null;
                        }

                        TriggerStimulus();
                        return;
                    }
                }

                playerObjectSeen = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == true)
            {
                Vector3 spawnLocation = new Vector3(raycastSpawn.position.x, other.gameObject.transform.position.y, raycastSpawn.position.z);

                Vector3 direction = -(spawnLocation - other.gameObject.transform.position);
                float distance = Vector3.Distance(raycastSpawn.position, other.gameObject.transform.position) + 2;

                if (!Physics.Raycast(spawnLocation, direction, out RaycastHit info, distance, raycastLayer))
                {
                    timer = StartCoroutine(VisionBreakTimer());
                }
            }
            else if (obj.Equals(PlayerManager.Instance.CurrentObject))
            {
                playerObjectSeen = false;
            }
        }
    }

    /// <summary>
    /// Controls how long the player must be out of the vision cone before it enters search state
    /// </summary>
    /// <returns></returns>
    private IEnumerator VisionBreakTimer()
    {
        yield return new WaitForSeconds(visionBreakTimer);

        hasSeenPlayer = false;
        parentController.OnVisionBroken();
        timer = null;
    }

    /// <summary>
    /// Sends the stimulus to the guard recieving it
    /// </summary>
    public override void TriggerStimulus()
    {
        parentController.RecieveStimulus(this, stateToChangeTo);
    }

    private void ActionDetected()
    {
        if (playerObjectSeen == true)
        {
            if (timer != null)
            {
                StopCoroutine(timer);
                timer = null;
            }

            parentController.RecieveStimulus(this, stateToChangeTo);
        }
    }

    private void ObjectLeft()
    {
        playerObjectSeen = false;
    }

    private void OnDisable()
    {
        PossessableObject.OnActionPerformed -= ActionDetected;
        PossessableObject.OnObjectLeft -= ObjectLeft;
    }

    [Button("Sync Light to Collider")]
    private void SyncLightToCollider()
    {
        if (spotLight == null)
        {
            Debug.LogWarning("No spotlight assigned on VisionStimulus.");
            return;
        }

        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("No collider found on VisionStimulus.");
            return;
        }

        if (col is MeshCollider meshCol && meshCol.sharedMesh != null)
        {
            Bounds bounds = meshCol.sharedMesh.bounds;

            float coneHeight = bounds.size.z * transform.localScale.z;
            float coneRadius = Mathf.Max(bounds.size.x, bounds.size.y) * 0.5f * transform.localScale.x;

            spotLight.range = coneHeight * 5;
            spotLight.spotAngle = Mathf.Rad2Deg * Mathf.Atan(coneRadius / coneHeight) * 2f;
            spotLight.innerSpotAngle = spotLight.spotAngle * 0.8f;

            Debug.Log($"[VisionStimulus] Synced spotlight from MeshCollider -> Range: {spotLight.range}, Angle: {spotLight.spotAngle}");
        }
    }
}
