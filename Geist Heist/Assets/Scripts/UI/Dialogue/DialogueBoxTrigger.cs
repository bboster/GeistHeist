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
using UnityEngine.SceneManagement;

public class DialogueBoxTrigger : MonoBehaviour
{
    [SerializeField, AllowNesting] private List<DialogueTextData> dialogueText = new();

    // guys i went REALLY overboard but i am having so much fun
    [Header("Conditions to appear:")]
    [SerializeField] private bool AlwaysAppear = false;
    [InfoBox("If conditions are left blank/default, then flavor text can always appear")]
    [Tooltip("0: never appears, 1: appears every time")]
    [SerializeField, Range(0, 1), HideIf(nameof(AlwaysAppear))] private float chanceToAppear = 1;
    [Tooltip("Leave list empty to make it so player can see flavor text without completing any levels")]
    [SerializeField, Scene, HideIf(nameof(AlwaysAppear))] private string[] requiredScenesCompleted;
    [Tooltip("Require player to not have experienced a certain level to display")]
    [SerializeField, Scene, HideIf(nameof(AlwaysAppear))] private string[] requiredScenesNotCompleted;
    [Tooltip("Leave list empty to make it so player can see flavor text without collecting anything")]
    [SerializeField, HideIf(nameof(AlwaysAppear))] private Collectable[] requiredCollectables;
    [Tooltip("Require player to not collected certain collectables")]
    [SerializeField, HideIf(nameof(AlwaysAppear))] private Collectable[] requiredCollectablesUncollected;
    [Tooltip("If true, requires a specific hat to be worn")]
    [SerializeField, HideIf(nameof(AlwaysAppear))] private bool RequireSpecificHat = false;
    [SerializeField, ShowIf(nameof(RequireSpecificHat)), HideIf(nameof(AlwaysAppear))] private Collectable requiredHat;

    [Header("Audio")]
    // IMPLEMENT THIS AFTER FUSE
    [SerializeField] private bool useOldAudioSystem = true;
    [HideIf(nameof(useOldAudioSystem)), SerializeField] private EventReference audioEventReference;
    [HideIf(nameof(useOldAudioSystem)), SerializeField] private string AudioParameterName;

    [InfoBox("'Text' is deprecated! please copy your text variables to the 'dialogueTest' list", EInfoBoxType.Warning)]

    #region Deprecated
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private EventInstance voiceline;

    [Header("Advanced")]
    [SerializeField] private int whichLine;
    #endregion

    public currentLevel thisLevel;

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
        if(other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered && HasMetConditionsToAppear())
        {
            alreadyTriggered = true;
            DialogueUIManager.Instance.DisplayText_Dialogue(dialogueText, thisLevel, useOldAudioSystem, audioEventReference, AudioParameterName);

            /*
            string currentParameter = "";

            //This needs more changes later when we add voicelines to remaining scenes
            if (thisLevel == currentLevel.Tutorial)
            {
                currentParameter = "VLTutorial";
                voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLTutorial);
            }
            if (thisLevel == currentLevel.Parlor)
            {
                currentParameter = "VLParlor";
                voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLParlor);
            }
            if (thisLevel == currentLevel.Lobby)
            {
                int levelCount = SaveDataManager.Instance.GetLevelsCompletedCount();
                Debug.Log("levelCount: " + levelCount);
                switch (levelCount)
                {
                    case 2:
                        currentParameter = "VLLobbyNightOne";
                        voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLLobbyNightOne);
                        break;
                    case 3:
                        currentParameter = "VLLobbyNightTwo";
                        voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLLobbyNightTwo);
                        break;
                    default:
                        break;
                }
            }

            if (whichLine >= 0 && whichLine < 9)
            {
                RuntimeManager.StudioSystem.setParameterByName(currentParameter, whichLine);
                voiceline.start();
            }
            */
        }
    }

    /// <summary>
    /// If random chance is met, levels have been completed, and other conditions.
    /// </summary>
    /// <returns>True if flavor text can be read</returns>
    private bool HasMetConditionsToAppear()
    {
        if (AlwaysAppear)
            return true;

        if (Random.value > chanceToAppear)
            return false;

        foreach (var scene in requiredScenesCompleted)
        {
            if (SaveDataManager.Instance.IsLevelCompleted(scene) == false)
                return false;
        }

        foreach (var scene in requiredScenesNotCompleted)
        {
            if (SaveDataManager.Instance.IsLevelCompleted(scene) == true)
                return false;
        }

        foreach (var collectable in requiredCollectables)
        {
            if (SaveDataManager.Instance.IsCollectableCollected(collectable) == false)
                return false;
        }

        foreach (var collectable in requiredCollectablesUncollected)
        {
            if (SaveDataManager.Instance.IsCollectableCollected(collectable) == true)
                return false;
        }

        if (RequireSpecificHat && SaveDataManager.Instance.IsHatEqupped(requiredHat) == false)
            return false;

        return true;
    }
}
