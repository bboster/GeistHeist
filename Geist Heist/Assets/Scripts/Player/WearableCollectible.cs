using System;
using System.Runtime.CompilerServices;
using UnityEngine;

/*
 * Contributors: Joshua Kelly
 * Creation Date: 10/23/25
 * Last Modified: 10/23/25
 * 
 * Brief Description: dont put this script on the player.
 * handles possession and such.
 */
public class WearableCollectible : MonoBehaviour
{
    PossessableObject player;
    Transform wearable;
    public CollectableRegistry Registry = Resources.Load<CollectableRegistry>("Resource/CollectableRegistry.asset");
    private Collectable ThisCollectable;
    private GameObject meshPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   ThisCollectable = GetEquippedCollectable(SaveDataManager.Instance.EquipedHat());
        meshPrefab = GetEquippedCollectableMesh(Registry);
        equipHat(ThisCollectable);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void equipHat(Collectable  ThisCollectable)
    { 
            GameObject hat = Instantiate(meshPrefab);
            hat.transform.localPosition = Vector3.zero;
            hat.transform.localRotation = Quaternion.identity;
            hat.transform.localScale = Vector3.one; // ensures correct size
    }
    public Collectable GetEquippedCollectable(int collectableValue)
    {
        if (collectableValue < 0)
            return Collectable.None;

        if (!Enum.IsDefined(typeof(Collectable), collectableValue))
            return Collectable.None;

        return (Collectable)collectableValue;
    }

    /// <summary>
    /// Returns the mesh prefab for the currently equipped collectable (or null).
    /// </summary>
    public GameObject GetEquippedCollectableMesh(CollectableRegistry registry)
    {
        if (registry == null)
            return null;

        if (ThisCollectable == Collectable.None)
            return null;

        return registry.GetMesh(ThisCollectable);
    }
}
