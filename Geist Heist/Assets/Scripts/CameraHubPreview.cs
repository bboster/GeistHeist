using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraHubPreview : MonoBehaviour
{
    private CinemachineCamera hubCamera;
    private Animator camAnimator;
    //level 1, level 2, level 3, level 4, level 5, globe

    private void Start()
    {
        hubCamera = GetComponent<CinemachineCamera>();
        camAnimator = GetComponent<Animator>();

        //mark hub as complete
        SaveDataManager.Instance.MarkSceneAsCompleted(SceneManager.GetActiveScene().name);

        //do cutscene
        CheckForCameraTransition();
    }

    private void CheckForCameraTransition()
    {
        for (int i = 0; i < SaveDataManager.Instance.ScenesToCutscene.Count; i++)
        {
            if (IsSceneCompletedAndCutsceneNotPlayed(SaveDataManager.Instance.ScenesToCutscene[i]))
            {
                if (camAnimator == null || hubCamera == null)
                {
                    break;
                }

                camAnimator.SetInteger("Cutscene", i);
                hubCamera.Priority = 50;

                SaveDataManager.Instance.MarkSceneCutsceneAsCompleted(SaveDataManager.Instance.ScenesToCutscene[i]);
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

    //separated for visuals
    public void SwitchToPlayerCam()
    {
        hubCamera.Priority = 0;
    }


    public void OnCutsceneEnd()
    {
        InputEvents.Instance.CutsceneRunning = false;
        camAnimator.SetInteger("Cutscene", 50);
    }
}
