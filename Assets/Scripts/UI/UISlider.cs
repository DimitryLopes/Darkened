using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class UISlider : Slider
{
    [Inject]
    private AudioManager audioManager;

    [SerializeField]
    private AudioKey sfxKey;

    public UnityEvent<float> onSnapValueChanged = new();

    protected override void Start()
    {
        base.Start();
        wholeNumbers = true;
        onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        audioManager.PlaySFX(sfxKey);
        onSnapValueChanged.Invoke(value);
    }
}