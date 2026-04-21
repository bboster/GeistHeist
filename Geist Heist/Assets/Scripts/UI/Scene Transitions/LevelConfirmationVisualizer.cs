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
    public class LevelConfirmCollectableMesh
    {
        public Collectable collectable;
        [HideInInspector] public GameObject collectableObject;
        
        [Foldout("Advanced")] public Vector3 rotationOffset = Vector3.zero;
        [Foldout("Advanced")] public float scaleMultiplier = 1;
        [Foldout("Advanced")] public Vector2 positionOffset;

        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
    }

    [Header("Per-level settings")]
    [InfoBox("Collectable Models and Materials will be automatically retrieved from the collectable registry")]
    [SerializeField] private List<LevelConfirmCollectableMesh> CollectableMeshes;
    [SerializeField] private Texture2D notCollectedTexture;
    [SerializeField] private Vector3 tetherRotationOffset;


    [Header("Settings")]
    [SerializeField] private float sizeToFitForTether = 5.0f;
    [SerializeField] private float sizeToFitForCollectable = 1.0f;
    [SerializeField] private float tiltAngle = 15;
    [Tooltip("Seconds to do a full spin")]
    [SerializeField] private float collectableRotationSeconds = 6;
    [SerializeField] private float tetherRotationSeconds = 20;
    [SerializeField, Required] private Material notCollectedMaterial;


    [Foldout("Advanced"), Required, SerializeField] private KioskCameraController renderCameraPrefab;
    [Foldout("Advanced"), Required, SerializeField] private RawImage renderCameraOverlayImage;
    [Foldout("Advanced"), Required, SerializeField] private int renderCameraSize = 5;

    private static CollectableRegistry collectableRegistry;
    private static RenderTexture renderCameraOutputTexture;
    private static Material notCollectedMaterialInstance;

    private KioskCameraController kioskRenderCameraInstance;

    private string tetherToDisplay;
    

    /// <summary>
    /// Initialized from kiosk
    /// </summary>
    public void Initialize(string sceneToLoad)
    {
        GameManager.Instance.SetPlayerInMenu(true);

        if (collectableRegistry == null)
            collectableRegistry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);

        kioskRenderCameraInstance = Instantiate(renderCameraPrefab);
        kioskRenderCameraInstance.Initialize(CollectableMeshes);

        foreach (var tetherModel in kioskRenderCameraInstance.TetherModels)
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

    private Material GetNotCollectedMaterialInstance()
    {
        if(notCollectedMaterialInstance == null)
        {
            // make a copy of the material, to not flood github
            notCollectedMaterialInstance = Instantiate(notCollectedMaterial);
            notCollectedMaterialInstance.SetTexture("_Background_Image", notCollectedTexture);
        }

        notCollectedMaterialInstance.SetTexture("_Background_Image", notCollectedTexture);

        return notCollectedMaterialInstance;
    }

    #region Viewport Objects Initialization
    private void RefreshTether(MeshRenderer tetherMesh)
    {
        ScaleToFitBounds(tetherMesh.GetComponent<MeshFilter>(), sizeToFitForTether, 1);

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

            var emptyMaterials = Enumerable.Repeat(GetNotCollectedMaterialInstance(), count).ToList();
            tetherMesh.SetMaterials(emptyMaterials);
        }
    }

    private void RefreshCollectable(LevelConfirmCollectableMesh collectable)
    {
        var mesh = collectableRegistry.GetMesh(collectable.collectable);

        if (mesh == null)
        {
            Debug.LogError($"There is no mesh associated with the hat {collectable.ToString()} in collectible registry");
            return;
        }
        else
        {
            collectable.meshFilter.mesh = mesh;
            ScaleToFitBounds(collectable.meshFilter, sizeToFitForCollectable, collectable.scaleMultiplier);
        }

        // if its null then its probably because this is being run from the debug button.
        if (SaveDataManager.Instance == null) return;

        if (SaveDataManager.Instance.IsCollectableCollected(collectable.collectable))
        {
            collectable.meshRenderer.materials = collectableRegistry.GetMaterials(collectable.collectable);
            return;
        }
        else
        {
            int count = collectable.meshRenderer.materials.Count();

            #region specific tether hard coding (sorry

            if (collectable.collectable == Collectable.Nightcap_Hat)
                count = 3;

            if (collectable.collectable == Collectable.Wizard_Hat)
                count = 20;

            #endregion

            var emptyMaterials = Enumerable.Repeat(GetNotCollectedMaterialInstance(), count).ToList();
            collectable.meshRenderer.SetMaterials(emptyMaterials);
        }
    }

    private void ScaleToFitBounds(MeshFilter mesh, float sizeToFit, float scaleMultiplier)
    {
        mesh.transform.localScale = Vector3.one;
        Vector3 meshSize = mesh.sharedMesh.bounds.extents * 2;
        Vector3 scaledSize = new Vector3(sizeToFit / meshSize.x, sizeToFit / meshSize.y, sizeToFit / meshSize.z);
        mesh.transform.localScale = Vector3.one * scaledSize.Min() * scaleMultiplier;
    }

    #endregion

    #region Viewport Objects Animation
    private void Update()
    {
        notCollectedMaterialInstance.SetFloat("_Unscaled_Time", Time.unscaledTime);

        for (int i=0; i<CollectableMeshes.Count; i++)
        {
            var collectableMesh = CollectableMeshes[i];
            RotateItem(collectableMesh.collectableObject.transform, collectableRotationSeconds, i, tiltAngle, collectableMesh.rotationOffset);
        }

        for (int i = 0; i < kioskRenderCameraInstance.TetherModels.Count; i++)
        {
            var tetherMesh = kioskRenderCameraInstance.TetherModels[i];
            RotateItem(tetherMesh.transform, tetherRotationSeconds, 0, 0, tetherRotationOffset);
        }

    }

    private void RotateItem(Transform item, float rotateSeconds, float timeOffset, float tilt, Vector3 rotationOffset)
    {
        Vector3 rotation = new Vector3(0, (Time.unscaledTime + timeOffset) * 360 / rotateSeconds, tilt) + rotationOffset;
        item.localEulerAngles = rotation;
    }

    #endregion

    #endregion

    private void InitializeRenderCamera()
    {
        // manually create a new render texture with code so we dont have one more thing clogging up our github

        if (renderCameraOutputTexture == null)
        {
            renderCameraOutputTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32); // 1920 x 1080 resolution
            renderCameraOutputTexture.Create();
        }

        // set render cameras output
        kioskRenderCameraInstance.camera.targetTexture = renderCameraOutputTexture;
        
        // apply render texture as overlay
        renderCameraOverlayImage.texture = renderCameraOutputTexture;
        renderCameraOverlayImage.GetComponent<CanvasGroup>().alpha = 1; // in inspector, interactability is disabled.
    }

    public void OnLoadingAnimationFinished()
    {
        // middle-man function because of the way animations events work
        var confirmation = GetComponentInChildren<LevelConfirmationPopup>();
        confirmation.OnLoadingAnimationFinished();
        GameManager.Instance.SetPlayerInMenu(false);
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
        foreach(var tether in kioskRenderCameraInstance.TetherModels)
        {
            Gizmos.DrawWireCube(tether.transform.position, Vector3.one * sizeToFitForTether);
        }

        Gizmos.color = Color.blue;
        foreach (var collectable in CollectableMeshes)
        {
            Gizmos.DrawWireCube(collectable.collectableObject.transform.position, Vector3.one * sizeToFitForCollectable);
            collectable.collectableObject.transform.localEulerAngles = collectable.rotationOffset;
        }
    }

    #endregion debug

    private void OnDestroy()
    {
        Destroy(kioskRenderCameraInstance.gameObject);
        GameManager.Instance.SetPlayerInMenu(false);
    }
}
