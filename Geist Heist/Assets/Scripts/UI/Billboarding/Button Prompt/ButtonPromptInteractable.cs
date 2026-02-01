/*
 * Contributors: Toby
 * Creation Date: 10/23/25
 * Last Modified: 10/23/25
 * 
 * Brief Description: Interactable component that listens for player interaction.
 * Lets the ButtonPromptBillboardUI (different script) know whats going on.
 * 
 * May be on an empty transform childed from the object. Because of the way collisions work, it 
 * will still when the player looks at the relevant colldier.
 * 
 * TODO: swap UI for controller support eventually.
 */

using UnityEngine;
using UnityEngine.Events;

public class ButtonPromptInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] public string buttonText="E";

    private ButtonPromptBillboardUI billboardUI;
    [HideInInspector] public UnityEvent ShowUIEvent = new();
    [HideInInspector] public UnityEvent HideUIEvent = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitializeFromBillboardUI(ButtonPromptBillboardUI buttonPrompt)
    {
        billboardUI = buttonPrompt;
    }

    void IInteractable.Interact()
    {/* do nothing */}

    void IInteractable.OnPlayerLookStart()
    {
        /*int parentsDisabled =
            transform
            .GetComponentsInParent<IInteractable>() // parent's Interactables
            .Select(i => i.IsInteractable() == false) // filter by uninteractable
            .Count();// > 0; // overengineered but i love lambda so much
        Debug.Log($"{parentsDisabled} parents disabled");*/
        var parent_interactable = transform.GetComponentInParent < IInteractable> ();
        if(parent_interactable.IsInteractable() == false)
        {
            Debug.Log("Parent uninteractable");
            billboardUI.Hide();
            return;
        }

        // UpdateButtonPrompt changes the text depending on if its a controller / keyboard. 
        // This is redundant now but will be important later.
        billboardUI.UpdateButtonPrompt();
        billboardUI.Show();
    }
    void IInteractable.OnPlayerLookStop()
    {
        billboardUI.Hide();
    }
}
