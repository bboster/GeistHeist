/*
 * Contributors: Toby
 * Creation: 10/2/2025
 * Last Edited: 10/2/25
 * Summary: Tether possessable. Progresses player to next level
 * 
 * TODO: An animation for when tether is collected, i suppose
 */

using NaughtyAttributes;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TetherPossessable : IInputHandler
{
    [SerializeField] private GameObject CollectionParticlePrefab;
    [SerializeField,Required] private GameObject thirdPersoncinemachineCamera;
    [Tooltip("Loads this scene")]
    [SerializeField, Scene] private string HubScene = "Lobby";

    private Coroutine victoryAnimation;

    private void Start()
    {
        thirdPersoncinemachineCamera.SetActive(false);
    }

    public override void OnPossessionStarted()
    {
        victoryAnimation = StartCoroutine(LoadNextSceneCooldown());
    }

    public override void OnPossessionEnded()
    {
        // Not sure if this code will ever get reached (hopefully not), but im keeping it to be safe
        Debug.Log("Canceling victory");
        StopCoroutine(victoryAnimation);
    }

    //TODO: replace this with something else
    IEnumerator LoadNextSceneCooldown()
    {
        Debug.Log($"Tether collected! Leaving {SceneManager.GetActiveScene().name} now...");
        
        yield return new WaitForSeconds(1.5f);

        SaveDataManager.Instance.MarkSceneAsCompleted(SceneManager.GetActiveScene().name);

        SceneManager.LoadScene(HubScene);
    }

    #region action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld()
    {
    }

    public override void OnActionCanceled()
    {
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
    }

    public override void WhileInteractHeld()
    { }

    public override void OnInteractCanceled()
    {
    }
    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld()
    {
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled() { }
    #endregion
}