#if UNITY_EDITOR

/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 11/15/25
 * Summary: Has miscellaneous debugging functions for the guard, will be expanded as required.
 * NOTE: ONLY COMPILES FOR EDITOR, NOT FOR BUILD
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardDebugger : MonoBehaviour
{
    private bool shouldDrawRay = false;

    private Coroutine rayDrawer;

    public void DrawRay(Vector3 start, Vector3 direction)
    {
        StartCoroutine(RayDrawer(start, direction));
    }

    /// <summary>
    /// Draws a ray with given start and direction every frame
    /// </summary>
    /// <param name="start"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerator RayDrawer(Vector3 start, Vector3 direction)
    {
        for(; ; )
        {
            Debug.DrawRay(start, direction, Color.lightGreen);
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Controls progress bars for behaviors
    /// </summary>
    /// <returns></returns>
    public IEnumerator PathProgressBarCoroutine(NavMeshAgent agent, GuardMovement reciever)
    {
        float pathDistance = 0;

        if (agent.path != null)
        {
            for(; ; )
            {
                NavMeshPath path = new();
                agent.CalculatePath(agent.destination, path);

                for(int i = 1; i < path.corners.Length; i++)
                {
                    pathDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }

                for (; ; )
                {
                    reciever.progress = ((pathDistance - agent.remainingDistance) / pathDistance) * 100;

                    if (reciever.progress >= 100)
                        break;

                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    /// <summary>
    /// Starts a progress bar
    /// </summary>
    /// <param name="duration"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public void StartDebugProgress(float duration, Behavior reciever)
    {
        StartCoroutine(ProgressTimer(duration, reciever));
    }

    /// <summary>
    /// Runs a generic progress bar
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="reciever"></param>
    /// <returns></returns>
    private IEnumerator ProgressTimer(float duration, Behavior reciever)
    {
        for (int i = 0; i < duration * 10; i++)
        {
            reciever.progress = (i / (duration * 10)) * 100;

            yield return new WaitForSeconds(0.1f);
        }

        reciever.progress = 0;
    }
}
#endif
