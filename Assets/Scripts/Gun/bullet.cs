using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
  public float speed = 50f;
  public float damage;
  public float lifeTime = 3f;

  private int maxBounces = 0; // 🔥 최대 튕길 횟수 (기본 0)
  private int bounceCount = 0; // 현재 튕긴 횟수

  private IObjectPool<GameObject> pool;
  private List<IStatusEffect> statusEffects = new List<IStatusEffect>();
  private Rigidbody rb;

  private void Awake()
  {
    rb = GetComponent<Rigidbody>();
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

  public void EnableBounceShot(int maxBounce) // 🔥 튕길 수 있는 횟수 설정
  {
    maxBounces = maxBounce;
    bounceCount = 0; // 초기화
  }

  private void OnTriggerEnter(Collider other)
  {
    if ( other.CompareTag("Player") ) // 🔥 플레이어는 맞아도 무시 (튕기지 않음)
    {
      return;
    }

    if ( other.CompareTag("Enemy") || other.CompareTag("GrimReaper") || other.CompareTag("AirUnit") ) // 🔥 적을 맞추면 바로 사라짐
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

      ReturnToPool(); // 🔥 적을 맞추면 삭제
    }
    else if ( other.CompareTag("Wall") ) // 🔥 벽에 맞으면 튕기기
    {
      if ( bounceCount < maxBounces )
      {
        Vector3 normal = other.ClosestPoint(transform.position) - transform.position;
        normal.Normalize();
        Vector3 reflectDir = Vector3.Reflect(rb.velocity.normalized, normal);

        rb.velocity = reflectDir * speed; // 🔥 반사 후 동일 속도 유지

        // 🔥 🔄 반사 방향으로 화살 회전 업데이트
        transform.rotation = Quaternion.LookRotation(reflectDir);

        bounceCount++;
      }
      else
      {
        ReturnToPool(); // 최대 튕김 횟수 초과 시 삭제
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
    // 🔥 총알 초기화 (효과 삭제)
    statusEffects.Clear();
    CancelInvoke(nameof(ReturnToPool)); // 타이머 취소 (다시 풀로 반환될 때 초기화)
  }

  public void ClearStatusEffects()
  {
    statusEffects.Clear();
  }
}
