using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraHubPreview : MonoBehaviour
{
    [SerializeField] private CinemachineCamera hubCamera;
    [SerializeField] private Animator camAnimator;
    [SerializeField] private List<string> ScenesToCutscene;
    //level 1, level 2, level 3, level 4, level 5, globe

    private void Start()
    {
        CheckForCameraTransition();
    }

    private void CheckForCameraTransition()
    {
        for (int i = 0; i < ScenesToCutscene.Count; i++)
        {
            if (IsSceneCompletedAndCutsceneNotPlayed(ScenesToCutscene[i]))
            {
                camAnimator.SetInteger("Cutscene", i);
                SaveDataManager.Instance.MarkSceneCutsceneAsCompleted(ScenesToCutscene[i]);
                InputEvents.Instance.CutsceneRunning = true;
                return;
            }
        }
    }

    public bool IsSceneCompletedAndCutsceneNotPlayed(string sceneName)
    {
        if (!SaveDataManager.Instance.IsSceneCutsceneCompleted(sceneName) && SaveDataManager.Instance.IsLevelCompleted(sceneName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnCutsceneEnd()
    {
        InputEvents.Instance.CutsceneRunning = false;
    }
}
