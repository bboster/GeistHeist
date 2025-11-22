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
    private int collectablesCollected => allCollectables.Where(c => c.IsCollected).Count();

    private void Start()
    {
        allCollectables = Object.FindObjectsByType<OptionalCollectable>(FindObjectsSortMode.None);
    }

    public override void RefreshUI()
    {
        // Count of collectables
        if(allCollectables.Count() != 0)
            collectablesCollectedText.text = $"{defaultCollectedText} {collectablesCollected} / {allCollectables.Count()}";
        else
            collectablesCollectedText.gameObject.SetActive(false);

        // current scene name
        stageNameText.text = defaultStageNameText.Replace("[SCENE_NAME]", SceneManager.GetActiveScene().name); //TODO: make a static class that handles strings like this? 8/
    }
}
