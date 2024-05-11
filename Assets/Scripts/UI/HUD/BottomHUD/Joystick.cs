using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform joystick;
    [SerializeField]
    public RectTransform joystickBackground;
    private bool isDragging;
    private Vector2 direction;

    public Vector3 Direction => direction;

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out localPoint);

            // Calculate the distance from the center of the boundary to the current position
            float distance = Vector2.Distance(Vector2.zero, localPoint);

            // Calculate the clamped position within the circular boundary
            Vector2 clampedPosition = localPoint.normalized * Mathf.Min(distance, joystickBackground.rect.width / 2f);

            joystick.localPosition = clampedPosition;

            direction = clampedPosition.normalized;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        joystick.anchoredPosition = Vector2.zero;
        direction = Vector2.zero;
    }
}