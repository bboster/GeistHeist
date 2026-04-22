using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraHubPreview : MonoBehaviour
{
    [SerializeField] private CinemachineCamera hubCamera;
    [SerializeField] private Animator camAnimator;
    //level 1, level 2, level 3, level 4, level 5, globe

    private void Start()
    {
        SaveDataManager.Instance.MarkSceneAsCompleted(SceneManager.GetActiveScene().name);
        CheckForCameraTransition();
    }

    private void CheckForCameraTransition()
    {
        for (int i = 0; i < SaveDataManager.Instance.ScenesToCutscene.Count; i++)
        {
            if (IsSceneCompletedAndCutsceneNotPlayed(SaveDataManager.Instance.ScenesToCutscene[i].ToString()))
            {
                camAnimator.SetInteger("Cutscene", i);
                hubCamera.Priority = 50;
                SaveDataManager.Instance.MarkSceneCutsceneAsCompleted(SaveDataManager.Instance.ScenesToCutscene[i]);
                InputEvents.Instance.CutsceneRunning = true;
                Debug.Log("horse");
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
        hubCamera.Priority = 0;

        camAnimator.SetInteger("Cutscene", 50);
    }
}
