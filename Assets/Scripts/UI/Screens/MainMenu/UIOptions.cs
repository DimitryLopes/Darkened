using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIOptions : MonoBehaviour
{
    [Inject]
    private PersistenceManager persistenceManager;
    [Inject]
    private AudioManager audioManager;

    [SerializeField]
    private Button saveButton;
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
        saveButton.onClick.AddListener(SavePrefs);
    }

    public void ChangeBGMVolume(float volume)
    {
        audioManager.ChangeBGMVolume(volume / 10);
    }

    public void ChangeSFXVolume(float volume)
    {
        audioManager.ChangeSFXVolume(volume / 10);
    }

    public void SavePrefs()
    {
        persistenceManager.ChangeBGMPreferences(audioManager.BGMVolume);
        persistenceManager.ChangeSFXPreferences(audioManager.SFXVolume);
    }
}
