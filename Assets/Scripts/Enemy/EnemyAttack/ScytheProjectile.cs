using UnityEngine;

public class ScytheProjectile : MonoBehaviour
{
  public float speed = 10f;  // 낫이 앞으로 나가는 속도
  public float returnSpeed = 12f; // 되돌아오는 속도
  public float rotationSpeed = 360f; // 낫의 회전 속도 (초당 회전 각도)

  private Vector3 direction;
  private bool returning = false;
  private Transform reaper;
  private Rigidbody rb;
  public float damage = 20f; // 낫의 공격력

  public void Initialize(Vector3 shootDirection)
  {
    direction = shootDirection.normalized;

    // Grim Reaper를 태그 기반으로 찾기
    GameObject reaperObject = GameObject.FindGameObjectWithTag("GrimReaper");
    if ( reaperObject != null )
    {
      reaper = reaperObject.transform;
      Debug.Log($"✅ Grim Reaper 찾음: {reaper.name}");
    }
    else
    {
      Debug.LogError("❌ Grim Reaper를 찾을 수 없음! 낫이 되돌아가지 못할 수 있음.");
    }

    rb = GetComponent<Rigidbody>();

    if ( rb == null )
    {
      Debug.LogError("❌ Rigidbody가 없음! 자동 추가");
      rb = gameObject.AddComponent<Rigidbody>();
    }

    rb.useGravity = false; // 중력 영향 제거
    rb.isKinematic = false; // 물리 적용 활성화
    rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 빠른 충돌 감지
    rb.velocity = direction * speed; // 이동 시작

    Debug.Log($"🚀 낫이 발사됨! 속도: {speed}, 방향: {direction}, Rigidbody 속도: {rb.velocity}");
  }

  private void FixedUpdate()
  {
    // 낫이 회전하도록 처리
    RotateProjectile();

    if ( returning && reaper != null )
    {
      Vector3 returnDirection = ( reaper.position - transform.position ).normalized;
      rb.velocity = returnDirection * returnSpeed;

      // Grim Reaper에 가까워지면 자동 삭제
      if ( Vector3.Distance(transform.position, reaper.position) < 0.5f )
      {
        rb.velocity = Vector3.zero; // 속도 리셋
        Debug.Log("⚡ 낫이 Grim Reaper에게 돌아옴, 제거됨!");
        Destroy(gameObject);
      }
    }
  }

  private void RotateProjectile()
  {
    // 낫이 Y축 기준으로 계속 회전
    transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
  }

  private void OnTriggerEnter(Collider other)
  {
    if ( other.CompareTag("Wall") )
    {
      // 벽에 닿으면 즉시 되돌아가기 시작
      returning = true;
      Debug.Log("🔄 낫이 벽에 부딪힘 → 되돌아가기 시작!");

      if ( reaper != null )
      {
        Vector3 returnDirection = ( reaper.position - transform.position ).normalized;
        rb.velocity = returnDirection * returnSpeed;
      }
      else
      {
        Debug.LogError("❌ Grim Reaper가 설정되지 않음! 낫이 돌아갈 수 없음.");
      }
    }

    if ( returning && other.CompareTag("GrimReaper") )
    {
      // Grim Reaper에 도착하면 삭제
      Debug.Log("⚡ 낫이 Grim Reaper에게 돌아옴, 제거됨!");
      Destroy(gameObject);
    }

    if ( other.CompareTag("Player") )
    {
      // 플레이어에게 데미지 적용
      PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
      if ( playerHealth != null )
      {
        playerHealth.OnDamage(damage, other.transform.position, Vector3.zero);
        Debug.Log($"💥 낫이 플레이어를 명중! {damage} 데미지 적용");
        Destroy(gameObject); // 플레이어 공격 후 낫 삭제
      }
    }
  }
}
