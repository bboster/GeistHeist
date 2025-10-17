/*
 * Author: Jacob Bateman, Toby
 * Contributors:
 * Creation: 10/04/25
 * Last Edited: 10/16/25
 * Summary: Changes text for the temporary state visualizer.
 * Is billboarded to always face the player
 */

using GuardUtilities;
using TMPro;
using UnityEngine;

public class StateText : IBillboardUI
{
    [SerializeField] private TextMeshProUGUI stateText;

    public override void OnInitialize(GameObject sourceGameObject)
    {
        var guard = sourceGameObject.GetComponent<GuardController>();
        guard.OnBehaviorStarted.AddListener(ChangeText);
    }

    /// <summary>
    /// Runs temporary state visualizer
    /// </summary>
    /// <param name="state"></param>
    public void ChangeText(GuardStates state)
    {
        switch(state)
        {
            case GuardStates.none:
                stateText.text = "NONE";
                break;
            case GuardStates.patrol:
                stateText.text = "PATROL";
                break;
            case GuardStates.chase:
                stateText.text = "CHASE";
                break;
            case GuardStates.attack:
                stateText.text = "ATTACK";
                break;
            case GuardStates.surprised:
                stateText.text = "SURPRISED";
                break;
            case GuardStates.search:
                stateText.text = "SEARCH";
                break;
            case GuardStates.visionBreak:
                stateText.text = "VISION BREAK";
                break;
            case GuardStates.returnToPath:
                stateText.text = "RETURN";
                break;
        }
    }
}
