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

public class ButtonPromptInteractable : MonoBehaviour, IInteractable, IActionable
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

        var parent_interactable = transform.GetComponentInParent < IInteractable > ();
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
        Debug.Log("stopped looking");
        billboardUI.Hide();
    }

    void IActionable.OnPlayerLookStart()
    {
        var parent_actionable = transform.GetComponentInParent<IActionable>();
        if (parent_actionable.IsActionable() == false)
        {
            Debug.Log("Parent unactionable");
            billboardUI.Hide();
            return;
        }

        // UpdateButtonPrompt changes the text depending on if its a controller / keyboard. 
        // This is redundant now but will be important later.
        billboardUI.UpdateButtonPrompt();
        billboardUI.Show();
    }

    void IActionable.OnPlayerLookStop()
    {
        billboardUI.Hide();
    }

    public void Action()
    {/* do nothing */}
    public bool IsParentInteractable()
    {
        var parent_interactable = transform.parent.GetComponent<IInteractable>();
        var parent_actionable   = transform.parent.GetComponent<IActionable>();

        if (parent_interactable != null && parent_actionable != null)
            return parent_interactable.IsInteractable() && parent_actionable.IsActionable();

        if(parent_interactable != null)
            return parent_interactable.IsInteractable();

        if (parent_actionable != null)
            return parent_actionable.IsActionable();

        Debug.LogWarning($"{gameObject.name}'s parent does not have an interactable or actionable component");
        return false;
    }
}
