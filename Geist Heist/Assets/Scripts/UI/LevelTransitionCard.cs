/*
 * Contributors: Toby
 * Creation Date: 10/27/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Brief animation that plays between levels
 */

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class LevelTransitionCard : MonoBehaviour
{
    [SerializeField] private float fadeInSeconds = 0.5f;
    [SerializeField] private float waitingSeconds = 2;
    [SerializeField] private float fadeOutSeconds = 0.5f;

    [SerializeField, Required] CanvasGroup group;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartTransition(int sceneToLoad)
    {
        
    }

    private IEnumerator FadeIn()
    {

    }
}
