using UnityEngine;
using Zenject;

public class AudioFactory
{
    private readonly AudioSource audioSourcePrefab;
    private readonly DiContainer container;

    public AudioFactory(DiContainer container, AudioSource audioSourcePrefab)
    {
        this.audioSourcePrefab = audioSourcePrefab;
        this.container = container;
    }

    public AudioSource Create()
    {
        AudioSource audioSource = container.InstantiatePrefabForComponent<AudioSource>(audioSourcePrefab);
        return audioSource;
    }
}