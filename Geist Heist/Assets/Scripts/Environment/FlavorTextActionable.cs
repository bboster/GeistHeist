/*
 * Contributors: Toby
 * Creation Date: 11/18/2025
 * Last Modified:  3/ 4/2026
 * 
 * Brief Description: When player interacts with flavor text, display some text, then ollie says something.
 * Flavor Text can only be read once per save file.
 * Parameters allow text to be displayed based only on various conditions.
 */

using FMODUnity;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class FlavorTextActionable : MonoBehaviour, IInteractable
{
    [SerializeField] private List<DialogueTextData> flavorText = new();

    // IMPLEMENT THIS AFTER FUSE
    [SerializeField] private bool useOldAudioSystem = true;
    [HideIf(nameof(useOldAudioSystem)), SerializeField] private EventReference audioEventReference;
    [HideIf(nameof(useOldAudioSystem)), SerializeField] private string AudioParameterName;

    [InfoBox("'Text' is deprecated! please copy your text variables to the 'dialogueTest' list", EInfoBoxType.Warning)]

    [Space(10)]

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


    [InfoBox("Flavor text can only be read once per save file. Reset your save file if you are debugging.")]
    [SerializeField, ResizableTextArea, Foldout("Deprecated")] private string DisplayText = "";
    [SerializeField, Foldout("Deprecated")] private float secondsUntilCloseText = 10;

    private Outline outline;
    private bool? cached_isActionable; // decide one time if it is actionable and never again (until scene is reloaded)

    public currentLevel thisLevel;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
        else
            Debug.LogError($"{gameObject.name} does not have an outline component");

        if (flavorText.Count == 0)
        {
            Debug.Log($"Please update set the Dialogue text to the list in gameobject: {gameObject.name}");

            var temp = new DialogueTextData();
            temp.BodyText = DisplayText;
            temp.StayLength = 8;
            temp.audioLine = 0;
            flavorText.Add(temp);
        }

        if (SaveDataManager.Instance.IsFlavorTextRead(flavorText))
            DisableTextActionable();
    }

    public void Interact()
    {
        DialogueUIManager.Instance.DisplayText_Dialogue(flavorText, thisLevel, 
            useOldAudioSystem, audioEventReference, AudioParameterName,
            onDialogueEndCallback: OnFlavorTextEnd);
        SaveDataManager.Instance.MarkFlavorTextAsRead(flavorText, autoSave: true);
        DisableTextActionable();
    }

    private void OnFlavorTextEnd()
    {
        Debug.Log("ollie voice clip go here");
        //TODO: @Joe put sound effect here
    }

    public void DisableTextActionable()
    {
        this.enabled = false; // cant interact with it anymore
        outline.enabled = false;
    }

    public void EnableTextInteractable()
    {
        this.enabled = true;
        // keep outline disabled tho
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

    void IInteractable.OnPlayerLookStart()
    {
    }

    bool IInteractable.IsInteractable()
    {
        // only decide actionability first time you look at the object. Like shroedingers cat.
        cached_isActionable = cached_isActionable ?? HasMetConditionsToAppear();

        if (SaveDataManager.Instance.IsFlavorTextRead(flavorText))
            return false;

        return cached_isActionable.Value;
    }
}
