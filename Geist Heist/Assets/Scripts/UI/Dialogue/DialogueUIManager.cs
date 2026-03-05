/*
 * Contributors:  Brenden, Toby
 * Creation Date: 10/28/2025
 * Last Modified:  3/ 4/2026
 * 
 * Brief Description: Instantiates and keeps the textboxes and canvases of the
 * Dialogue and PA system
 */
using FMOD.Studio;
using FMODUnity;
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
    [SerializeField] public float secondsBetweenLetters = 0.1f;
    [SerializeField] public float pixelsGapBetweenDialogueBubbles = 10; // this variable name is like a sentence bruh
    [SerializeField] public float positionTransitionSpeed = 100; 
    [SerializeField] public RectTransform dialogueBubblesLayout;
    [SerializeField] private Canvas DialogueCanvas;

    private List<DialogueUIViewModel> viewModels = new();

    private Coroutine relocateDialogueBubblesCoroutine;
    public void Initialize()
    {
    }

    public void DisplayText_Dialogue(List<DialogueTextData> dialogueText, UnityAction onDialogueEndCallback=null)
    {
        StartCoroutine(DisplayTextList(dialogueText, false, onDialogueEndCallback: onDialogueEndCallback));

        if (relocateDialogueBubblesCoroutine == null)
            relocateDialogueBubblesCoroutine = StartCoroutine(UpdateDialogueBubbleLayout());
    }

    public void DisplayText_PASystem(List<DialogueTextData> dialogueText, UnityAction onDialogueEndCallback = null)
    {
        StartCoroutine(DisplayTextList(dialogueText, true, onDialogueEndCallback: onDialogueEndCallback));

        if (relocateDialogueBubblesCoroutine == null)
            relocateDialogueBubblesCoroutine = StartCoroutine(UpdateDialogueBubbleLayout());
    }

    private IEnumerator DisplayTextList(List<DialogueTextData> dialogueText, bool isPASystem , UnityAction onDialogueEndCallback = null)
    {
        EventInstance voiceline;

        if(isPASystem)
        {
            //play audio clip here joey
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PAJingle);
            yield return new WaitForSeconds(0.4f); // idk how long the PAJingle is, theres no way to get it either
        }

        // this code is really dense, im sorry.

        for (int i = 0; i < dialogueText.Count; i++)
        {
            DialogueTextData textData = dialogueText[i];

            var prefab = isPASystem ? PATextboxPrefab : DialogueTextboxPrefab;
            DialogueUIViewModel textBubble = Instantiate(prefab, dialogueBubblesLayout);
            textBubble.Initialize(textData);
            viewModels.Insert(0, textBubble);

            if (textData.audioLine != -1)
            {
                Debug.Log($"Playing audio clip for: {textData.BodyText}");
                string paramField = isPASystem ? "PA" : "JOEY PUT PARAM NAME HERE PLS :3";
                RuntimeManager.StudioSystem.setParameterByName(paramField, textData.audioLine);
                voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PALines);
                voiceline.start();
            }
            else Debug.LogWarning("No audio clip for dialogue: " + textData.BodyText);

            // Wait for typewriter animation. DialogueViewModel knows when to destroy itself (dont wait for that)
            float timeTypewriterStarted = Time.time;
            yield return textBubble.TypewriterAnimation();

            // if theres a voice line, wait for it to finish playing (accounting for time elapsed from typewriter)
            // cant do a null check because EventInstances cant be null for whatever reason 
            if (textData.audioLine != -1)
                yield return new WaitForSeconds(Time.time - timeTypewriterStarted - textData.SecondsDelayUntilNextDialogue);

            yield return new WaitForSeconds(textData.SecondsDelayUntilNextDialogue);
        }

        if(onDialogueEndCallback != null)
            onDialogueEndCallback();
    }

    private IEnumerator UpdateDialogueBubbleLayout()
    {
        // shitty solution to avoid race conditions:
        while (viewModels.Count <= 0)
        {
            Debug.Log("viewModels: "+viewModels.Count);
            yield return null;
        }

        while (viewModels.Count > 0)
        {
            Debug.Log("viewModels: " + viewModels.Count);
            viewModels.RemoveAll(vm => vm == null);
            float totalHeight = 0;
            for (int i = 0; i < viewModels.Count; i++)
            {
                var dialogueBubble = viewModels[i];

                dialogueBubble.opacity = dialogueBubble.baseOpacity - (0.15f * (float)i);

                // manipulate y position
                Vector2 desiredPosition = new Vector2(0, totalHeight);
                Debug.Log(desiredPosition);
                dialogueBubble.rectTransform.anchoredPosition = Vector2.MoveTowards(dialogueBubble.rectTransform.anchoredPosition, desiredPosition, positionTransitionSpeed * Time.deltaTime);
                    
                totalHeight += dialogueBubble.rectTransform.rect.height + pixelsGapBetweenDialogueBubbles;
            }

            yield return null;
        }
    }
}
