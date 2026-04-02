/*
 * Contributors: Toby
 * Creation: 11/22/2025
 * Last Edited: 11/22/2025
 * 
 * Summary: Pause menu tab for player controls.
 * No behavior right now, but that is subject to change when controller support is added.
 */


using NaughtyAttributes;
using UnityEngine;

public class ControlsTab : PauseMenuTab
{
    [Header("Controls Panels")]
    [SerializeField, Required] private CanvasGroup keyboardGroup;
    [SerializeField, Required] private CanvasGroup controllerGroup;

    private void Start()
    {
        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerChanged);
    }

    protected void OnControllerChanged()
    {
        RefreshUI();
    }

    public override void RefreshUI()
    {
        bool controller = InputEvents.Instance.IsGamepadActive() ;

        if (controller)
        {
            StaticUtilities.DisableCanvasGroup(keyboardGroup);
            StaticUtilities.EnableCanvasGroup(controllerGroup);
        }
        else
        {
            StaticUtilities.EnableCanvasGroup(keyboardGroup);
            StaticUtilities.DisableCanvasGroup(controllerGroup);
        }
    }
}
