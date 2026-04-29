/*
 * Contributors:  Toby
 * Creation Date: 3/4/2026
 * Last Modified: 3/4/2026
 * 
 * Brief Description: Typewrites dialogue from PA and flavor text.
 * Manipulated by DialogueUIManager
 */

using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUIViewModel : MonoBehaviour
{
    [SerializeField, Required] private TMP_Text textBox;
    [SerializeField, Required] public CanvasGroup group;
    [SerializeField, Required] public Animator animator;
    [SerializeField] public float baseOpacity = 0.9f;

    [ReadOnly] public RectTransform rectTransform;

    [HideInInspector] public DialogueTextData textData;
    [HideInInspector] public float opacity;

    public void Initialize(DialogueTextData textData)
    {
        this.textData = textData;
        rectTransform = GetComponent<RectTransform>();
        textBox.text = "";
        opacity = baseOpacity;

        AnchorToBottomStretch(rectTransform);

        StaticUtilities.FadeOpacity(group, 0, baseOpacity, seconds: 0.25f);
        StartCoroutine(DelayEndingAnimation());
    }

    public IEnumerator TypewriterAnimation()
    {
        textBox.text = "";
        for (int i = 1; i < textData.BodyText.Length; i++)
        {
            textBox.text = textData.BodyText.Substring(0, i+1);
            yield return new WaitForSeconds(DialogueUIManager.Instance.secondsBetweenLetters);
        }

        // start destroy self coroutine, but dont wait for it to finish (to not offset other animations).
        StartCoroutine(WaitToDestroySelf());
    }

    public IEnumerator WaitToDestroySelf()
    {
        //yield return new WaitForSeconds(textData.StayLength);

        float timeStarted = Time.time;

        // first half of fadeout 
        while (Time.time - timeStarted < textData.StayLength - 1)
        {
            // baseOpacity gets updated in DialogueUIManager 
            group.alpha = Mathf.MoveTowards(group.alpha, opacity, Time.deltaTime / 10);
            yield return null;
        }

        animator.SetTrigger("Ending");

        // second half of fadeout 
        while (Time.time - timeStarted < textData.StayLength)
        {
            // baseOpacity gets updated in DialogueUIManager 
            group.alpha = Mathf.MoveTowards(group.alpha, opacity, Time.deltaTime / 10);
            yield return null;
        }

        yield return StaticUtilities.FadeToHidden(group, seconds: 0.4f);
        Destroy(this.gameObject);
    }

    private IEnumerator DelayEndingAnimation()
    {
        float secondsToTypeWrite = (float)textData.BodyText.Length * DialogueUIManager.Instance.secondsBetweenLetters;
        yield return new WaitForSeconds(secondsToTypeWrite + textData.StayLength);
    }

    private static void AnchorToBottomStretch(RectTransform rectTransform)
    {
        // stole this code from the internet -Toby.
        // making it 'static' wasnt taken from the internet. i did that because i live on the edge

        // Set anchors to bottom stretch
        // anchorMin (0, 0) is bottom-left
        // anchorMax (1, 0) is bottom-right
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);

        // Set offsets to zero for full stretch relative to the parent's bottom edge
        // offsetMin = (left, bottom), offsetMax = (-right, -top)
        rectTransform.offsetMin = new Vector2(0f, 0f);
        rectTransform.offsetMax = new Vector2(0f, 0f);

        // Optional: Set pivot to the bottom-center for intuitive positioning
        rectTransform.pivot = new Vector2(0.5f, 0f);

    }

}
