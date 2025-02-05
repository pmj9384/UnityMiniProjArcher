using UnityEngine;

public class FireProjectile : MonoBehaviour
{
  public float damage = 10f; // 🔥 불꽃 데미지

  private void OnTriggerEnter(Collider other)
  {
    if ( other.CompareTag("Player") )
    {
      PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
      if ( playerHealth != null )
      {
        playerHealth.OnDamage(damage, transform.position, Vector3.zero);
        Debug.Log($"🔥 불꽃이 플레이어에게 명중! {damage} 데미지");
      }
    }
  }
}
