/*
 * Contributors:  Brenden, Toby
 * Creation Date: 11/17/25
 * Last Modified: 11/18/25
 * 
 * Brief Description: Used to start the text box of the PA system
 */
using System.Collections;
using UnityEngine;

public class PATrigger : MonoBehaviour
{
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            //play audio clip here joey
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.PAJingle);

            alreadyTriggered = true;
            DialougeManager.Instance.DisplayText_PASystem(Text, stayLength);
        }
    }

    
}
