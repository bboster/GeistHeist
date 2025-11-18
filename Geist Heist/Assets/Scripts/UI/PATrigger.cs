/*
 * Contributors:  Brenden
 * Creation Date: 11/17/25
 * Last Modified: 11/17/25
 * 
 * Brief Description: Used to start the text box of the PA system
 */
using System.Collections;
using UnityEngine;

public class PATrigger : MonoBehaviour
{
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            //play audio clip here joey
            alreadyTriggered = true;
            StartCoroutine(FillText());
        }
    }

    private IEnumerator FillText()
    {
        DailougeManager.Instance.PAholder.SetActive(true);
        int temp = 0;
        DailougeManager.Instance.PATextbox.text = "";
        while (DailougeManager.Instance.PATextbox.text.Length < Text.Length)
        {
            DailougeManager.Instance.PATextbox.text += Text.Substring(temp, 1);
            temp++;
            yield return new WaitForSeconds(.05f);
        }
        yield return new WaitForSeconds(stayLength); //this will be replaced with the end of the audio clip eventually
        clearBox();
    }

    //this is just in case we have to have it called somewhere else for the audio clip ending when that gets implemented
    private void clearBox()
    {
        DailougeManager.Instance.PAholder.SetActive(false);
    }
}
