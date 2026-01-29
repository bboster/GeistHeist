/*
 * Contributors:  Brenden
 * Creation Date: 10/28/25
 * Last Modified: 11/17/25
 * 
 * Brief Description: Instantiates and keeps the textboxes and canvuses of the
 * Dialogue and PA system
 */
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialougeManager : Singleton<DialougeManager>
{
    [SerializeField] private GameObject DialogueTextboxPrefab;
    [SerializeField] private GameObject PAPrefab;
    [SerializeField] private float secondsBetweenLetters;

    private GameObject DialogueCanvas;
    private TMPro.TMP_Text DialogueTextbox;

    private GameObject PAholder;
    private TMPro.TMP_Text PATextbox;

    private Coroutine typingCoroutine;

    public void Initialize()
    {
        DialogueCanvas = Instantiate(DialogueTextboxPrefab);
        DialogueTextbox = DialogueCanvas.GetComponentInChildren<TMPro.TMP_Text>();

        PAholder = Instantiate(PAPrefab);
        PATextbox = PAholder.GetComponentInChildren<TMPro.TMP_Text>();

        DialogueCanvas.SetActive(false);
        PAholder.SetActive(false);
    }

    public void DisplayText_Dialogue(string text, float stayLength, UnityAction onDialogueEndCallback=null)
    {
        StaticUtilities.StopAndStartCoroutine(ref typingCoroutine, FillText(text, stayLength, DialogueTextbox, onDialogueEndCallback: onDialogueEndCallback));
    }

    public void DisplayText_PASystem(string text, float stayLength, UnityAction onDialogueEndCallback = null)
    {
        StaticUtilities.StopAndStartCoroutine(ref typingCoroutine, FillText(text, stayLength, PATextbox, onDialogueEndCallback: onDialogueEndCallback));
    }

    private IEnumerator FillText(string text, float stayLength, TMPro.TMP_Text textbox, UnityAction onDialogueEndCallback = null)
    {
        DialougeManager.Instance.PAholder.SetActive(true);
        int temp = 0;
        DialougeManager.Instance.PATextbox.text = "";
        while (DialougeManager.Instance.PATextbox.text.Length < text.Length)
        {
            DialougeManager.Instance.PATextbox.text += text.Substring(temp, 1);
            temp++;
            yield return new WaitForSeconds(secondsBetweenLetters);
        }
        yield return new WaitForSeconds(stayLength); //this will be replaced with the end of the audio clip eventually

        ClearBox();

        if(onDialogueEndCallback != null) 
            onDialogueEndCallback();    
    }

    //this is just in case we have to have it called somewhere else for the audio clip ending when that gets implemented
    private void ClearBox()
    {
        DialougeManager.Instance.PAholder.SetActive(false);
    }
}
