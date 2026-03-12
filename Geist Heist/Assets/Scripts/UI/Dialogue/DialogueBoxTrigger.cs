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

    [SerializeField] private List<DialogueTextData> dialogueText = new();

    #region Deprecated
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private EventInstance voiceline;

    [Header("Advanced")]
    [SerializeField] private int whichLine;
    #endregion


    private void Start()
    {
        if (dialogueText.Count == 0)
        {
            Debug.Log($"Please update set the Dialogue text to the list in gameobject: {gameObject.name}");

            var temp = new DialogueTextData();
            temp.BodyText = Text;
            temp.StayLength = stayLength;
            temp.audioLine = 0;
            dialogueText.Add(temp);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if collided with player and not already triggered
        if(other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            alreadyTriggered = true;
            DialogueUIManager.Instance.DisplayText_PASystem(dialogueText);

            voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PALines);

            if (whichLine >= 0 && whichLine < 3)
            {
                RuntimeManager.StudioSystem.setParameterByName("PA", whichLine);
                voiceline.start();
            }
        }
    }
}
