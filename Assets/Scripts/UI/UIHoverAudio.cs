using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class UIHoverAudio : MonoBehaviour, IPointerEnterHandler
{
    [Inject]
    private AudioManager audioManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioManager.PlaySFX(AudioKey.UI_hover_default);
    }
}
