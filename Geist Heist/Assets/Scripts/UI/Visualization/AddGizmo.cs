using UnityEngine;
/*
 * Contributors: Sky
 * Creation Date: 10/28/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Draws a gizmo at the game object's position
 * 
 * TODO: Improve this script (choose shape, choose color, choose size, etc)
 */
public class AddGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, .5f);
    }
}
