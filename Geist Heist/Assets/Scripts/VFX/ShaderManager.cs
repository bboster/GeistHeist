/*
 * Contributors: Toby
 * Creation:    10/7/2025
 * Last Edited: 10/7/2025
 * 
 * Summary: Sends data to shaders that need data every frame.
 */

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : DontDestroyOnLoadSingleton<ShaderManager>
{
    //[SerializeField] private List<Material> playerPositionListeners;

    private static readonly int playerPositionID = Shader.PropertyToID("_Player_Position");

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.CurrentObject == null) return;

        Shader.SetGlobalVector(playerPositionID, PlayerManager.Instance.CurrentObject.transform.position);
    }
}
