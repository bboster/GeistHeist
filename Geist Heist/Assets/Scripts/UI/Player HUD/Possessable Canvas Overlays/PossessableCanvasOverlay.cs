/*
 * Contributors: Toby
 * Creation Date: 4/14/2026
 * Last Modified: 4/14/2026
 * 
 * Brief Description: Base class for possessable-specific canvas overlays
 */

using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PossessableCanvasOverlay : MonoBehaviour
{
    public abstract void Initialize();
    public abstract void WhilePossessedUpdate();
    public abstract Task ThisDeinitialize();

    private bool isDeinitializing;
    public async Task DeinitializeThenDestroy()
    {
        // Prevent from deinitializing
        if (isDeinitializing) return;
        isDeinitializing = true;

        await ThisDeinitialize();
        Destroy(this.gameObject);
    }
}
