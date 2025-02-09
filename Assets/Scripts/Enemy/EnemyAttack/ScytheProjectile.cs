using UnityEngine;

public class ScytheProjectile : MonoBehaviour
{
  public float speed = 10f;  // 낫이 앞으로 나가는 속도
  public float returnSpeed = 12f; // 되돌아오는 속도
  public float rotationSpeed = 360f; // 낫의 회전 속도 (초당 회전 각도)

  private Vector3 direction;
  private bool returning = false;
  private Transform reaper; // Grim Reaper 참조
  private Rigidbody rb;
  public float damage = 20f; // 낫의 공격력

  public void Initialize(Vector3 shootDirection, Transform reaperTransform)
  {
    direction = shootDirection.normalized;
    reaper = reaperTransform; // 🔥 Reaper 설정

    rb = GetComponent<Rigidbody>();
    if ( rb == null )
    {
      Debug.LogError("❌ Rigidbody가 없음! 자동 추가");
      rb = gameObject.AddComponent<Rigidbody>();
    }

    rb.useGravity = false;
    rb.isKinematic = false;
    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    rb.velocity = direction * speed;

    Debug.Log($"🚀 낫이 발사됨! 속도: {speed}, 방향: {direction}, 목표: {reaper?.name}");
  }

  private void FixedUpdate()
  {
    RotateProjectile();

    if ( returning && reaper != null )
    {
      Vector3 returnDirection = ( reaper.position - transform.position ).normalized;
      rb.velocity = returnDirection * returnSpeed;

      Debug.Log($"🔄 낫이 {reaper.name}에게 되돌아가는 중! 속도: {rb.velocity}");

      if ( Vector3.Distance(transform.position, reaper.position) < 0.5f )
      {
        rb.velocity = Vector3.zero;
        Debug.Log("⚡ 낫이 Grim Reaper에게 돌아옴, 제거됨!");
        Destroy(gameObject);
      }
    }
  }

  private void RotateProjectile()
  {
    transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
  }

  private void OnTriggerEnter(Collider other)
  {
    if ( other.CompareTag("NatWall") )
    {
      returning = true; // ✅ 벽에 닿으면 되돌아가기 시작
      Debug.Log("🔄 낫이 벽에 부딪힘 → 되돌아가기 시작!");
    }

    if ( returning && other.CompareTag("GrimReaper") )
    {
      Debug.Log("⚡ 낫이 Grim Reaper에게 돌아옴, 제거됨!");
      Destroy(gameObject);
    }

    if ( other.CompareTag("Player") )
    {
      PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
      if ( playerHealth != null )
      {
        playerHealth.OnDamage(damage, other.transform.position, Vector3.zero);
        Debug.Log($"💥 낫이 플레이어를 명중! {damage} 데미지 적용");
        Destroy(gameObject);
      }
    }
  }
}
