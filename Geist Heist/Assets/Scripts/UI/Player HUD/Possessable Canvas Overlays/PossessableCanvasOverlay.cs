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
    public abstract IEnumerator ThisDeinitialize();

    /// <summary>
    /// Call this function from the prefab, not from an instantiated object
    /// </summary>
    public abstract bool ShouldSpawnOverlay();

    protected bool isDeinitializing;
    public void DeinitializeThenDestroy()
    {
        Debug.Log($"Deinitializing {gameObject.name}");

        // Prevent from deinitializing
        if (isDeinitializing) return;
        isDeinitializing = true;

        StartCoroutine(ThisDeinitialize()).
            Then(() => Destroy(this.gameObject));
        
    }
}
