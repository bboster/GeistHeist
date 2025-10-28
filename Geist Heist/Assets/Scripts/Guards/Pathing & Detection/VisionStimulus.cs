/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/07/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using System.Collections;
using UnityEngine;
using GuardUtilities;
using NaughtyAttributes;

public class VisionStimulus : Stimulus
{
    #region Variable Declarations

    private bool hasSeenPlayer = false;
    private bool playerObjectSeen = false;
    private Coroutine timer;

    [Header("Progamming")]
    [Tooltip("Controls whether or not certain variables are displayed")]
    [SerializeField] private bool showProgrammingValues;

    [Tooltip("The index of the behavior to activate when the player is seen. WILL REPLACE WITH BETTER SYSTEM WHEN I THINK OF ONE")]
    [ShowIf("showProgrammingValues")]
    [SerializeField] private int behaviorIndex;
    [Tooltip("The index of the behavior to activate when the enemy loses track of the player during a chase.")]
    [ShowIf("showProgrammingValues")]
    [SerializeField] private int recoveryBehaviorIndex;
    [ShowIf("showProgrammingValues")]
    [SerializeField] private float visionBreakTimer;
    [ShowIf("showProgrammingValues")]
    [SerializeField] private LayerMask raycastLayer;
    [ShowIf("showProgrammingValues")]
    [SerializeField] private Transform raycastSpawn;


    [ShowIf("showProgrammingValues")]
    [SerializeField] private GuardController parentController;

    #endregion

    private void Awake()
    {
        PossessableObject.OnActionPerformed += ActionDetected;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(timer != null)
        {
            StopCoroutine(timer);
            timer = null;
        }
        else if (other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == false)
            {
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
            else if(obj.Equals(PlayerManager.Instance.CurrentObject))
            {
                playerObjectSeen = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PossessableObject obj))
        {
            if (obj.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == true)
            {
                timer = StartCoroutine(VisionBreakTimer());
            }
            else if(obj.Equals(PlayerManager.Instance.CurrentObject))
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
        if(playerObjectSeen == true)
        {
            parentController.RecieveStimulus(this, stateToChangeTo);
        }
    }

    private void OnDisable()
    {
        PossessableObject.OnActionPerformed -= ActionDetected;
    }
}
