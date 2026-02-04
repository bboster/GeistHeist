using UnityEngine;
/*
* Contributors: Brenden
* Creation Date: 2/4/26
* Last Modified: 2/4/26
* 
* Brief Description: to display a voiceline of a guard to be able to be read aswell as heard
*/

public class Speechbubble : IBillboardUI
{
    private CanvasGroup group;
    private GuardController guard;
    private bool talking;

    public override void OnInitialize(GameObject sourceGameObject)
    {
        group = GetComponent<CanvasGroup>();
        guard = sourceGameObject.GetComponent<GuardController>();
        guard.VoiceClipPlayed.AddListener(UpdateBubble);
        guard.VoiceClipStopped.AddListener(stopTalking);
        group.alpha = 0;
    }

    [SerializeField] TMPro.TMP_Text Textbox;
    public void UpdateBubble(string Caption)
    {
        talking = true;
        Textbox.text = Caption;
        CalculateOpacity(1, Vector3.zero);
    }

    public void stopTalking()
    {
        talking=false;
        CalculateOpacity(1, Vector3.zero);
    }

    protected override float CalculateOpacity(float playerDistance, Vector3 UIPosition)
    {
        if (talking)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
