using System.Collections;
using UnityEngine;

public class DailougeBoxTrigger : MonoBehaviour
{
    [SerializeField] string Text;
    [SerializeField] GameObject ResizingTextbox;
    [SerializeField] TMPro.TMP_Text Textbox;
    [Tooltip("How long the full text will stay on the screen")]
    [SerializeField] int stayLength;
    bool alreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !alreadyTriggered)
        {
            alreadyTriggered = true;
            StartCoroutine(FillText());
        }
    }

    private IEnumerator FillText()
    {
        ResizingTextbox.SetActive(true);
        int temp = 0;
        Textbox.text = "";
        while (Textbox.text.Length < Text.Length)
        {
            Textbox.text += Text.Substring(temp, 1);
            temp++;
            yield return new WaitForSeconds(.05f);
        }
        yield return new WaitForSeconds(stayLength);
        ResizingTextbox.SetActive(false);
    }
}
