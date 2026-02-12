using NaughtyAttributes;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobePossessable : IInputHandler
{
    [SerializeField] private CinemachineCamera globeCamera;
    private PossessableObject possessableObject => GetComponent<PossessableObject>();

    private Coroutine globePossessAnimation;
    [SerializeField, Scene] private string GlobeScene = "ACTUAL Globe Scene";

    public override void WhilePossessingUpdate()
    {
    }

    public override void OnPossessionStart()
    {
        if (globePossessAnimation != null)
        {
            globePossessAnimation = StartCoroutine(LoadNextSceneCooldown());
        }
    }

    public override void OnPossessionEnded()
    {
    }


    #region action
    public override void OnActionStarted()
    {
        Debug.Log("please");
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

    #region Space Bar
    public override void OnSpaceStarted()
    {
    }

    public override void WhileSpaceHeld(float secondsHeld)
    {
    }

    public override void OnSpaceCanceled(float secondsHeld)
    {
    }
    #endregion

    //TO DO: adapt this to start on the tether and maybe go to another script when we get an animation, currently changes scenes abruptly
    IEnumerator LoadNextSceneCooldown()
    {

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(GlobeScene);
        LevelManager.Instance.ChangeScene(GlobeScene);
    }
}
