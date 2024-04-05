using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Audio Mixing Settings", menuName = "Scriptable Objects/Audio Mixing Settings")]
public class AudioMixingSettings : ScriptableObject
{
    [SerializeField]
    private float audioFadeDuration;
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioMixerGroup bgmGroup;
    [SerializeField]
    private AudioMixerGroup sfxGroup;

    public AudioMixer AudioMixer => audioMixer;
    public AudioMixerGroup BGMGroup => bgmGroup;
    public AudioMixerGroup SFXGroup => sfxGroup;
    public float AudioFadeDuration => audioFadeDuration;
}
