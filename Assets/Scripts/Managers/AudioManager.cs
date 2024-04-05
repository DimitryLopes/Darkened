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

    private List<AudioSource> sfxSources = new List<AudioSource>();

    public AudioManager(AudioFactory audioFactory, AudioDataBase audioDataBase, AudioMixingSettings audioMixingSettings)
    {
        this.audioFactory = audioFactory;
        this.audioDataBase = audioDataBase;
        this.audioSettings = audioMixingSettings;

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

    private void FadeOutBGM(AudioKey key = AudioKey.default_key, UnityAction<AudioKey> onFadeOutComplete = null)
    {

        LeanTween.value(bgmSource.gameObject, 0, -80, audioSettings.AudioFadeDuration)
            .setOnUpdate((float val) =>
            {
                audioSettings.AudioMixer.SetFloat(audioSettings.BGMGroup + Constants.AudioParameters.MIXER_GROUP_VOLUME_PARAMETER, val);
            })
            .setOnComplete(() =>
            {
                onFadeOutComplete?.Invoke(key);
            });
    }

    private void FadeInBGM(UnityAction onFadeInComplete = null)
    {
        LeanTween.value(bgmSource.gameObject, -80, 0, audioSettings.AudioFadeDuration)
            .setOnUpdate((float val) =>
            {
                audioSettings.AudioMixer.SetFloat(audioSettings.BGMGroup + Constants.AudioParameters.MIXER_GROUP_VOLUME_PARAMETER, val);
            })
            .setOnComplete(() =>
            {
                onFadeInComplete?.Invoke();
            });
    }

    private void CreateBGMSource()
    {
        AudioSource bgmSource = GetNewAudioSource();
        bgmSource.transform.position = Camera.main.transform.position;
        this.bgmSource = bgmSource;
        bgmSource.outputAudioMixerGroup = audioSettings.BGMGroup;
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
            newSource.outputAudioMixerGroup = audioSettings.SFXGroup;
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
    BGM,
    World,
    UI
}

public enum SoundType
{
    SFX,
    BGM
}