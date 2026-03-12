/*
 * Utility script that spawns sound waves at positions. 
 * 
 * -Toby
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundWaveManager : Singleton<SoundWaveManager>
{
    [SerializeField] private GameObject SoundWavePrefab;

    [SerializeField] private Gradient DefaultGradient;



    /// <summary>
    /// Instantiates a new sound wave at specified position and destroys it after lifetime elapses
    /// </summary>
    public void CreateSoundWaveAtPosition(Vector3 position, Gradient colorOverLifetime, float maxRadius, float lifetime, bool collision)
    {
        ParticleSystem particle = InstantiateSoundWave(position, colorOverLifetime, maxRadius, lifetime, collision);
        particle.Play(false);

        if (particle.transform.parent != null)
            Destroy(particle.transform.parent.gameObject, lifetime);
        else
            Destroy(particle.gameObject);
    }

    /// <summary>
    /// Instantiates a new sound wave at specified position and destroys it after lifetime elapses
    /// </summary>
    public void CreateSoundWaveAtPosition(Vector3 position, SoundWaveProperties wave, float volume = 1)
    {
        if (wave.PlayMultipleWaves && wave.NumberOfWaves > 1)
        {
            CreateMultipleSoundWavesAtPositionFromProperties(position, wave);
        }
        else
        {
            CreateOneSoundWaveAtPositionFromProperties(position, wave);
        }

        if (wave.sound != null)
            AudioSource.PlayClipAtPoint(wave.sound, position, volume);
    }

    /// <summary>
    /// Instantiates a new sound wave at specified position and destroys it after lifetime elapses
    /// </summary>
    public void CreateSoundWaveAtPosition(Vector3 position, float maxRadius, float lifetime, bool collision)
    {
        CreateSoundWaveAtPosition(position, DefaultGradient, maxRadius, lifetime, collision);
    }

    public static void DrawSoundWaveGizmos(Vector3 position, SoundWaveProperties wave)
    {
        // big sphere
        Color color = wave.ColorOverLifetime.Evaluate(1);
        color.a = 1;

        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, wave.MaxRadius);

        // smaller inside sphere
        float time = Time.time % wave.MaxRadius;
    }

    #region Private Methods
    private ParticleSystem InstantiateSoundWave(Vector3 position, Gradient colorOverLifetime, float maxRadius, float lifetime, bool collision=false)
    {
        GameObject wave = Instantiate(SoundWavePrefab, position, Quaternion.identity);
        ParticleSystem particle = wave.GetComponentInChildren<ParticleSystem>();

        // guys particle systems are so annoying

        ParticleSystem.ColorOverLifetimeModule particleColorOverLifetime = particle.colorOverLifetime; // you need to make new variables or else it doesnt work
        particleColorOverLifetime.enabled = true;
        particleColorOverLifetime.color = colorOverLifetime;

        var main = particle.main;
        main.startSize = maxRadius;
        main.startLifetime = lifetime;

        var c = particle.collision;
        c.enabled = collision;

        return particle;

    }

    private ParticleSystem InstantiateSoundWave(Vector3 position, SoundWaveProperties waveProperties, bool collision = false)
    {
        GameObject wave = Instantiate(SoundWavePrefab, position, Quaternion.identity);
        ParticleSystem particle = wave.GetComponentInChildren<ParticleSystem>();

        // guys particle systems are so annoying

        ParticleSystem.ColorOverLifetimeModule particleColorOverLifetime = particle.colorOverLifetime; // you need to make new variables or else it doesnt work
        particleColorOverLifetime.enabled = true;
        particleColorOverLifetime.color = waveProperties.ColorOverLifetime;

        var main = particle.main;
        main.startSize = waveProperties.MaxRadius;
        main.startLifetime = waveProperties.Lifetime;

        var sizeOverLifetime = particle.sizeOverLifetime.size;
        sizeOverLifetime.curve = waveProperties.sizeOverLifetime;

        var c = particle.collision;
        c.enabled = collision;

        return particle;

    }

    /// <summary>
    /// Long ass function name but its pretty clear what its doing imo
    /// </summary>
    private void CreateOneSoundWaveAtPositionFromProperties(Vector3 position, SoundWaveProperties wave)
    {
        CreateSoundWaveAtPosition(position, wave.ColorOverLifetime, wave.MaxRadius, wave.Lifetime, wave.IsPerpetualSound);
    }

    private void CreateMultipleSoundWavesAtPositionFromProperties(Vector3 position, SoundWaveProperties wave, bool collision = false)
    {
        ParticleSystem ps = InstantiateSoundWave(position, wave, wave.IsPerpetualSound);
        var template = ps.emission.GetBurst(0);
        float time = 0;

        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[wave.NumberOfWaves];
        bursts[0] = template;
        for (int i = 1; i < bursts.Length; i++)
        {
            time = (float)i * wave.SecondsBetweenWaves;
            ParticleSystem.Burst temp = new ParticleSystem.Burst(time, template.count, template.cycleCount, template.repeatInterval);
            bursts[i] = temp;
        }
        ps.emission.SetBursts(bursts);

        // wave.Lifetime + time = (time each sound wave lasts for) + (time that last sound wave gets played)
        if (ps.gameObject.transform.parent != null)
            Destroy(ps.transform.parent.gameObject, wave.Lifetime + time);
        else
            Destroy(ps.gameObject, wave.Lifetime + time);

        #endregion

    }
}