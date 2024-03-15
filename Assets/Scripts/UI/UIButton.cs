using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class UIButton : MonoBehaviour, IPointerEnterHandler
{
    [Inject]
    private AudioManager audioManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioManager.PlaySFX(AudioKey.UI_hover_default);
    }
}
