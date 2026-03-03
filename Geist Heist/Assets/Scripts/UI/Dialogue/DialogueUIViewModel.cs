using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUIViewModel : MonoBehaviour
{
    [SerializeField, Required] private TMP_Text textBox;
    [SerializeField, Required] private CanvasGroup group;

    [HideInInspector]
    public DialogueTextData textData;
    public IEnumerator InitializeTypewriterAnimation(DialogueTextData textData)
    {
        this.textData = textData;

        textBox.text = "";
        for (int i = 1; i < textData.BodyText.Length; i++)
        {
            textBox.text = textData.BodyText.Substring(0, i);
            yield return new WaitForSeconds(DialogueUIManager.Instance.secondsBetweenLetters);
        }

        // start destroy self coroutine, but dont wait for it to finish (to not offset other animations).
        StartCoroutine(WaitToDestroySelf());
    }

    public IEnumerator WaitToDestroySelf()
    {
        yield return new WaitForSeconds(textData.StayLength);
        yield return StaticUtilities.FadeToHidden(group, 0.5f);
        Destroy(this.gameObject);
    }
}
