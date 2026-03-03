/*
 * Contributors:  Brenden
 * Creation Date: 10/28/25
 * Last Modified: 11/17/25
 * 
 * Brief Description: Instantiates and keeps the textboxes and canvases of the
 * Dialogue and PA system
 */
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class DialogueUIManager : Singleton<DialogueUIManager>
{
    [SerializeField] private DialogueUIViewModel DialogueTextboxPrefab;
    [SerializeField] private DialogueUIViewModel PATextboxPrefab;
    [SerializeField] public float secondsBetweenLetters;
    [SerializeField] public RectTransform dialogueBubblesLayout;

    public void Initialize()
    {
    }

    public void DisplayText_Dialogue(List<DialogueTextData> dialogueText, UnityAction onDialogueEndCallback=null)
    {
        StartCoroutine(DisplayTextList(dialogueText, DialogueTextboxPrefab, onDialogueEndCallback: onDialogueEndCallback));
    }

    public void DisplayText_PASystem(List<DialogueTextData> dialogueText, UnityAction onDialogueEndCallback = null)
    {
        StartCoroutine(DisplayTextList(dialogueText, PATextboxPrefab, onDialogueEndCallback: onDialogueEndCallback));
    }

    private IEnumerator DisplayTextList(List<DialogueTextData> dialogueText, DialogueUIViewModel textboxPrefab , UnityAction onDialogueEndCallback = null)
    {
        for(int i = 0; i < dialogueText.Count; i++)
        {
            DialogueTextData textData = dialogueText[i];

            DialogueUIViewModel textBubble = Instantiate(textboxPrefab, dialogueBubblesLayout);

            // Wait for typewriter animation. DialogueViewModel knows when to destroy itself (dont wait for that)
            yield return textBubble.InitializeTypewriterAnimation(textData);
            yield return new WaitForSeconds(textData.SecondsDelayUntilNextDialogue);
        }

        if(onDialogueEndCallback != null)
            onDialogueEndCallback();
    }
}
