/*
 * Contributors: Toby
 * Creation:    4/21/2026
 * Last Edited: 4/21/206
 * Summary: Camera that renders the hats in a level.
 */

using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static LevelConfirmationVisualizer;

public class KioskCameraController : MonoBehaviour
{
    [Required] public Camera camera;
    [SerializeField] private List<GameObject> collectableObjectsList = new();
    public Dictionary<Collectable, GameObject> CollectableObjects = new();

    // i know theres only one tether in our game but scalability (also there may be an animation later that duplicates the tethers)
    [SerializeField] public List<GameObject> TetherModels;

    public void Initialize(List<LevelConfirmCollectableMesh> collectableInfo, Vector2 tetherPositionOffset, float tetherSizeMultiplier, MeshRenderer tetherMeshPrefab)
    {
        DontDestroyOnLoad(this.gameObject);

        if (collectableObjectsList.Count != collectableInfo.Count)
        {
            Debug.LogError("there is a different amount of hats in the camera.");
        }
        for(int i = 0; i < collectableInfo.Count; i++)
        {
            var info = collectableInfo[i];
            CollectableObjects.Add(info.collectable, collectableObjectsList[i]);
            collectableInfo[i].collectableObject = collectableObjectsList[i].gameObject;
            collectableInfo[i].meshFilter = collectableObjectsList[i].gameObject.GetComponent<MeshFilter>();
            collectableInfo[i].meshRenderer = collectableObjectsList[i].gameObject.GetComponent<MeshRenderer>();

            collectableObjectsList[i].transform.position += (Vector3) collectableInfo[i].positionOffset;
        }

        for (int i = 0; i < TetherModels.Count; i++)
        {
            TetherModels[i].transform.position += (Vector3)tetherPositionOffset;
            TetherModels[i].transform.localScale *= tetherSizeMultiplier;

            // tetherMeshPrefab is intentionally null on some kiosks (with tethers with complicated models)
            if (tetherMeshPrefab != null)
            {
                TetherModels[i].GetComponent<MeshFilter>().mesh = tetherMeshPrefab.GetComponent<MeshFilter>().sharedMesh;
                TetherModels[i].GetComponent<MeshRenderer>().materials = tetherMeshPrefab.sharedMaterials;
            }
        }

    }
}
