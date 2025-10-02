/*
 * Contributors: Toby
 * Creation: 9/30/25
 * Last Edited: 9/30/25
 * Summary: The players save file. Will get saved to JSON
 */


using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class SaveDataFile
{
    public HashSet<string> ScenesCompleted = new();
    public HashSet<int> CollectablesCollected = new(); // int correlates to the Collectable Enum
}
