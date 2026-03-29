/*
 * Contributors: Toby
 * Creation: 11/20/25
 * Last Edited: 11/20/25
 * Summary: Base class for a pause menu tab. Can open and close (wow!)
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuTab : MonoBehaviour
{
    public static PauseMenuTab currentOpenTab;

    [SerializeField, Required] public CanvasGroup canvasGroup;
    [SerializeField, Required] public Toggle toggleButton;
    [SerializeField, Required] public WavyTextAnimation wavyTextAnimation;

    public virtual void OpenTab()
    {
        RefreshUI();
        toggleButton.isOn = true;
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
        toggleButton.isOn = false;
        StaticUtilities.DisableCanvasGroup(canvasGroup);
        //InputEvents.PauseStartedOverride = null;
        //pauseMenu.OpenPauseMenu();
    }

    public virtual void RefreshUI()
    {
    }
}
