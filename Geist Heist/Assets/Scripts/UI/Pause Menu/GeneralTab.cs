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
    [Header("Collectables Text")]
    [SerializeField, Required] private TMP_Text tethersCollectedText;
    [SerializeField] private string defaultTethersCollectedText = "[COLLECTED]/1 Tethers";

[Header("Collectables Text")]
    [SerializeField, Required] private TMP_Text collectablesCollectedText;
    [SerializeField] private string defaultHatsCollectedText = "[COLLECTED]/[COUNT] Hats";

    [Header("Current Level Text")]
    [SerializeField, Required] private TMP_Text stageNameText;
    [SerializeField] private string defaultStageNameText = "Current Wing: [SCENE_NAME]";

    private OptionalCollectable[] allCollectables;
    private TetherPossessable[] allTethers;

    private int numTethersCollected => allTethers.Where(c => c.IsCollected).Count();

    private int numCollectablesCollected => allCollectables.Where(c => c.IsCollected).Count();

    /// <summary>
    /// RefreshUI is automatically called when generaltab is opened
    /// </summary>
    public override void RefreshUI()
    {
        if (allCollectables == null) allCollectables = Object.FindObjectsByType<OptionalCollectable>(FindObjectsSortMode.None);
        if(allTethers == null) allTethers = Object.FindObjectsByType<TetherPossessable>(FindObjectsSortMode.None);

        // Count of tethers
        if (allTethers.Count() != 0)
        {
            tethersCollectedText.text = defaultTethersCollectedText
                                        .Replace("[COLLECTED]", numTethersCollected.ToString())
                                        .Replace("[COUNT]", allTethers.Count().ToString());
        }
        else
            tethersCollectedText.gameObject.SetActive(false);

        // Count of collectables
        if (allCollectables.Count() != 0)
        {
            collectablesCollectedText.text = defaultHatsCollectedText
                                            .Replace("[COLLECTED]", numCollectablesCollected.ToString())
                                            .Replace("[COUNT]", allCollectables.Count().ToString());
        }
        else
            collectablesCollectedText.gameObject.SetActive(false);

        // current scene name
        string levelName = LevelManager.Instance.GetLevelDisplayName(SceneManager.GetActiveScene().name);
        stageNameText.text = defaultStageNameText.Replace("[SCENE_NAME]", levelName); //TODO: make a static class that handles strings like this? 8/
    }
}
