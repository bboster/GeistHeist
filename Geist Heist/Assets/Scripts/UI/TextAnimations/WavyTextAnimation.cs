using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class WavyTextAnimation : MonoBehaviour
{
    [SerializeField] private float waveSpeed = 1;
    [SerializeField] private float waveHeight = 1;
    public bool PlayAnimation = true;


    [SerializeField] public TMP_Text textBox;
    [SerializeField, Foldout("Advanced Settings")] private float disableAnimationSeconds = 0.1f;

    private string textString;

    private float t;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayAnimation) t = 1;

        textString = textBox.text;
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        while (true)
        {
            yield return null;

            if (PlayAnimation) t = Mathf.MoveTowards(t, 1, Time.unscaledDeltaTime / disableAnimationSeconds);
            else               t = Mathf.MoveTowards(t, 0, Time.unscaledDeltaTime / disableAnimationSeconds);


            if (t > 0)
            {
                string waveString = "";
                for (int i = 0; i < textString.Length; i++)
                {
                    char c = textString[i];
                    float height = StaticUtilities.SinRange((Time.unscaledTime + i) * waveSpeed, -waveHeight, waveHeight);
                    height = StaticUtilities.RoundToHundreth(height * t);
                    waveString += $"<voffset={height}em>{c}</voffset>";


                }
                textBox.text = waveString;
            }

            else
            {
                textBox.text = textString;
            }
        }
    }
}
