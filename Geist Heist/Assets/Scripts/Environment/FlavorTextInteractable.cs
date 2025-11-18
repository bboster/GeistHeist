/*
 * Contributors: Toby
 * Creation Date: 11/18/25
 * Last Modified: 11/18/25
 * 
 * Brief Description: When player interacts with flavor text, display some text, then ollie says something
 */

using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class FlavorTextInteractable : MonoBehaviour, IInteractable
{
    [SerializeField, ResizableTextArea] private string DisplayText = "";

    void Start()
    {

    }

    public void Interact()
    {
        DialougeManager.Instance.DisplayText_Dialogue(DisplayText, 3, onDialogueEndCallback:OnFlavorTextEnd);
        this.enabled = false; // cant interact with it anymore
    }

    private void OnFlavorTextEnd()
    {
        Debug.Log("ollie voice clip go here");
        //TODO: @Joe put sound effect here
    }

}
