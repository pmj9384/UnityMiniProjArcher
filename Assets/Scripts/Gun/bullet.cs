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

  private void OnTriggerEnter(Collider other)
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

  private void OnCollisionEnter(Collision collision)
  {
    if ( collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("NatWall") || collision.gameObject.CompareTag("Door") ) // ✅ 벽 감지
    {
      if ( bounceCount < maxBounces )
      {
        ContactPoint contact = collision.contacts[0];
        Vector3 normal = contact.normal; // 충돌 표면의 법선 벡터
        Vector3 incomingDir = rb.velocity.normalized; // 화살의 진행 방향

        // 🔥 자연스러운 반사각 계산 (기존 Reflect 보정)
        Vector3 reflectDir = Vector3.Reflect(incomingDir, normal);
        reflectDir.y *= 0.3f;
        // 🔄 반사각이 너무 가파르면 조정
        float dot = Vector3.Dot(reflectDir, normal);
        if ( dot > -0.1f ) // 너무 벽을 타고 튀는 경우
        {
          reflectDir = ( reflectDir + normal * 0.5f ).normalized; // 자연스럽게 반사되도록 보정
        }

        rb.velocity = reflectDir * speed; // 🔥 반사 방향 이동
        transform.rotation = Quaternion.LookRotation(reflectDir); // 🔄 방향 회전 보정

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
