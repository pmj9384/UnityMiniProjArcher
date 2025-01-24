using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;
    private float joystickRadius;
    
    public Vector2 Input { get; private set; }

    private void Start()
    {
        joystickRadius = background.rect.width * 0.5f;
    }

    private void Update()
    {
       // Debug.Log(Input);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out Vector2 position))
        {
            position = Vector2.ClampMagnitude(position, joystickRadius);
            handle.anchoredPosition = position;
            Input = position.normalized;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
