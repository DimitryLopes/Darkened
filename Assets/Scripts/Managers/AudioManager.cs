using System.Collections.Generic;
using UnityEngine;

public class AudioManager 
{
    private const int MAXIMUM_SFX_SOURCES_ALLOWED = 6;

    private AudioDataBase audioDataBase;
    private AudioFactory audioFactory;
    private AudioSource bgmSource;
    private float sfxVolume = 1f;
    private float bgmVolume = 1f;

    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;

    private List<AudioSource> sfxSources = new List<AudioSource>();

    public AudioManager(AudioFactory audioFactory, AudioDataBase audioDataBase)
    {
        this.audioFactory = audioFactory;
        this.audioDataBase = audioDataBase;

        CreateBGMSource();
    }

    public void PlaySFX(AudioKey clipKey, Vector3 position = new())
    {
        AudioSource source = GetAvailableSFXSource();
        AudioInfo info = audioDataBase.GetAudioInfo(clipKey);
        if (source != null)
        {
            source.clip = info.AudioClip;
            source.volume = sfxVolume;
            source.Play();

            if (info.Origin == SoundOrigin.World)
            {
                source.transform.position = position;
            }
            else
            {
                source.transform.position = Camera.main.transform.position;
            }
        }
    }

    private void CreateBGMSource()
    {
        AudioSource bgmSource = GetNewAudioSource();
        bgmSource.transform.position = Camera.main.transform.position;
        this.bgmSource = bgmSource;
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        if(sfxSources.Count < MAXIMUM_SFX_SOURCES_ALLOWED)
        {
            AudioSource newSource = GetNewAudioSource();
            sfxSources.Add(newSource);
            newSource.loop = false;
            return newSource;
        }
        return null;
    }

    private AudioSource GetNewAudioSource()
    {
        AudioSource newSource = audioFactory.Create();
        return newSource;
    }

    public void ChangeSFXVolume(float volume)
    {
        sfxVolume = volume;
        foreach (var source in sfxSources)
        {
            source.volume = volume;
        }
    }

    public void ChangeBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = volume;
    }
}

public enum SoundOrigin
{
    World,
    UI
}

public enum SoundType
{
    SFX,
    BGM
}