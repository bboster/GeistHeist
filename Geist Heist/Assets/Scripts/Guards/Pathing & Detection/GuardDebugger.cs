using System.Collections;
using UnityEngine;

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
}
