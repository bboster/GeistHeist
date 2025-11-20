/*
 * Contributors:  Brenden
 * Creation Date: 10/28/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Used to start the text box of the dailogue system
 */
using System.Collections;
using UnityEngine;

public class DailougeBoxTrigger : MonoBehaviour
{
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            alreadyTriggered = true;
            DialougeManager.Instance.DisplayText_Dialogue(Text, stayLength);
        }
    }
}
