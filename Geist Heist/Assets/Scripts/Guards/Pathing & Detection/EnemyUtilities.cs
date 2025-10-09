/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 10/07/25
 * Summary: Holds data that needs to be accessed by multiple scripts that the enemies utilize.
 */

using UnityEngine;
using System.Collections;

namespace GuardUtilities
{
    public enum Priority { low, medium, high, highest }
    public enum GuardStates { none, patrol, chase, attack, surprised, search, visionBreak, returnToPath, concussed }

    public class GuardDebug
    {
        /// <summary>
        /// Creates a persistent debug ray for the guard's vision cast
        /// </summary>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerator PersistentRay(Vector3 start, Vector3 direction, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Debug.DrawRay(start, direction);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}


