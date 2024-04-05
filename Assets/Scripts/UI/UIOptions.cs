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
        sfxSlider.value = audioManager.SFXVolume * 10;
        bgmSlider.value = audioManager.BGMVolume * 10;
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
