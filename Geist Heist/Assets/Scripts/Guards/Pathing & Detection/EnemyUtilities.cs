/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 10/02/25
 * Summary: Holds data that needs to be accessed by multiple scripts that the enemies utilize.
 */

using UnityEngine;

namespace GuardUtilities
{
    public enum Priority { low, medium, high, highest }
    public enum GuardStates { none, patrol, chase, attack, surprised, search, visionBreak, returnToPath, concussed }
}


