/*
 * Contributors: Toby
 * Creation Date: 12/2/2025
 * Last Modified: 12/2/2025
 * 
 * Brief Description: Billboarded ui with no special functionality
 */

using NaughtyAttributes;
using NaughtyAttributes.Test;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BasicBillboardUI : IBillboardUI
{
    /// <summary>
    /// Called when this objects is initialized
    /// </summary>
    /// <param name="sourceGameObject"></param>
    public override void OnInitialize(GameObject sourceGameObject)
    {
    }

    public override void Show()
    {
        base.Show();
    }


}
