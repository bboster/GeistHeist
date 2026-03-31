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

    public enum currentLevel
    {
        Tutorial,
        Lobby,
        Parlor
    }

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
        if(other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            alreadyTriggered = true;
            DialogueUIManager.Instance.DisplayText_PASystem(dialogueText);

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
        }
    }
}
