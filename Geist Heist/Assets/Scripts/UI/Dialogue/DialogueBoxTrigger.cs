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
using System.Collections.Generic;
using NaughtyAttributes;

public class DialogueBoxTrigger : MonoBehaviour
{
    [InfoBox("'Text' is deprecated! please copy your text variables to the 'dialogueTest' list", EInfoBoxType.Warning)]

    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private EventInstance voiceline;

    [Header("Advanced")]
    [SerializeField] private int whichLine;

    [SerializeField] private List<DialogueTextData> dialogueText;

    private void OnTriggerEnter(Collider other)
    {


        // if collided with player and not already triggered
        if(other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            alreadyTriggered = true;
            DialogueUIManager.Instance.DisplayText_Dialogue(Text, stayLength);

            voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PALines);

            if (whichLine >= 0 && whichLine < 3)
            {
                RuntimeManager.StudioSystem.setParameterByName("PA", whichLine);
                voiceline.start();
            }
        }
    }
}
