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
                    reciever.PathProgress = ((pathDistance - agent.remainingDistance) / pathDistance) * 100;

                    if (reciever.PathProgress >= 100)
                        break;


                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
            }



        }
    }
}
