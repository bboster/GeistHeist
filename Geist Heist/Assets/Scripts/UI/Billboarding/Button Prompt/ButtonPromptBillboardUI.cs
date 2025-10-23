/*
 * Contributors: Toby
 * Creation Date: 10/23/25
 * Last Modified: 10/23/25
 * 
 * Brief Description: billboarded. Appears when the player can interact with it.
 * Childed under billboard UI manager.
 * Gets shown / hidden when the player is looking at the base gameobject.
 */

using UnityEngine;
using UnityEngine.Events;

public class ButtonPromptBillboardUI : IBillboardUI
{
    /// <summary>
    /// Called when this objects is initialized
    /// </summary>
    /// <param name="sourceGameObject"></param>
    public override void OnInitialize(GameObject sourceGameObject)
    {
        sourceGameObject.GetComponentInChildren<ButtonPromptInteractable>().InitializeFromBillboardUI(this);
    }

    
}
