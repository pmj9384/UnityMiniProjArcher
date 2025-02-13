using UnityEngine;

public class EarthGolemProjectile : MonoBehaviour
{
  public float damage = 20f; // 돌 투사체 데미지
  public float explosionRadius = 2f; // 폭발 반경 (필요할 경우)
  public float lifetime = 5f; // 투사체 생명 시간

  private void Start()
  {
    Destroy(gameObject, lifetime); // 일정 시간 후 자동 삭제
  }

  private void OnCollisionEnter(Collision collision)
  {
    string tag = collision.gameObject.tag;

    if ( tag == "Wall" || tag == "NatWall" )
    {
      //      Debug.Log("🪨 돌이 벽/바닥에 부딪힘 → 삭제됨!");
      Destroy(gameObject);
      return;
    }

    if ( tag == "Player" )
    {
      PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
      if ( playerHealth != null )
      {
        playerHealth.OnDamage(damage, transform.position, Vector3.zero);
        Debug.Log($"💥 돌이 플레이어를 맞춤! {damage} 데미지");
      }
      Destroy(gameObject);
      return;
    }
  }
}
