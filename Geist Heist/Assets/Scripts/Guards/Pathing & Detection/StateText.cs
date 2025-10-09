/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/04/25
 * Last Edited: 10/04/25
 * Summary: Changes text for the temporary state visualizer.
 */

using GuardUtilities;
using TMPro;
using UnityEngine;

public class StateText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText;

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
