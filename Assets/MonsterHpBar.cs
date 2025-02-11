using UnityEngine;
using UnityEngine.UI;

public class MonsterHpBar : MonoBehaviour
{
  public Transform monster; // 몬스터 Transform
  public Vector3 offset = new Vector3(0, 2f, 0); // 체력바 위치 (머리 위로 조정)

  private Slider healthSlider;
  private Camera cam;

  private void Start()
  {
    cam = Camera.main;

    // ✅ Slider 찾기
    healthSlider = GetComponentInChildren<Slider>();
    if ( healthSlider == null )
    {
      Debug.LogError("❌ MonsterHpBar: 체력바 Slider를 찾을 수 없습니다!");
    }
  }

  private void LateUpdate()
  {
    if ( monster == null ) return;

    // ✅ 체력바 위치를 몬스터 머리 위로 유지
    transform.position = monster.position + offset;

    // ✅ 항상 카메라를 바라보도록 설정 (UI 회전 방지)
    transform.LookAt(transform.position + cam.transform.forward);
    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
  }

  // ✅ 체력 업데이트 함수
  public void UpdateHealthBar(float currentHp, float maxHp)
  {
    if ( healthSlider != null )
    {
      healthSlider.value = Mathf.Clamp01(currentHp / maxHp);
    }
  }
}
