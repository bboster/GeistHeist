/*
 * Contributors: Toby
 * Creation: 10/3/25
 * Last Edited: 10/3/25
 * Summary: Mirrors a tether object, appears in the hub if the save data has marked the collectable as got.
 */

using NaughtyAttributes;
using System;
using UnityEngine;

public class TetherHubDisplay : MonoBehaviour
{
    [InfoBox("Set this gameobject to active by default, it will be disabled based on whether or not the player has save data for completing the tether's level")]
    [InfoBox("If a level is not appearing, make sure it is added to the build settings")]

    [SerializeField, Scene] private string LevelWithTether;

    [Header("Debug")]
    [SerializeField, OnValueChanged(nameof(UpdateVisibility))] private bool DebugAlwaysDisplay;

    public void Start()
    {
        // TODO: I think it might be better if the tethers model/mesh was pulled from some kind of table/dictionary?

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        gameObject.SetActive(DebugAlwaysDisplay || SaveDataManager.Instance.IsLevelCompleted(LevelWithTether));
    }
}
