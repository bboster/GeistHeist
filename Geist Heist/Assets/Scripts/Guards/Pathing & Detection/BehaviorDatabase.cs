/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/30/25
 * Last Edited: 9/30/25
 * Summary: Stores the scriptable objects for guard behaviors and facilitates their retrieval.
 */

using UnityEngine;
using GuardUtilities;

public class BehaviorDatabase : Singleton<BehaviorDatabase>
{
    [SerializeField] private Behavior[] guardBehaviors;

    /// <summary>
    /// Retrieves a behavior from the database.
    /// </summary>
    /// <param name="state"></param>
    public Behavior GetBehavior(GuardStates state)
    {
        return guardBehaviors[(int)state];
    }
}
