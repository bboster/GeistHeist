/*
 * Contributors:Josh
 * Creation:2/1/2026
 * Last Edited: 2/1/2026
 * Summary:  Runtime-only key manager. Keys are kept in-memory and cleared on scene load. 
 * Call KeyManager.Instance.AddKey(...) from pickups. Use HasKey(...) to check.
 * Keys are NOT consumed on use.
 * 
 * TODO: // Simple enum for keys. Extend with the symbols used in UI.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    public event Action<KeyType> OnKeyCollected;

    private readonly HashSet<KeyType> _keys = new HashSet<KeyType>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear inventory when a new scene is loaded (player can't carry keys across scenes).
        Clear();
    }

    public void AddKey(KeyType key)
    {
        if (key == KeyType.None) return;
        if (_keys.Add(key))
        {
            OnKeyCollected?.Invoke(key);
            Debug.Log($"KeyInventory: Collected key {key}");
        }
    }

    // Keys are not consumed on use.
    public bool HasKey(KeyType key) => key != KeyType.None && _keys.Contains(key);

    public IReadOnlyCollection<KeyType> GetKeys() => _keys;

    public void Clear()
    {
        if (_keys.Count == 0) return;
        _keys.Clear();
        Debug.Log("KeyInventory: Cleared keys on scene load.");
    }
}
