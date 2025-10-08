/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/07/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using UnityEngine;
using GuardUtilities;
using NaughtyAttributes;

public class VisionStimulus : Stimulus
{
    #region Variable Declarations

    [Header("Progamming")]
    [Tooltip("Controls whether or not certain variables are displayed")]
    [SerializeField] private bool showProgrammingValues;

    private bool hasSeenPlayer = false;

    [Tooltip("The index of the behavior to activate when the player is seen. WILL REPLACE WITH BETTER SYSTEM WHEN I THINK OF ONE")]
    [ShowIf("showProgrammingValues")]
    [SerializeField] private int behaviorIndex;
    [Tooltip("The index of the behavior to activate when the enemy loses track of the player during a chase.")]
    [ShowIf("showProgrammingValues")]
    [SerializeField] private int recoveryBehaviorIndex;
    [ShowIf("showProgrammingValues")]
    [SerializeField] private LayerMask raycastLayer;
    [ShowIf("showProgrammingValues")]
    [SerializeField] private Transform raycastSpawn;

    [ShowIf("showProgrammingValues")]
    [SerializeField] private GuardController parentController;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("3rd Person Player") && hasSeenPlayer == false)
        {
            Vector3 direction = -(raycastSpawn.position - other.gameObject.transform.position);
            float distance = Vector3.Distance(raycastSpawn.position, other.gameObject.transform.position) + 1;
            Physics.Raycast(raycastSpawn.position, direction, out RaycastHit info, distance, raycastLayer);
            //StartCoroutine(GuardDebug.PersistentRay(raycastSpawn.transform.position, direction, 10));

            if (info.collider != null && info.collider.gameObject.name.Equals("3rd Person Player"))
            {
                hasSeenPlayer = true;
                TriggerStimulus();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("3rd Person Player") && hasSeenPlayer == true)
        {
            hasSeenPlayer = false;
            parentController.OnVisionBroken();
        }
    }

    /// <summary>
    /// Sends the stimulus to the guard recieving it
    /// </summary>
    public override void TriggerStimulus()
    {
        parentController.RecieveStimulus(this, stateToChangeTo);
    }
}
