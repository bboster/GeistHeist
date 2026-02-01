/*
 * Author: Jacob Bateman
 * Contributors: Joshua Kelly
 * Creation: 10/02/25
 * Last Edited: 12/05/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using FMOD;
using GuardUtilities;
using NaughtyAttributes;
using System.Collections;
using System.Net.NetworkInformation;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

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
    [SerializeField] private GameObject visionRenderer;
    [Foldout("Programming Values")]
    [SerializeField] private int rayCount;
    [Foldout("Programming Values")]
    [SerializeField] private Transform coneOrigin;
    [Foldout("Programming Values")]
    [SerializeField] private Transform coneForwardExtent;
    [Foldout("Programming Values")]
    [SerializeField] private LayerMask layer;

    [Foldout("Programming Values")]
    [SerializeField] private Light spotLight;
    [Foldout("Programming Values")]
    [SerializeField] private Collider visionCollider;

    [Foldout("Programming Values")]
    [SerializeField] private GuardController parentController;

    private Mesh visionMesh;
    private Mesh tempMesh;

    //TEMP DEBUG VARS
    private Vector3 sweeper;
    private float diameter;

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

    #region Trigger Functions

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PossessableObject obj)) //Checks if the object detected is the player
        {
            //if the detected object is the player and the enemy has not alredy seen the player, it executes this condition
            if (obj.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == false)
            {
                StopTimer();

                hasSeenPlayer = true;

                //Checks if the player can be seen and, if yes, triggers the visions stimulus detection on the guard
                if (!VisionCast(other.gameObject, out RaycastHit info))
                {
                    hasSeenPlayer = true;
                    TriggerStimulus();
                }

#if UNITY_EDITOR
                if (info.collider != null)
                    UnityEngine.Debug.Log(info.collider.gameObject.name);
#endif
            }
            else if (obj.Equals(PlayerManager.Instance.CurrentObject) && playerObjectSeen == false) //handles reentering the vision cone
            {
                StopTimer(); //Stops the timer that would transition the guard into vision break state

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

    #region Vision Cone Renderer

    /// <summary>
    /// Renders the deformable vision cone.
    /// </summary>
    private void VisionMesh()
    {
        if(visionMesh != null)
        {
            MeshCleanup();
        }

        visionMesh = new Mesh();
        visionMesh.name = "visualizerMesh";
        visionRenderer.GetComponent<MeshFilter>().mesh = visionMesh;

        //Calculates the angle of the vision cone
        Mesh coneMesh = GetComponent<MeshFilter>().mesh;

        //Gets the bounds of the cone mesh and uses it to determine various useful measurements
        Bounds coneBounds = GetComponent<MeshRenderer>().bounds;
        float coneHeight = Vector3.Distance(coneOrigin.position, coneForwardExtent.position);
        float coneRadius = coneBounds.size.x * 0.5f;
        float coneDiameter = coneBounds.size.x;
        diameter = coneDiameter; //REMOVE THIS LINE

        Vector3[] vertices = new Vector3[rayCount + 2]; //Sizes the vertices array to be the amount we need given our ray casts
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        //Sets the first vertice (the point of the triangle) to be where the cone starts at the guard
        vertices[0] = visionRenderer.transform.InverseTransformPoint(coneOrigin.position);

        Vector3 raySweep = ((-coneOrigin.transform.right * coneRadius) + (-coneOrigin.transform.forward * coneHeight)) + coneOrigin.position;
        raySweep.y = coneOrigin.position.y;
        
        int vIndex = 1;
        int tIndex = 0;

        for (int i = 0; i <= rayCount; i++) //Calculates the vertex positions
        {
            UnityEngine.Debug.DrawLine(coneOrigin.position, raySweep);

            Vector3 vertex;

            if (Physics.Linecast(coneOrigin.position, raySweep, out RaycastHit hit, layer))
            {
                vertex = visionRenderer.transform.InverseTransformPoint(hit.point);
            }
            else
            {
                raySweep.y = coneOrigin.position.y;
                vertex = visionRenderer.transform.InverseTransformPoint(raySweep);
            }

            vertices[vIndex] = vertex;

            //Defines the triangles given the vertex just created
            if (i > 0)
            {
                triangles[tIndex + 0] = 0;
                triangles[tIndex + 1] = vIndex - 1;
                triangles[tIndex + 2] = vIndex;

                tIndex += 3;
            }

            vIndex++;

            Vector3 p1 = (-coneForwardExtent.right * coneRadius) + coneForwardExtent.position;
            Vector3 p2 = (coneForwardExtent.right * coneRadius) + coneForwardExtent.position;
            Vector3 dir = p2 - p1;
            dir.y = coneOrigin.position.y;

            //Sweeps the raycast a given distance along the base of the triangular visualizer. NewPoint = OldPoint + distance * unit vector of the base
            raySweep = raySweep - (coneDiameter / rayCount) * Vector3.Normalize(-dir);
        }

        visionMesh.vertices = vertices;
        visionMesh.uv = uv;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateBounds();
    }

    /// <summary>
    /// Function to clean up the generated meshes to ensure 100% that they don't live in memory for too long
    /// </summary>
    private void MeshCleanup()
    {
        tempMesh = visionMesh;
        visionMesh = null;

        Destroy(tempMesh);
    }

    #endregion

    private void Update()
    {
        VisionMesh();
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
        direction.y = 0;
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
