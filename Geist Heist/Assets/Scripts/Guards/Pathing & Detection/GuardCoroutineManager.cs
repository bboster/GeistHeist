using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GuardCoroutineManager : MonoBehaviour
{
    public static GuardCoroutineManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Starts a behavior timer
    /// </summary>
    /// <param name="timerLength"></param>
    /// <param name="signalReciever"></param>
    public void StartBehaviorTimer(float timerLength, Behavior signalReciever)
    {
        signalReciever.TimerCoroutine = StartCoroutine(BehaviorTimer(timerLength, signalReciever));
    }


    /// <summary>
    /// Stops a behavior timer
    /// </summary>
    /// <param name="stopRef"></param>
    public void StopBehaviorTimer(Coroutine stopRef)
    {
        if(stopRef != null)
        {
            StopCoroutine(stopRef);
        }
    }

    /// <summary>
    /// Runs a timer for a behavior
    /// </summary>
    /// <param name="timerLength"></param>
    /// <param name="signalReciever"></param>
    /// <returns></returns>
    public IEnumerator BehaviorTimer(float timerLength, Behavior signalReciever)
    {
        yield return new WaitForSeconds(timerLength);

        signalReciever.StopBehavior();
    }
}
