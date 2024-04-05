using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIOptions : MonoBehaviour
{
    [Inject]
    private AudioManager audioManager;

    [SerializeField]
    private UISlider sfxSlider;
    [SerializeField]
    private UISlider bgmSlider;

    private void Start()
    {
        sfxSlider.value = audioManager.SFXVolume;
        bgmSlider.value = audioManager.BGMVolume;
        sfxSlider.onSnapValueChanged.AddListener(ChangeSFXVolume);
        bgmSlider.onSnapValueChanged.AddListener(ChangeBGMVolume);
    }

    public void ChangeBGMVolume(float volume)
    {
        audioManager.ChangeBGMVolume(volume / 10);
    }

    public void ChangeSFXVolume(float volume)
    {
        audioManager.ChangeSFXVolume(volume / 10);
    }
}
