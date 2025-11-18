/*
 * Author: Jacob Bateman
 * Contributors: Joshua Kelly
 * Creation: 10/02/25
 * Last Edited: 11/15/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using FMOD;
using GuardUtilities;
using NaughtyAttributes;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.ProBuilder.Shapes;

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

    private void Start()
    {
        GenerateVisionMesh();
    }

    private void OnValidate()
    {
        SyncLightToCollider();
    }

    #region Trigger Functions

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == false)
            {
                StopTimer();

                hasSeenPlayer = true;

                if (!VisionCast(other.gameObject, out RaycastHit info))
                {
                    hasSeenPlayer = true;
                    TriggerStimulus();
                }

#if UNITY_EDITOR
                if (info.collider != null)
                    Debug.Log(info.collider.gameObject.name);
#endif
            }
            else if (obj.Equals(PlayerManager.Instance.CurrentObject) && playerObjectSeen == false)
            {
                StopTimer();

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
                if (!VisionCast(other.gameObject))
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

    #endregion

    private void GenerateVisionMesh()
    {
        Mesh visionMesh = new Mesh();

        Vector3[] vertices = new Vector3[3];
        Vector2[] uv = new Vector2[3];
        int[] triangles = new int[3];

        Mesh coneMesh = GetComponent<Mesh>();

        float coneHeight = coneMesh.bounds.size.z * transform.localScale.z;
        float coneRadius = Mathf.Max(coneMesh.bounds.size.x, coneMesh.bounds.size.y) * 0.5f * transform.localScale.x;

        vertices[0] = coneMesh.vertices[0];
        vertices[1] = new Vector3(vertices[0].x + coneRadius, 0, vertices[0].z + coneHeight);
        vertices[1] = new Vector3(vertices[0].x - coneRadius, 0, vertices[0].z + coneHeight);

        visionMesh.vertices = vertices;
        visionMesh.uv = uv;
        visionMesh.triangles = triangles;
    }

    #region Vision Raycast

    /// <summary>
    /// Handles the raycast to detect whether or not the target can be seen
    /// </summary>
    /// <returns></returns>
    private bool VisionCast(GameObject target)
    {
        //Starts the raycast at the raycast spawn location of the guard
        Vector3 spawnLocation = new Vector3(raycastSpawn.position.x, target.transform.position.y, raycastSpawn.position.z);

        //Calculates the direction pointing toward the seen object
        Vector3 direction = -(spawnLocation - target.transform.position);
        float distance = Vector3.Distance(raycastSpawn.position, target.transform.position) + 2; //Calculates the distance to raycast

        return Physics.Raycast(spawnLocation, direction, out RaycastHit info, distance, raycastLayer); ;
    }

    /// <summary>
    /// Handles the raycast to detect whether or not the target can be seen
    /// </summary>
    /// <returns></returns>
    private bool VisionCast(GameObject target, out RaycastHit hit)
    {
        //Starts the raycast at the raycast spawn location of the guard
        Vector3 spawnLocation = new Vector3(raycastSpawn.position.x, target.transform.position.y, raycastSpawn.position.z);

        //Calculates the direction pointing toward the seen object
        Vector3 direction = -(spawnLocation - target.transform.position);
        float distance = Vector3.Distance(raycastSpawn.position, target.transform.position) + 2; //Calculates the distance to raycast

        bool targetHit = Physics.Raycast(spawnLocation, direction, out RaycastHit info, distance, raycastLayer);
        hit = info;

        return targetHit;
    }

    #endregion

    /// <summary>
    /// Stops the vision break timer if running
    /// </summary>
    private void StopTimer()
    {
        if (timer != null)
        {
            StopCoroutine(timer);
            timer = null;
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
            UnityEngine.Debug.LogWarning("No spotlight assigned on VisionStimulus.");
            return;
        }

        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            UnityEngine.Debug.LogWarning("No collider found on VisionStimulus.");
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

            UnityEngine.Debug.Log($"[VisionStimulus] Synced spotlight from MeshCollider -> Range: {spotLight.range}, Angle: {spotLight.spotAngle}");
        }
    }
}
