/*
 * Contributors: Toby Schamberger
 * Creation: 11/20/25
 * Last Edited: 11/20/25
 * Summary: General/main tab of the pause menu
 */

using NaughtyAttributes;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralTab : PauseMenuTab
{
    [SerializeField, Required] private TMP_Text collectablesCollectedText;
    [SerializeField] private string defaultCollectedText = "Collectables:";
    [SerializeField, Required] private TMP_Text stageNameText;
    [SerializeField] private string defaultStageNameText = "Current Wing: [SCENE_NAME]";

    private OptionalCollectable[] allCollectables;
    private int numCollectablesCollected => allCollectables.Where(c => c.IsCollected).Count();

    /// <summary>
    /// RefreshUI is automatically called when generaltab is opened
    /// </summary>
    public override void RefreshUI()
    {
        if (allCollectables == null)
            allCollectables = Object.FindObjectsByType<OptionalCollectable>(FindObjectsSortMode.None);

        // Count of collectables
        if (allCollectables.Count() != 0)
            collectablesCollectedText.text = $"{defaultCollectedText} {numCollectablesCollected} / {allCollectables.Count()}";
        else
            collectablesCollectedText.gameObject.SetActive(false);

        // current scene name
        stageNameText.text = defaultStageNameText.Replace("[SCENE_NAME]", SceneManager.GetActiveScene().name); //TODO: make a static class that handles strings like this? 8/
    }
}
