/*
 * Contributors: Toby, Sky
 * Creation Date: 10/23/25
 * Last Modified:  2/17/26
 * 
 * Brief Description: billboarded. Appears when the player can interact with it.
 * Childed under billboard UI manager.
 * Gets shown / hidden when the player is looking at the base gameobject.
 */

using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptBillboardUI : IBillboardUI
{
    [SerializeField, Required] private RectTransform popupParent;
    //[SerializeField, Required] private TMP_Text interactText;
    [SerializeField, Required] private Image image;
    [ReadOnly] public ButtonType buttonType;

    private ButtonPromptInteractable buttomPrompt;
    private Coroutine popupAnimation;

    [Foldout("Popup Animation"), SerializeField, Label("Start Rotation")] private float popupStartRotation = 15;
    [Foldout("Popup Animation"), SerializeField, Label("Mid-Way Rotation")] private float popupMidwayRotation = -8;
    [Foldout("Popup Animation"), SerializeField, Label("Start Scale")] private float popupStartScale = 0.8f;
    [Foldout("Popup Animation"), SerializeField, Label("Mid-Way Scale")] private float popupMidwayScale = 1.1f;
    [Foldout("Popup Animation"), SerializeField, Label("Start to Mid-way seconds")] private float popupStartToMidwayAnimationSeconds = 0.15f;
    [Foldout("Popup Animation"), SerializeField, Label("Mid-Way to zero seconds"), HideIf(nameof(show_popupMidwayToZeroAnimationSeconds))] private float popupMidwayToZeroAnimationSeconds = 0.05f;
    private bool show_popupMidwayToZeroAnimationSeconds => popupMidwayRotation == 0 && popupMidwayScale == 1;

    /// <summary>
    /// Called when this objects is initialized
    /// </summary>
    /// <param name="sourceGameObject"></param>
    public override void OnInitialize(GameObject sourceGameObject)
    {
        buttomPrompt = sourceGameObject.GetComponentInChildren<ButtonPromptInteractable>();
        buttomPrompt.InitializeFromBillboardUI(this);
        buttonType = buttomPrompt.buttonKey;

        UpdateButtonPrompt();
    }

    public override void Show()
    {
        base.Show();

        StaticUtilities.StopAndStartCoroutine(ref popupAnimation, PopupAnimation());
    }

    public void UpdateButtonPrompt()
    {
        image.sprite = BillboardUIManager.Instance.GetKeyButtonSprite(buttonType, )
    }

    /// <summary>
    /// Meant to give the effect of having some snapback after rotating
    /// </summary>
    /// <returns></returns>
    private IEnumerator PopupAnimation()
    {
        // go counter clockwise half the time
        float randomDirection = Random.value > 0.5 ? -1 : 1;
        float startTime = Time.time;
        float t, z_rot, scalar;
        do
        {
            t = Mathf.Clamp((Time.time - startTime) / popupStartToMidwayAnimationSeconds, 0,1);
            z_rot = Mathf.Lerp(popupStartRotation, popupMidwayRotation, t) * randomDirection;
            scalar = Mathf.Lerp(popupStartScale, popupMidwayScale, t);  
            popupParent.transform.eulerAngles = popupParent.transform.eulerAngles.WithZ(z_rot);
            popupParent.transform.localScale = Vector3.one * scalar;
            yield return null;
        }
        while (t < 1);

        startTime = Time.time;
        do
        {
            t = Mathf.Clamp((Time.time - startTime) / popupMidwayToZeroAnimationSeconds, 0, 1);
            z_rot = Mathf.Lerp(popupMidwayRotation, 0, t) * randomDirection;
            scalar = Mathf.Lerp(popupMidwayScale, 1, t);
            popupParent.transform.eulerAngles = popupParent.transform.eulerAngles.WithZ(z_rot);
            popupParent.transform.localScale = Vector3.one * scalar;
            yield return null;
        }
        while (t < 1);
    }

    protected override float CalculateOpacity(float playerDistance, Vector3 UIPosition)
    {
        if (buttomPrompt.IsParentInteractable() == false)
            return 0;

        return base.CalculateOpacity(playerDistance, UIPosition);
    }

}


public enum ButtonType
{
    Interact, Action
}
