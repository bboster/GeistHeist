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
using System.Collections;
using TMPro;
using UnityEngine;

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
    public void SetTextProperties(string text, float fontScale, float randomRotationRange, float lifetime,
                                  bool bold, bool italics,
                                  bool animateRotation, bool animateScale)
    {
        // apply bold and/or italic tags (you can not set tmps bold attributes with code. its inspector only >:/
        //string styledText = $"{(bold?"<b>":"")}{(italics ? "<i>" : "")}{text}";

        textbox.text = text;

        // apply settings
        textbox.fontSize = textbox.fontSize * fontScale;

        if (animateRotation)
            StartCoroutine(RotateOverLifetime(lifetime, randomRotationRange));
        else
            textbox.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-randomRotationRange, randomRotationRange));

        if (animateScale)
            StartCoroutine(ScaleOverLifetime(lifetime/4, 0.25f, 1f));
        
        // (Reset and) apply text stylings
        textbox.fontStyle = FontStyles.Normal;
        if(bold)
            textbox.fontStyle = FontStyles.Bold;
        if (italics)
            textbox.fontStyle = textbox.fontStyle | FontStyles.Italic;
    }

    private IEnumerator RotateOverLifetime(float lifetime, float randomRotationRange)
    {
        float startRotation, endRotation = 0;
        do
        {   // Make sure that start rotation and end rotation are different enough that the animation is noticable
            startRotation = Random.Range(-randomRotationRange, randomRotationRange);
            endRotation = Random.Range(-randomRotationRange, randomRotationRange);
        }
        while (StaticUtilities.Difference(startRotation, endRotation) < randomRotationRange / 3);

        float timeStarted = Time.time;
        float timeElapsed = 0;
        while(timeElapsed < lifetime)
        {
            timeElapsed = Time.time - timeStarted;
            float t = timeElapsed / lifetime;
            t = Mathf.Pow(t, 0.25f); // big change at start, slows down

            float z = Mathf.LerpAngle(startRotation, endRotation, t);

            textbox.transform.localEulerAngles = new Vector3(0, 0, z);

            Debug.Log("angle: "+z);

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ScaleOverLifetime(float lifetime, float initialScale, float targetScale)
    {
        float timeStarted = Time.time;
        float timeElapsed = 0;
        while (timeElapsed < lifetime)
        {
            timeElapsed = Time.time - timeStarted;
            float t = timeElapsed / lifetime;
            //t = Mathf.Pow(t, 0.25f); // big change at start, slows down

            textbox.transform.localScale = Vector3.one * Mathf.Lerp(initialScale, targetScale, t);

            Debug.Log("scale t: " + t);

            yield return new WaitForEndOfFrame();
        }
    }
}
