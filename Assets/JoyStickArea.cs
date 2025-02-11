using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
  public VirtualJoyStick joystick; // 🎯 Joystick 스크립트 참조
  private Vector2 initialPosition; // 🌟 초기 위치 저장

  private void Start()
  {
    if ( joystick != null )
    {
      initialPosition = joystick.GetInitialPosition(); // ✅ 초기 위치 저장
    }
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if ( joystick != null )
    {
      joystick.OnPointerDown(eventData); // ✅ 터치한 곳으로 조이스틱 이동
      joystick.OnDrag(eventData); // ✅ 터치 즉시 핸들도 반응하도록 수정!
    }
  }

  public void OnDrag(PointerEventData eventData)
  {
    if ( joystick != null )
    {
      joystick.OnDrag(eventData); // ✅ 터치 후 드래그 가능하게 함
    }
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    if ( joystick != null )
    {
      joystick.OnPointerUp(eventData); // ✅ 손을 떼면 조이스틱 숨김
      joystick.ResetToInitialPosition(initialPosition); // ✅ 초기 위치로 이동
    }
  }
}
