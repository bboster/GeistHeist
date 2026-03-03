/*
 * Contributors:  Brenden, Toby
 * Creation Date: 11/17/25
 * Last Modified: 11/18/25
 * 
 * Brief Description: Used to start the text box of the PA system
 */
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

//This script looks very similar to DialogueBoxTrigger, maybe try to combine these scripts into one?
public class PATriggerDialogueUI : MonoBehaviour
{
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private EventInstance voiceline;

    [SerializeField] private int whichLine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            //play audio clip here joey
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PAJingle);

            voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PALines);

            if (whichLine >= 0 && whichLine < 3)
            {
                RuntimeManager.StudioSystem.setParameterByName("PA", whichLine);
                voiceline.start();
            }

            alreadyTriggered = true;
            DialogueUIManager.Instance.DisplayText_PASystem(Text, stayLength);
        }
    }

    

    
}
