using UnityEngine;
/*
 * Contributors: Sky
 * Creation Date: 10/28/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Draws a gizmo at the game object's position
 * 
 * TODO: Improve this script (choose shape, choose color, choose size, etc)
 * USE NAUGHTY ATTRIBUTES!!
 * I think this can be a really expandable script for easy design gizmos 
 * so that you don't have to put gizmo code in every script
 */
public class AddGizmo : MonoBehaviour
{
    //temp for possessable exit points, make more modular in the future
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, .5f);

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 0.35f);
        if (colliders.Length > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, .2f);
        }
    }
}
