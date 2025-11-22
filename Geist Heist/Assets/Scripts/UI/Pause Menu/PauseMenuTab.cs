/*
 * Contributors: Toby
 * Creation: 11/20/25
 * Last Edited: 11/20/25
 * Summary: Base class for a pause menu tab. Can open and close (wow!)
 */

using NaughtyAttributes;
using UnityEngine;

public class PauseMenuTab : MonoBehaviour
{
    public static PauseMenuTab currentOpenTab;

    [SerializeField, Required] public CanvasGroup canvasGroup;
    protected PauseMenu pauseMenu;

    public virtual void OpenTab()
    {
        if (currentOpenTab == this) return;

        currentOpenTab.CloseTab();
        currentOpenTab = this;

        RefreshUI();

        StaticUtilities.EnableCanvasGroup(canvasGroup);

        /*StaticUtilities.DisableCanvasGroup(pauseMenu.pauseGroup);
        InputEvents.PauseStartedOverride = () => CloseTab();*/
    }

    public virtual void CloseTab()
    {
        StaticUtilities.DisableCanvasGroup(canvasGroup);
        //InputEvents.PauseStartedOverride = null;
        //pauseMenu.OpenPauseMenu();
    }

    public virtual void RefreshUI()
    {
    }
}
