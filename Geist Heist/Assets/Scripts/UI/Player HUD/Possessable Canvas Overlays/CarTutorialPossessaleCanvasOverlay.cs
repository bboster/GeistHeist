/*
 * Contributors: Toby Schamberger
 * Creation Date: 4/14/2026
 * Last Modified: 4/14/2026
 * 
 * Brief Description: Tutorializes pressing left/right if the player has never pressed left / right on the car before.
 */

using NaughtyAttributes;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CarTutorialPossessaleCanvasOverlay : PossessableCanvasOverlay
{
    [Header("Components")]
    [Required, SerializeField] private CanvasGroup group;
    [SerializeField, Required] private WavyTextAnimation wavyTextBox;

    [Header("Text")]
    [SerializeField] private string KeyboardText;
    [SerializeField] private string ControllerText;

    [Header("Misc")]
    [SerializeField] private float fadeAnimationSeconds = 1;

    private Coroutine fadeOpacityCoroutine;

    public override void Initialize()
    {
        if (SaveDataManager.Instance.HasPlayerMovedWithCar())
        {
            Debug.Log("Player has moved with car before: hiding tutorial");
            Destroy(this.gameObject);
            return;
        }

        fadeOpacityCoroutine = StaticUtilities.FadeOpacity(group, 0, 1, seconds: fadeAnimationSeconds, unscaledTime: false);

        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerUpdated);
        OnControllerUpdated();
    }

    public override void WhilePossessedUpdate()
    {
        if (isDeinitializing) return;

        //PossessableToolbar.Instance?.currentCanvasOverlay?.WhilePossessedUpdate();

        // if the player has now moved with the car
        if (SaveDataManager.Instance.HasPlayerMovedWithCar())
        {
            DeinitializeThenDestroy();
        }
    }

    public override IEnumerator ThisDeinitialize()
    {
        yield return StaticUtilities.FadeOpacity(group, group.alpha, 0, fadeAnimationSeconds, unscaledTime: true, currentCoroutineToCancel: fadeOpacityCoroutine);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        
        if (Application.isPlaying) return;
        
        if(Mathf.RoundToInt((float)EditorApplication.timeSinceStartup / 5) % 2 == 0)
            wavyTextBox.SetText( KeyboardText );
        else
            wavyTextBox.SetText( ControllerText );
    }

#endif

    private void OnControllerUpdated()
    {
        wavyTextBox.SetText(InputEvents.Instance.IsGamepadActive() ? ControllerText : KeyboardText);
    }

    public override bool ShouldSpawnOverlay()
    {
        return ! SaveDataManager.Instance.HasPlayerMovedWithCar();
    }
}
