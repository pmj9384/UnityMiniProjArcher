using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
  public RectTransform background; // 조이스틱 배경
  public RectTransform handle; // 조이스틱 핸들
  private float joystickRadius; // 조이스틱 반경
  private Vector2 initialPosition; // 🌟 초기 위치 저장
  public Vector2 Input { get; private set; }

  private CanvasGroup canvasGroup; // 조이스틱 숨기기

  private void Start()
  {
    joystickRadius = background.rect.width * 0.5f;

    // ✅ 초기 위치 저장
    initialPosition = background.anchoredPosition;

    // ✅ CanvasGroup 추가 (없으면 자동 추가)
    canvasGroup = background.GetComponent<CanvasGroup>();
    if ( canvasGroup == null )
    {
      canvasGroup = background.gameObject.AddComponent<CanvasGroup>();
    }

    HideJoystick(); // 처음엔 숨김
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if ( RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint) )
    {
      background.anchoredPosition = localPoint; // 터치 위치로 조이스틱 이동
      handle.anchoredPosition = Vector2.zero; // 핸들 초기화
      ShowJoystick();
    }
  }

  public void OnDrag(PointerEventData eventData)
  {
    if ( RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out Vector2 position) )
    {
      position = Vector2.ClampMagnitude(position, joystickRadius); // 핸들은 배경 내부로만 이동
      handle.anchoredPosition = position;
      this.Input = position.normalized;
    }
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    HideJoystick();
  }

  private void ShowJoystick()
  {
    canvasGroup.alpha = 1; // 조이스틱 보이기
    canvasGroup.blocksRaycasts = true;
  }

  private void HideJoystick()
  {
    canvasGroup.alpha = 0; // 조이스틱 숨기기
    canvasGroup.blocksRaycasts = false;
    handle.anchoredPosition = Vector2.zero;
    this.Input = Vector2.zero;
  }

  // ✅ 초기 위치 반환 함수
  public Vector2 GetInitialPosition()
  {
    return initialPosition;
  }

  // ✅ 초기 위치로 이동하는 함수
  public void ResetToInitialPosition(Vector2 startPos)
  {
    background.anchoredPosition = startPos;
    handle.anchoredPosition = Vector2.zero;
    HideJoystick(); // 🌟 초기 위치로 돌아가면서 숨김
  }
  public void SetJoystickActive(bool isActive)
  {
    gameObject.SetActive(isActive);
  }

  public void ResetInput()
  {
    Input = Vector2.zero;
    handle.anchoredPosition = Vector2.zero;
  }
}
