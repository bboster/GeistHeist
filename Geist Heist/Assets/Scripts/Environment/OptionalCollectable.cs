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
using UnityEngine;

public class OptionalCollectable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button("Add new collectable as enum")]
    private void RegisterAsEnum()
    {
        var currentEnums = Enum.GetNames(typeof(Collectable));
        var name = this.gameObject.name;
        foreach (var s in currentEnums)
        {
            Debug.Log(s);
        }
        if (currentEnums.Contains(name))
        {
            Debug.Log("Enum already exists");
            return;
        }

        // Get the path to this script file
        string scriptPath = Application.dataPath + "/OptionalCollectable.cs";

        // Read the current content of the script
        string currentContent = File.ReadAllText(scriptPath);

        // Modify the content (e.g., add a comment)
        string newContent = currentContent + "\n// This line was added programmatically.\n";

        // Write the new content back to the file, overwriting the old content
        File.WriteAllText(scriptPath, newContent);

        Debug.Log("Script modified and saved.");
    }
}



public enum Collectable
{
    TestName1,
    TestName2,
}