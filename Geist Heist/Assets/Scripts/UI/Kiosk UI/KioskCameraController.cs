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
    [SerializeField] public List<MeshRenderer> TetherModels;

    public void Initialize(List<LevelConfirmCollectableMesh> collectableInfo)
    {
        if(collectableObjectsList.Count != collectableInfo.Count)
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
        }
    }
}
