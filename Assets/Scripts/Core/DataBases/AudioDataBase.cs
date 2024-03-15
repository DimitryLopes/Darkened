using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataBase", menuName = "Scriptable Objects/Data Bases/Audio Data Base")]
public class AudioDataBase : ScriptableObject
{
    [SerializeField]
    private List<AudioInfo> audioInfoList;

    private Dictionary<AudioKey, AudioInfo> audioInfo = new Dictionary<AudioKey, AudioInfo>();

    public void SetUp()
    {
        foreach(AudioInfo info in audioInfoList)
        {
            audioInfo.Add(info.AudioKey, info);
        }
    }

    public AudioInfo GetAudioInfo(AudioKey clipKey)
    {
        AudioInfo info = audioInfo[clipKey];
        return info;
    }
}

[Serializable]
public struct AudioInfo
{
    [SerializeField]
    private AudioKey audioKey;
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private SoundOrigin origin;

    public AudioClip AudioClip => audioClip;
    public SoundOrigin Origin => origin;
    public AudioKey AudioKey => audioKey;

}
