using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
  public float speed = 50f;
  public float damage;
  public float lifeTime = 3f;

  private int maxBounces = 0;
  private int bounceCount = 0;

  private IObjectPool<GameObject> pool;
  private List<IStatusEffect> statusEffects = new List<IStatusEffect>();
  private Rigidbody rb;

  private void Awake()
  {
    rb = GetComponent<Rigidbody>();
    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 🔥 빠른 속도에서 충돌 감지 보장
  }

  public void Launch(Vector3 direction, IObjectPool<GameObject> objectPool)
  {
    rb.velocity = direction * speed;
    pool = objectPool;
    Invoke(nameof(ReturnToPool), lifeTime);
  }

  public void ApplyStatusEffect(IStatusEffect effect)
  {
    statusEffects.Add(effect);
  }

  public void EnableBounceShot(int maxBounce)
  {
    maxBounces = maxBounce;
    bounceCount = 0;
  }

  private void OnTriggerEnter(Collider other) // ❌ Trigger 제거 (사용하지 않음)
  {
    if ( other.CompareTag("Player") ) return;

    if ( other.CompareTag("Enemy") || other.CompareTag("GrimReaper") || other.CompareTag("AirUnit") )
    {
      LivingEntity entity = other.GetComponent<LivingEntity>();
      if ( entity != null )
      {
        entity.OnDamage(damage, entity.transform.position, -transform.forward);
        foreach ( var effect in statusEffects )
        {
          entity.StartCoroutine(effect.ApplyEffect(entity));
        }
      }
      ReturnToPool();
    }
  }

  private void OnCollisionEnter(Collision collision) // ✅ 물리 충돌 감지
  {
    if ( collision.gameObject.CompareTag("Wall") ) // 벽에 부딪힌 경우
    {
      if ( bounceCount < maxBounces )
      {
        ContactPoint contact = collision.contacts[0]; // 가장 첫 번째 충돌 지점
        Vector3 reflectDir = Vector3.Reflect(rb.velocity.normalized, contact.normal);

        rb.velocity = reflectDir * speed; // 반사 방향으로 이동
        transform.rotation = Quaternion.LookRotation(reflectDir);

        bounceCount++;
      }
      else
      {
        ReturnToPool();
      }
    }
  }

  private void ReturnToPool()
  {
    if ( pool != null )
    {
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      gameObject.SetActive(false);
      pool.Release(gameObject);
    }
  }

  private void OnDisable()
  {
    statusEffects.Clear();
    CancelInvoke(nameof(ReturnToPool));
  }

  public void ClearStatusEffects()
  {
    statusEffects.Clear();
  }
}
