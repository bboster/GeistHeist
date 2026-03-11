/*
 * Contributors: Toby
 * Creation: 10/2/2025
 * Last Edited: 11/15/2025
 * Summary: Tether possessable. Progresses player to next level
 * 
 * TODO: An animation for when tether is collected, i suppose
 */

using NaughtyAttributes;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TetherPossessable : IInputHandler
{
    [SerializeField] private GameObject CollectionParticlePrefab;
    [SerializeField] private GameObject thirdPersoncinemachineCamera;
    [Tooltip("Loads this scene")]
    [SerializeField, Scene] private string HubScene = "Lobby";

    private Coroutine victoryAnimation;

    private void Start()
    {
        if(thirdPersoncinemachineCamera != null)
            thirdPersoncinemachineCamera.SetActive(false);
    }

    public override void OnPossessionStart()
    {
        victoryAnimation = StartCoroutine(LoadNextSceneCooldown());
    }

    public override void OnPossessionEnded()
    {
        // Not sure if this code will ever get reached (hopefully not), but im keeping it to be safe
        Debug.Log("Canceling victory");
        StopCoroutine(victoryAnimation);
    }


    public override void WhilePossessingUpdate()
    {
    }

    
    //TO DO: adapt this to start on the tether and maybe go to another script when we get an animation, currently changes scenes abruptly
    IEnumerator LoadNextSceneCooldown()
    {
        Debug.Log($"Tether collected! Leaving {SceneManager.GetActiveScene().name} now...");
        
        yield return new WaitForSeconds(1.5f);

        SaveDataManager.Instance.MarkSceneAsCompleted(SceneManager.GetActiveScene().name);

        //SceneManager.LoadScene(HubScene);
        LevelManager.Instance.InstantiateFadeToBlack(() => LevelManager.Instance.ChangeScene(HubScene));
    }

    public override bool IsDetectable() { return false; }

    #region action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld(float secondsHeld)
    {
    }

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
    }

    public override void OnActionCanceled(float secondsHeld)
    {
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
    }

    public override void WhileInteractHeld(float secondsHeld)
    { }

    public override void OnInteractCanceled(float secondsHeld)
    {
    }
    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld(float secondsHeld)
    {
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled(float secondsHeld) { }


    #endregion
}