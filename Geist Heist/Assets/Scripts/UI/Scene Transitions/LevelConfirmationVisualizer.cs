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
using UnityEngine.UI;

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

    // i know theres only one tether in our game but scalability (also there may be an animation later that duplicates the tethers)
    [SerializeField] private List<MeshRenderer> TetherModels;

    [InfoBox("Collectable Models and Materials will be automatically retrieved from the collectable registry")]
    [SerializeField] private List<LevelConfirmCollectableMesh> CollectableMeshes;

    [Header("Settings")]
    [SerializeField] private float sizeToFitForTether = 5.0f;
    [SerializeField] private float sizeToFitForCollectable = 1.0f;
    [SerializeField] private float tiltAngle = 15;
    [Tooltip("Seconds to do a full spin")]
    [SerializeField] private float collectableRotationSeconds = 6;
    [SerializeField] private float tetherRotationSeconds = 20;
    [SerializeField, Required] private Material notCollectedMaterial;


    [Foldout("Advanced"), Required, SerializeField] private Camera renderCamera;
    [Foldout("Advanced"), Required, SerializeField] private RawImage renderCameraOverlayImage;

    private static CollectableRegistry collectableRegistry;
    private static RenderTexture renderCameraOutputTexture;

    private string tetherToDisplay;

    /// <summary>
    /// Initialized from kiosk
    /// </summary>
    public void Initialize(string sceneToLoad)
    {
        if (collectableRegistry == null)
            collectableRegistry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);

        // make a copy of the material, to not flood github
        notCollectedMaterial = Instantiate(notCollectedMaterial);

        foreach (var tetherModel in TetherModels)
        {
            RefreshTether(tetherModel);
        }

        foreach (var collectableMesh in CollectableMeshes)
        {
            RefreshCollectable(collectableMesh);
        }

        InitializeRenderCamera();
    }

    #region Viewport Objects

    #region Viewport Objects Initialization
    private void RefreshTether(MeshRenderer tetherMesh)
    {
        ScaleToFitBounds(tetherMesh.GetComponent<MeshFilter>(), sizeToFitForTether);

        // if its null then its probably because this is being run from the debug button.
        if (SaveDataManager.Instance == null) return;

        if (SaveDataManager.Instance.IsLevelCompleted(tetherToDisplay))
        {
            // return because tether is visible with correct materials by default
            return;
        }
        else
        {
            // Set materials to uncollected
            int count = tetherMesh.materials.Count();
            var emptyMaterials = Enumerable.Repeat(notCollectedMaterial, count).ToList();
            tetherMesh.SetMaterials(emptyMaterials);
        }
    }

    private void RefreshCollectable(LevelConfirmCollectableMesh collectable)
    {
        var mesh = collectableRegistry.GetMesh(collectable.collectable);
        if(mesh == null)
        {
            Debug.LogWarning($"There is no mesh associated with the hat {collectable.ToString()} in collectible registry");
            return;
        }
        collectable.meshFilter.mesh = mesh;
        ScaleToFitBounds(collectable.meshFilter, sizeToFitForCollectable);

        // if its null then its probably because this is being run from the debug button.
        if (SaveDataManager.Instance == null) return;

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

    #endregion

    #region Viewport Objects Animation
    private void Update()
    {
        notCollectedMaterial.SetFloat("_Unscaled_Time", Time.unscaledTime);

        for (int i=0; i<CollectableMeshes.Count; i++)
        {
            var collectableMesh = CollectableMeshes[i];
            RotateItem(collectableMesh.collectableObject.transform, collectableRotationSeconds, i, tiltAngle);
        }

        for (int i = 0; i < TetherModels.Count; i++)
        {
            var tetherMesh = TetherModels[i];
            RotateItem(tetherMesh.transform, tetherRotationSeconds, 0, 0);
        }
    }

    private void RotateItem(Transform item, float rotateSeconds, float offset, float tilt)
    {
        Vector3 rotation = new Vector3(0, (Time.unscaledTime + offset) * 360 / rotateSeconds, tilt);
        item.localEulerAngles = rotation;
    }

    #endregion

    #endregion

    private void InitializeRenderCamera()
    {
        // manually create a new render texture with code so we dont have one more thing clogging up our github

        if (renderCameraOutputTexture == null)
        {
            renderCameraOutputTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32); // 1920 x 1080 resolution
            renderCameraOutputTexture.Create();
        }

        // set render cameras output
        renderCamera.targetTexture = renderCameraOutputTexture;
        
        // apply render texture as overlay
        renderCameraOverlayImage.texture = renderCameraOutputTexture;
        renderCameraOverlayImage.GetComponent<CanvasGroup>().alpha = 1; // in inspector, interactability is disabled.
    }

    public void OnLoadingAnimationFinished()
    {
        // middle-man function because of the way animations events work
        var confirmation = GetComponentInChildren<LevelConfirmationPopup>();
        confirmation.OnLoadingAnimationFinished();
    }

    #region Debug

    [Button]
    private void Debug_RefreshCollectableDisplays()
    {
        // idk what scene to put in there, it doesnt matter
        Initialize("Main Menu");
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

    #endregion debug
}
