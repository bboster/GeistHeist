/*
 * Contributors:  Brenden
 * Creation Date: 10/28/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Used to start the text box of the dailogue system
 */
using System.Collections;
using UnityEngine;

public class DailougeBoxTrigger : MonoBehaviour
{
    [SerializeField] string Text;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] float stayLength;
    bool alreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ThirdPersonInputHandler>() != null && !alreadyTriggered)
        {
            alreadyTriggered = true;
            StartCoroutine(FillText());
        }
    }

    private IEnumerator FillText()
    {
        DailougeManager.Instance.ResizingTextbox.SetActive(true);
        int temp = 0;
        DailougeManager.Instance.Textbox.text = "";
        while (DailougeManager.Instance.Textbox.text.Length < Text.Length)
        {
            DailougeManager.Instance.Textbox.text += Text.Substring(temp, 1);
            temp++;
            yield return new WaitForSeconds(.05f);
        }
        yield return new WaitForSeconds(stayLength);
        DailougeManager.Instance.ResizingTextbox.SetActive(false);
    }
}
