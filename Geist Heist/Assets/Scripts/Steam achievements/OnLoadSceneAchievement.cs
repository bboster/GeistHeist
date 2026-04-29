using NaughtyAttributes;
using UnityEngine;

public class OnLoadSceneAchievement : MonoBehaviour
{
    [Foldout("Achievements"), SerializeField] bool hasAchievement;
    [Foldout("Achievements"), SerializeField] AchievementManager.eAchievements WhatAcheivement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(hasAchievement)
        {
            AchievementManager.Instance.UnlockAchievement(WhatAcheivement);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
