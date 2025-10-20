/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/09/25
 * Last Edited: 10/09/25
 * Summary: Handles any functions that need to be applied to all of the guards
 */
using UnityEngine;

public class GuardManager : Singleton<GuardManager>
{
    public GuardController[] guards;

    /// <summary>
    /// Initializes all guards in the scene.
    /// </summary>
    public void Initialize()
    {
        guards = FindObjectsByType<GuardController>(FindObjectsSortMode.None);

        foreach(GuardController gc in guards)
        {
            gc.InitializeGuard();
        }
    }
}