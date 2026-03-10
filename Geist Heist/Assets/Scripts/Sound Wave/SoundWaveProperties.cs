/*
 * Serialized object that represents the settings for a sound wave particle.
 * 
 * Has a method to create a new sound wave from SoundWaveManager
 * 
 * -Toby
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using NaughtyAttributes;

[System.Serializable]
public class SoundWaveProperties
{
    [Min(0)]
    [Tooltip("How far the sound wave will travel")]
    public float MaxRadius = 1;

    [Min(0)]
    [Tooltip("How long it takes each sound wave to reach the max radius")]
    public float Lifetime = 5;

    [Tooltip("If true, play sound waves multiple times")]
    public bool PlayMultipleWaves=false;

    [ShowIf("PlayMultipleWaves")]
    [Tooltip("How many times sound wave will play")]
    [AllowNesting]
    public int NumberOfWaves=1;

    [ShowIf("PlayMultipleWaves")]
    [AllowNesting]
    public float SecondsBetweenWaves=0.25f;

    public Gradient ColorOverLifetime = new Gradient()
    {
        // default example gradient because its important that the color becomes less visible:

        colorKeys = new GradientColorKey[2] {
            new GradientColorKey(new Color(1, 1, 1), 0),
            new GradientColorKey(new Color(1, 1, 1), 1f)
        },
        
        alphaKeys = new GradientAlphaKey[2] {
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(0, 1)
        }
    };

    [CurveRange(0, 0, 1, 1, EColor.Red)]
    [Tooltip("Size of the particle sphere")]
    public AnimationCurve sizeOverLifetime = new AnimationCurve();

    public bool IsPerpetualSound;

    [Tooltip("Plays nothing if null. Clip that plays when sound wave is created.")]
    public AudioClip sound;

    public void PlayAtPosition(Vector3 position, float volume = 1)
    {
        SoundWaveManager.Instance.CreateSoundWaveAtPosition(position, this, volume);
    }

}
