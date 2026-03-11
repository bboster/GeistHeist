using UnityEngine;
using NaughtyAttributes;

public class CustomOutline : MonoBehaviour
{
    private Material[] defaultMaterials;
    private Material[] outlineMaterials;    
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField, ShowAssetPreview(16, 16)] private Material outlineMaterial;

    private void Start()
    {
        defaultMaterials = meshRenderer.materials;
        outlineMaterials = new Material[defaultMaterials.Length + 1];
        outlineMaterials[outlineMaterials.Length - 1] = outlineMaterial;

        for(int i = 0; i < defaultMaterials.Length; i++)
        {
            outlineMaterials[i] = defaultMaterials[i];
        }
    }

    /// <summary>
    /// Swaps the materials list on the object to include the outline
    /// </summary>
    public void AddMaterial()
    {
        meshRenderer.materials = outlineMaterials;
    }

    /// <summary>
    /// Swaps the materials list on the object to its default form
    /// </summary>
    public void RemoveMaterial()
    {
        meshRenderer.materials = defaultMaterials;
    }
}
