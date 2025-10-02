/*
 * Contributors: Toby
 * Creation: 9/30/25
 * Last Edited: 9/30/25
 * Summary: Collectable object. saves to player data.
 */

using NaughtyAttributes;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Compilation;
using System.Threading.Tasks;
#endif

public class OptionalCollectable : MonoBehaviour
{
    [SerializeField] private Collectable ThisCollectable;
    [SerializeField] private GameObject CollectionParticlePrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        // TODO: animation (?)

        StaticUtilities.PlayAndDestroyParticle(CollectionParticlePrefab, transform.position);

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    #region Debug Tools 
#if UNITY_EDITOR
    private string filePath => Application.dataPath + "/Scripts/Environment/OptionalCollectable.cs";

    [Button("Add new collectable as enum")]
    private async void RegisterAsEnum()
    {
        // code that rewrites the script itself. lol

        var currentEnums = Enum.GetNames(typeof(Collectable));
        string name = new string(
            this.gameObject.name
            .Replace(" ","_")
            .Where(c => !" $.,;'()!@#$%^&*()/\\\"[]-".Contains(c))
            .ToArray()
            );
        Debug.Log("Saving enum as " + name);

        if (currentEnums.Contains(name))
        {
            Debug.Log("Enum already exists");
            return;
        }

        // Sorry the path is hardcoded idk how to fix that
        string currentContent = File.ReadAllText(filePath);

        int index = currentContent.LastIndexOf("}");
        string newContent = currentContent.Insert(index, $"\t{name},\n");

        File.WriteAllText(filePath, newContent);

        // Force code to recompile
        CompilationPipeline.RequestScriptCompilation();

        Debug.Log("Script recompilation requested.");

        do
        {
            await Task.Delay(3000);
        }
        while (EditorApplication.isCompiling);
        await Task.Delay(3000);

        ThisCollectable = (Collectable)((int)currentEnums.Count() + 1);

        Debug.Log("Saved enum as " + name);

    }

    [Button("Delete this enum")]
    private async void DeleteThisEnum()
    {
        if(ThisCollectable == 0) {
            Debug.Log("Cant delete default enum");
            return;
        }

        string currentContent = File.ReadAllText(filePath);

        int index = currentContent.LastIndexOf("}");
        string newContent = currentContent.Replace($"{ThisCollectable.ToString()},", "");

        File.WriteAllText(filePath, newContent);

        ThisCollectable = 0;

        // Force code to recompile
        CompilationPipeline.RequestScriptCompilation();

        Debug.Log("Script recompilation requested.");

        do
        {
            await Task.Delay(3000);
        }
        while (EditorApplication.isCompiling);

        await Task.Delay(3000);

        Debug.Log("done recompiling");

    }
#endif
#endregion
}


// do not move this enum from being at the BOTTOM of THIS script
// This enum writes itself using debug tools
public enum Collectable
{
    None,
}