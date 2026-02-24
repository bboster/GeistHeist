/*
 * Contributors:Josh
 * Creation:2/1/2026
 * Last Edited: 2/1/2026
 * Summary:  Runtime-only key manager. Keys are kept in-memory and cleared on scene load. 
 * Call KeyManager.Instance.AddKey(...) from pickups. Use HasKey(...) to check.
 * Keys are NOT consumed on use.
 */
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class KeyManager : Singleton<KeyManager>
{
    [SerializeField] private List<KeyUISprite> KeyUIIcons = new List<KeyUISprite>();

    public event Action<KeyType> OnKeyCollected;

    private readonly HashSet<KeyType> _keys = new();
    public void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
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
            if (LevelManager.Instance != null)
                LevelManager.Instance.SaveCurrentKeys();

            OnKeyCollected?.Invoke(key);
            Debug.Log($"KeyInventory: Collected key {key}");
        }
    }

    // Keys are not consumed on use.
    public bool HasKey(KeyType key) => key != KeyType.None && _keys.Contains(key);

    public IReadOnlyCollection<KeyType> GetKeys() => _keys;

    public void Clear()
    {
        if (_keys == null) return;
        if (_keys.Count == 0) return;
        _keys.Clear();
        Debug.Log("KeyInventory: Cleared keys on scene load.");
    }

    #region UI Mapping

    [System.Serializable]
    public class KeyUISprite
    {
        public KeyType Key;
        [ShowAssetPreview(64,64)]
        public Sprite UISprite;
    }

    public Sprite GetKeySprite(KeyType key)
    {
        // this will return an error if a sprite for 'key' is undefined.
        // but that is good because that shouldnt happen.
        return KeyUIIcons.Where(k => k.Key == key).First().UISprite;
    }

    #endregion
}


