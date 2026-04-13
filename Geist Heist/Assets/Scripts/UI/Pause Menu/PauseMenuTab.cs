/*
 * Contributors: Toby
 * Creation: 11/20/25
 * Last Edited: 4/7/2026
 * Summary: Base class for a pause menu tab. Can open and close (wow!)
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuTab : MonoBehaviour
{
    public static PauseMenuTab currentOpenTab;

    [SerializeField, Required] public CanvasGroup canvasGroup;
    [SerializeField, Required] public Toggle toggleButton;
    [SerializeField, Required] public WavyTextAnimation wavyTextAnimation;
    [SerializeField, Required] private Selectable firstSelectedElement;
    [SerializeField, HideIf(nameof(_firstSelectedElementIsNull))] private Selectable secondSelectedElement;

    private bool _firstSelectedElementIsNull => firstSelectedElement == null;

    public virtual void OpenTab()
    {
        RefreshUI();
        if(toggleButton != null) toggleButton.isOn = true;

        if (currentOpenTab == this) return;

        if(currentOpenTab != null)
            currentOpenTab.CloseTab();

        currentOpenTab = this;

        StaticUtilities.EnableCanvasGroup(canvasGroup);

        /*StaticUtilities.DisableCanvasGroup(pauseMenu.pauseGroup);
        InputEvents.PauseStartedOverride = () => CloseTab();*/
    }

    public virtual void CloseTab()
    {
        if(toggleButton != null) toggleButton.isOn = false;
        StaticUtilities.DisableCanvasGroup(canvasGroup);
        //InputEvents.PauseStartedOverride = null;
        //pauseMenu.OpenPauseMenu();
    }

    public virtual void RefreshUI()
    {
    }

    public Selectable GetFirstSelectedElementInMenu()
    {
        if(_firstSelectedElementIsNull) return null;

        if (firstSelectedElement.gameObject.activeSelf == false)
            return secondSelectedElement;
        else
            return firstSelectedElement;
    }
}
