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

    private EventInstance voiceline;

    [SerializeField, Foldout("Deprecated")] private int whichLine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            string currentParameter = "";
            string currentSceneName = SceneManager.GetActiveScene().name;

            //This needs more changes later when we add voicelines to remaining scenes
            if (currentSceneName == "TutorialHallway")
            {
                currentParameter = "VLTutorial";
                voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLTutorial);
            }
            if (currentSceneName == "FINAL Parlor Room")
            {
                currentParameter = "VLParlor";
                voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLParlor);
            }
            if (currentSceneName == "HubV2.5")
            {
                if (SaveDataManager.Instance.IsLevelCompleted("TutorialHallway") && !SaveDataManager.Instance.IsLevelCompleted("FINAL Parlor Room"))
                {
                    currentParameter = "VLLobbyNightOne";
                    voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLLobbyNightOne);
                }
                if (SaveDataManager.Instance.IsLevelCompleted("FINAL Parlor Room") && !SaveDataManager.Instance.IsLevelCompleted("Exhibit 1 Wing 2 - Library"))
                {
                    currentParameter = "VLLobbyNightOne";
                    voiceline = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.VLLobbyNightOne);
                }
            }

            if (whichLine >= 0 && whichLine < 9)
            {
                RuntimeManager.StudioSystem.setParameterByName(currentParameter, whichLine);
                voiceline.start();
            }

            alreadyTriggered = true;
            DialogueUIManager.Instance.DisplayText_PASystem(dialogueText);
        }
    }

    

    
}
