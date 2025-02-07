using UnityEngine;

public class MushroomProjectile : MonoBehaviour
{
  public float explosionRadius = 2f; // 폭발 반경
  public float damage = 20f; // 피해량
  public float lifetime = 5f; // 투사체 생명 시간
  public GameObject explosionEffectPrefab; // 🔥 폭발 이펙트 프리팹

  private void Start()
  {
    Destroy(gameObject, lifetime); // 일정 시간 후 자동 삭제
  }

  private void OnCollisionEnter(Collision collision)
  {
    // 플레이어 충돌 시 데미지 적용
    if ( collision.gameObject.CompareTag("Player") )
    {
      PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
      if ( playerHealth != null )
      {
        playerHealth.OnDamage(damage, transform.position, Vector3.zero);
        Debug.Log($"💥 버섯 폭탄이 플레이어를 맞춤! {damage} 데미지");
      }
    }

    // 🔥 폭발 효과 실행
    Explode();
  }

  private void Explode()
  {
    Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
    foreach ( Collider nearbyObject in colliders )
    {
      if ( nearbyObject.CompareTag("Player") )
      {
        PlayerHealth playerHealth = nearbyObject.GetComponent<PlayerHealth>();
        if ( playerHealth != null )
        {
          playerHealth.OnDamage(damage, transform.position, Vector3.zero);
        }
      }
    }

    // 🔥 폭발 이펙트 생성
    if ( explosionEffectPrefab != null )
    {
      GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
      Destroy(explosionEffect, 2f); // 이펙트 2초 후 삭제
    }

    Destroy(gameObject);
  }
}
