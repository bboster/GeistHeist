/*
 * Contributors:  Brenden, Toby
 * Creation Date: 11/17/2025
 * Last Modified:  3/ 4/2026
 * 
 * Brief Description: Used to start the text box of the PA system
 */
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script looks very similar to DialogueBoxTrigger, maybe try to combine these scripts into one?
public class PADialogueUITrigger : MonoBehaviour
{

    [SerializeField] private List<DialogueTextData> dialogueText = new();

    [InfoBox("'Text' is deprecated! please copy your text variables to the 'dialogueTest' list", EInfoBoxType.Warning)]

    [SerializeField, Foldout("Deprecated")] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField, Foldout("Deprecated")] float stayLength;
    bool alreadyTriggered;


    [SerializeField, Foldout("Deprecated")] private int whichLine;

    public currentLevel thisLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            alreadyTriggered = true;
            DialogueUIManager.Instance.DisplayText_PASystem(dialogueText, thisLevel);
        }
    }

    

    
}
