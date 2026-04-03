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
    [SerializeField, Foldout("Advanced Settings")] private bool changeCharacterSpacing = true;
    [SerializeField, Foldout("Advanced Settings"), ShowIf(nameof(changeCharacterSpacing))] private float characterSpacingWhileWavy = 8;

    private string textString;
    private float defaultCharacterSpacing;

    private float t;

    private Coroutine anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayAnimation) t = 1;

        textString = textBox.text;
        defaultCharacterSpacing = textBox.characterSpacing;
    }

    void Update()
    {

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
            if(changeCharacterSpacing)
                textBox.characterSpacing = Mathf.Lerp(defaultCharacterSpacing, characterSpacingWhileWavy, t);
        }

        else
        {
            textBox.text = textString;
            if(changeCharacterSpacing)  
                textBox.characterSpacing = defaultCharacterSpacing;
        }
    }
}
