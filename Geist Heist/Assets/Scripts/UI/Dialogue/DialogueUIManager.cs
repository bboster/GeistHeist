/*
 * Contributors:  Brenden, Toby
 * Creation Date: 10/28/2025
 * Last Modified:  4/7/2026
 * 
 * Brief Description: Instantiates and keeps the textboxes and canvases of the
 * Dialogue and PA system
 */
using FMOD.Studio;
using FMODUnity;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    private EventInstance currentVL;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneUnloaded += StopVoiceLine;
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= StopVoiceLine;
    }

    public void Initialize()
    {
    }

    public void DisplayText_Dialogue(List<DialogueTextData> dialogueText, currentLevel currentLevel, 
        bool oldAudioSystem, EventReference eventReference, string parameter, 
        UnityAction onDialogueEndCallback = null)
    {
        StartCoroutine(DisplayTextList(dialogueText, currentLevel, oldAudioSystem, eventReference, parameter, onDialogueEndCallback: onDialogueEndCallback));

        if (relocateDialogueBubblesCoroutine == null)
            relocateDialogueBubblesCoroutine = StartCoroutine(UpdateDialogueBubbleLayout());
    }

    private IEnumerator DisplayTextList(List<DialogueTextData> dialogueText , currentLevel currentLevel,
        bool oldAudioSystem, EventReference eventReference, string parameter,
        UnityAction onDialogueEndCallback = null)
    {
        EventInstance voiceline;

        // this code is really dense, im sorry.

        for (int i = 0; i < dialogueText.Count; i++)
        {
            DialogueTextData textData = dialogueText[i];

            TryPlayVoiceLine(textData, currentLevel, oldAudioSystem, eventReference, parameter);

            var prefab = textData.dialogueSpeaker == DialogueSpeaker.PASystem ? PATextboxPrefab : DialogueTextboxPrefab;
            DialogueUIViewModel textBubble = Instantiate(prefab, dialogueBubblesLayout);
            textBubble.Initialize(textData);
            viewModels.Insert(0, textBubble);

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

    private void TryPlayVoiceLine(DialogueTextData textData, currentLevel thisLevel, bool oldAudioSystem, EventReference eventReference, string parameter)
    {
        if (textData.audioLine == -1) return;

        if( textData.overrideTextData)
        {
            currentVL = AudioManager.Instance.CreateEventInstance(textData.overrideAudioEventReference);
            RuntimeManager.StudioSystem.setParameterByName(textData.AudioParameterName, textData.overrideAudioLine);
            currentVL.start();
            return;
        }

        if (!oldAudioSystem)
        {
            currentVL = AudioManager.Instance.CreateEventInstance(eventReference);
            RuntimeManager.StudioSystem.setParameterByName(parameter, textData.audioLine);
            currentVL.start();
            return;
        }

        Debug.Log("playing audio with old dialogue audio system");

        //This needs more changes later when we add voicelines to remaining scenes
        if (thisLevel == currentLevel.Tutorial)
        {
            PlayVoiceLine(FMODEvents.Instance.VLTutorial, "VLTutorial", textData);
        }
        if (thisLevel == currentLevel.Parlor)
        {
            PlayVoiceLine(FMODEvents.Instance.VLParlor, "VLParlor", textData);
        }
        if (thisLevel == currentLevel.Gallery)
        {
            PlayVoiceLine(FMODEvents.Instance.VLGallery, "VLGallery", textData);
        }
        if (thisLevel == currentLevel.Pantry)
        {
            PlayVoiceLine(FMODEvents.Instance.VLPantry, "VLPantry", textData);
        }
        if (thisLevel == currentLevel.Canteen)
        {
            PlayVoiceLine(FMODEvents.Instance.VLCanteen, "VLCanteen", textData);
        }
        if (thisLevel == currentLevel.Bedroom)
        {
            PlayVoiceLine(FMODEvents.Instance.VLBedroom, "VLBedroom", textData);
        }
        if (thisLevel == currentLevel.Lobby)
        {
            int levelCount = SaveDataManager.Instance.GetLevelsCompletedCount();
            Debug.Log("levelCount: " + levelCount);
            switch (levelCount)
            {
                case 1:
                    PlayVoiceLine(FMODEvents.Instance.VLLobbyNightOne, "VLLobbyNightOne", textData);
                    break;
                case 2:
                    PlayVoiceLine(FMODEvents.Instance.VLLobbyNightTwo, "VLLobbyNightTwo", textData);
                    break;
                case 3:
                    PlayVoiceLine(FMODEvents.Instance.VLLobbyNightThree, "VLLobbyNightThree", textData);
                    break;
                case 4:
                    PlayVoiceLine(FMODEvents.Instance.VLLobbyNightFour, "VLLobbyNightFour", textData);
                    break;
                case 5:
                    PlayVoiceLine(FMODEvents.Instance.VLLobbyNightFive, "VLLobbyNightFive", textData);
                    break;
                default:
                    break;
            }
        }
    }
    
    private void PlayVoiceLine(EventReference eventInstance, string parameter, DialogueTextData textData)
    {
        currentVL = AudioManager.Instance.CreateEventInstance(eventInstance);
        RuntimeManager.StudioSystem.setParameterByName(parameter, textData.audioLine);
        currentVL.start();
    }

    public void StopVoiceLine()
    {
        currentVL.stop(STOP_MODE.IMMEDIATE);
    }

    public void StopVoiceLine(Scene scene)
    {
        currentVL.stop(STOP_MODE.IMMEDIATE);
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
            viewModels.RemoveAll(vm => vm == null);
            float totalHeight = 0;
            for (int i = 0; i < viewModels.Count; i++)
            {
                var dialogueBubble = viewModels[i];

                dialogueBubble.opacity = dialogueBubble.baseOpacity - (0.15f * (float)i);

                // manipulate y position
                Vector2 desiredPosition = new Vector2(0, totalHeight);
                dialogueBubble.rectTransform.anchoredPosition = Vector2.MoveTowards(dialogueBubble.rectTransform.anchoredPosition, desiredPosition, positionTransitionSpeed * Time.deltaTime);
                    
                totalHeight += dialogueBubble.rectTransform.rect.height + pixelsGapBetweenDialogueBubbles;
            }

            yield return null;
        }
    }
}


public enum currentLevel
{
    Tutorial,
    Lobby,
    Parlor
}
