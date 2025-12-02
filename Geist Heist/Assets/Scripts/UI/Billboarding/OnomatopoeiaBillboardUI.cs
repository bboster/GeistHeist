/*
 * Contributors: Toby
 * Creation Date: 12/2/2025
 * Last Modified: 12/2/2025
 * 
 * Brief Description: Billboarded ui that displays Onomatopoeia text.
 * Recyclable and can be used for all Onomatopoeias
 * 
 * Onomatopoeias get spawned in from BillboardUIManager when needed.
 */

using NaughtyAttributes;
using NaughtyAttributes.Test;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OnomatopoeiaBillboardUI : IBillboardUI
{
    [SerializeField, Required] TMP_Text textbox;

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

    // Basically an initalization function, called in BillboardUIManager after this is spawned in.
    public void SetTextProperties(string text, float scale, float randomRotationRange, 
                                  bool bold, bool italics)
    {
        // apply bold and/or italic tags (you can not set tmps bold attributes with code. its inspector only >:/
        //string styledText = $"{(bold?"<b>":"")}{(italics ? "<i>" : "")}{text}";

        textbox.text = text;

        // apply settings
        textbox.fontSize = textbox.fontSize * scale;
        textbox.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-randomRotationRange, randomRotationRange));
        
        // (Reset and) apply text stylings
        textbox.fontStyle = FontStyles.Normal;
        if(bold)
            textbox.fontStyle = FontStyles.Bold;
        if (italics)
            textbox.fontStyle = textbox.fontStyle | FontStyles.Italic;
    }


}
