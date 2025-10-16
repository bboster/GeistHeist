/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 10/07/25
 * Summary: Holds data that needs to be accessed by multiple scripts that the enemies utilize.
 */

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.UI;

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
        public static void PersistentRay(Vector3 start, Vector3 direction, int length, GuardDebugger debug)
        {
            debug.DrawRay(start, direction);
        }
    }
}


