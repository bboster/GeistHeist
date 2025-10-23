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

using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPromptInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string buttonText="E";

    private TMP_Text interactText;
    private ButtonPromptBillboardUI billboardUI;
    [HideInInspector] public UnityEvent ShowUIEvent = new();
    [HideInInspector] public UnityEvent HideUIEvent = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitializeFromBillboardUI(ButtonPromptBillboardUI buttonPrompt)
    {
        billboardUI = buttonPrompt;
        interactText = buttonPrompt.GetComponentInChildren<TMP_Text>();
    }

    void IInteractable.Interact()
    {/* do nothing */}

    // Call this with controller updates later.
    public void UpdateButtonText()
    {
        interactText.text = buttonText;
    }

    void IInteractable.DisplayInteractUI()
    {
        billboardUI.Show();
    }
    void IInteractable.HideInteractUI()
    {
        billboardUI.Hide();
    }
}
