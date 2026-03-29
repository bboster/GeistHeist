/*
 * Contributors: Toby
 * Creation:    3/5/2026
 * Last Edited: 3/5/2026
 * Summary: Base Class for the possessable action text.
 */

using UnityEngine;

public abstract class PossesableText : MonoBehaviour
{
    public void Initialize()
    {
        ThisInitialize();
        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerChanged);
        RefreshUI();
    }

    protected virtual void OnControllerChanged()
    {
        RefreshUI();
    }

    protected abstract void ThisInitialize();

    public abstract void RefreshUI();
}
