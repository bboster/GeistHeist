/*
 * Contributors: Toby
 * Creation Date: 11/18/25
 * Last Modified: 11/18/25
 * 
 * Brief Description: When player interacts with flavor text, display some text, then ollie says something.
 * Flavor Text can only be read once per save file.
 * Parameters allow text to be displayed based only on various conditions.
 */

using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class FlavorTextActionable : MonoBehaviour, IActionable
{
    [InfoBox("Flavor text can only be read once per save file. Reset your save file if you are debugging.")]
    [SerializeField, ResizableTextArea] private string DisplayText = "";
    [SerializeField] private float secondsUntilCloseText = 10;

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

    private Outline outline;
    private bool? cached_isActionable; // decide one time if it is actionable and never again (until scene is reloaded)
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        if (SaveDataManager.Instance.IsFlavorTextRead(DisplayText))
            DisableTextActionable();
    }

    public void Action()
    {
        DialogueUIManager.Instance.DisplayText_Dialogue(DisplayText, secondsUntilCloseText, onDialogueEndCallback: OnFlavorTextEnd);
        SaveDataManager.Instance.MarkFlavorTextAsRead(DisplayText, autoSave: true);
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

    public void EnableTextActionable()
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

    void IActionable.OnPlayerLookStart()
    {
    }

    bool IActionable.IsActionable()
    {
        // only decide actionability first time you look at the object. Like shroedingers cat.
        cached_isActionable = cached_isActionable ?? HasMetConditionsToAppear();

        if (SaveDataManager.Instance.IsFlavorTextRead(DisplayText))
            return false;

        return cached_isActionable.Value;
    }
}
