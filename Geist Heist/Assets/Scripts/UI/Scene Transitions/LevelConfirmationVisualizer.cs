/*
 * Contributors: Toby
 * Creation:    2/3/2026
 * Last Edited: 2/3/2026
 * Summary: Displays the tethers and all the collectables in a level.
 */

using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelConfirmationVisualizer : MonoBehaviour
{
    [Serializable]
    private class LevelConfirmCollectableMesh
    {
        public Collectable collectable;
        public GameObject collectableObject;

        public MeshRenderer meshRenderer => collectableObject.GetComponent<MeshRenderer>();
        public MeshFilter meshFilter => collectableObject.GetComponent<MeshFilter>();
    }

    [SerializeField, Scene] private string LevelToLoad;

    // i know theres only one tether in our game but scalability (also there may be an animation later that duplicates the tethers)
    [SerializeField] private List<MeshRenderer> TetherModels;

    [InfoBox("Collectable Models and Materials will be automatically retrieved from the collectable registry")]
    [SerializeField] private List<LevelConfirmCollectableMesh> CollectableMeshes;

    [SerializeField] private float sizeToFitForTether = 5.0f;

    [Header("Settings")]
    [SerializeField] private float sizeToFitForCollectable = 1.0f;
    [SerializeField, Required] private Material notCollectedMaterial;

    private static CollectableRegistry collectableRegistry;

    /// <summary>
    /// Initialized from kiosk
    /// </summary>
    public void Initialize()
    {
        if (collectableRegistry == null)
            collectableRegistry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);

        foreach (var tetherModel in TetherModels)
        {
            RefreshTether(tetherModel);
        }

        foreach (var collectableMesh in CollectableMeshes)
        {
            RefreshCollectable(collectableMesh);
        }
    }

    private void RefreshTether(MeshRenderer tetherMesh)
    {
        ScaleToFitBounds(tetherMesh.GetComponent<MeshFilter>(), sizeToFitForTether);

        if (SaveDataManager.Instance.IsLevelCompleted(LevelToLoad))
        {
            // return because tether is visible with correct materials by default
            return;
        }
        else
        {
            // Set materials to uncollected
            Array.Fill(tetherMesh.materials, notCollectedMaterial);
        }
    }

    private void RefreshCollectable(LevelConfirmCollectableMesh collectable)
    {
        collectable.meshFilter.mesh = collectableRegistry.GetMesh(collectable.collectable);
        ScaleToFitBounds(collectable.meshFilter, sizeToFitForCollectable);
        
        if (SaveDataManager.Instance.IsCollectableCollected(collectable.collectable))
        {
            collectable.meshRenderer.materials = collectableRegistry.GetMaterials(collectable.collectable);
            return;
        }
        else
        {
            // Set all materials to black / empty
            int count = collectable.meshRenderer.materials.Count();
            var emptyMaterials = Enumerable.Repeat(notCollectedMaterial, count).ToList();
            collectable.meshRenderer.SetMaterials(emptyMaterials);
        }
    }

    private void ScaleToFitBounds(MeshFilter mesh, float sizeToFit)
    {
        mesh.transform.localScale = Vector3.one;
        Vector3 meshSize = mesh.sharedMesh.bounds.extents * 2;
        Vector3 scaledSize = new Vector3(sizeToFit / meshSize.x, sizeToFit / meshSize.y, sizeToFit / meshSize.z);
        mesh.transform.localScale = Vector3.one * scaledSize.Min();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        foreach(var tether in TetherModels)
        {
            Gizmos.DrawWireCube(tether.transform.position, Vector3.one * sizeToFitForTether);
        }

        Gizmos.color = Color.blue;
        foreach (var collectable in CollectableMeshes)
        {
            Gizmos.DrawWireCube(collectable.collectableObject.transform.position, Vector3.one * sizeToFitForCollectable);
        }
    }
}
