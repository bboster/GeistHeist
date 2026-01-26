/*
 * Contributors:  Brenden
 * Creation Date: 10/28/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Used to start the text box of the dialogue system
 */
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DialogueBoxTrigger : MonoBehaviour
{
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private EventInstance voiceline;

    [SerializeField] private int whichLine;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            alreadyTriggered = true;
            DialogueManager.Instance.DisplayText_Dialogue(Text, stayLength);

            voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.PALines);

            if (whichLine >= 0 && whichLine < 3)
            {
                RuntimeManager.StudioSystem.setParameterByName("PA", whichLine);
                voiceline.start();
            }
        }
    }
}
