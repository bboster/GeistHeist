using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingTextAnimation : MonoBehaviour
{
    [SerializeField] private float persistSeconds = 1.75f;
    [SerializeField] private float secondsBetweenDots = 0.5f;
    [SerializeField] private string baseSavingText = "Loading";

    [SerializeField, Required] private TMP_Text loadingText;

    private int currentEllipses = 1;

    private const int MAX_ELLIPSES = 3; // ...
    private const int MIN_ELLIPSES = 1; // .

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EllipsesAnimation());
    }

    private IEnumerator EllipsesAnimation()
    {
        while(true)
        {
            // increase ellipses amount, loop around
            currentEllipses++;
            if (currentEllipses > MAX_ELLIPSES)
                currentEllipses = MIN_ELLIPSES;

            string ellipses = "";
            for (int _ = 0; _ < currentEllipses; _++)
                ellipses += ".";

            loadingText.text = baseSavingText + ellipses;

            yield return new WaitForSecondsRealtime(secondsBetweenDots);
        }
    }
}
