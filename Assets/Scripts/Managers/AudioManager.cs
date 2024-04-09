using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioManager 
{
    private const int MAXIMUM_SFX_SOURCES_ALLOWED = 6;

    private AudioMixingSettings audioSettings;
    private AudioDataBase audioDataBase;
    private AudioFactory audioFactory;
    private AudioSource bgmSource;
    private float sfxVolume = 1f;
    private float bgmVolume = 1f;

    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;

    private List<AudioSource> sfxSources = new ();

    public AudioManager(AudioFactory audioFactory, AudioDataBase audioDataBase, AudioMixingSettings audioMixingSettings)
    {
        this.audioFactory = audioFactory;
        this.audioDataBase = audioDataBase;
        this.audioSettings = audioMixingSettings;

        CreateBGMSource();
    }

    public void PlaySFX(AudioKey clipKey, bool canHaveMultiple = true, Vector3 position = new())
    {
        AudioSource source = GetAvailableSFXSource(clipKey, canHaveMultiple);
        if (source != null)
        {
            AudioInfo info = audioDataBase.GetAudioInfo(clipKey);
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

    public void StopBGM()
    {
        FadeOutBGM();
    }

    public void PlayBGM(AudioKey key)
    {
        AudioInfo info = audioDataBase.GetAudioInfo(key);
        bgmSource.clip = info.AudioClip;
        bgmSource.Play();
        FadeInBGM();
    }

    private void FadeOutBGM(Action onFadeOutComplete = null)
    {
        LeanTweenAnimationData data = new LeanTweenAnimationData(bgmSource.gameObject, 0, -80, audioSettings.AudioFadeDuration,
            ChangeBGMMixerVolume, onFadeOutComplete);
        TweenUtils.DoTween(data);
    }
    
    private void FadeInBGM(Action onFadeInComplete = null)
    {
        LeanTweenAnimationData data = new LeanTweenAnimationData(bgmSource.gameObject, -80, 0, audioSettings.AudioFadeDuration,
            ChangeBGMMixerVolume, onFadeInComplete);
        TweenUtils.DoTween(data);
    }

    public void ChangeBGMMixerVolume(float value)
    {
        audioSettings.AudioMixer.SetFloat(audioSettings.BGMGroup + Constants.AudioParameters.MIXER_GROUP_VOLUME_PARAMETER, value);
    }

    public void ChangeSFXMixerVolume(float val)
    {
        audioSettings.AudioMixer.SetFloat(audioSettings.SFXGroup + Constants.AudioParameters.MIXER_GROUP_VOLUME_PARAMETER, val);
    }

    private void CreateBGMSource()
    {
        AudioSource bgmSource = GetNewAudioSource();
        bgmSource.transform.position = Camera.main.transform.position;
        this.bgmSource = bgmSource;
        bgmSource.outputAudioMixerGroup = audioSettings.BGMGroup;
    }

    private AudioSource GetAvailableSFXSource(AudioKey key, bool canHaveMultiple = true)
    {
        AudioSource source = null;
        foreach (var instantiatedSource in sfxSources)
        {
            if (!instantiatedSource.isPlaying)
            {
                source = instantiatedSource;
            }
            else if (!canHaveMultiple)
            {
                if(instantiatedSource.clip == audioDataBase.GetClip(key))
                {
                    return null;
                }
            }
        }

        if(sfxSources.Count < MAXIMUM_SFX_SOURCES_ALLOWED)
        {
            source = GetNewAudioSource();
            sfxSources.Add(source);
            source.outputAudioMixerGroup = audioSettings.SFXGroup;
            source.loop = false;
            return source;
        }
        return source;
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
    BGM,
    World,
    UI
}

public enum SoundType
{
    SFX,
    BGM
}