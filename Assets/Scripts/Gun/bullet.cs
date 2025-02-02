using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
  public float speed = 50f;
  public float damage;
  public float lifeTime = 3f;

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

    // 🔥 일정 시간이 지나면 풀로 되돌리기
    Invoke(nameof(ReturnToPool), lifeTime);
  }

  public void ApplyStatusEffect(IStatusEffect effect)
  {
    statusEffects.Add(effect);
  }

  private void OnTriggerEnter(Collider other)
  {
    if ( other.CompareTag("Enemy") || other.CompareTag("Player") )
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

      ReturnToPool(); // 🔥 풀로 되돌리기
    }
    else if ( other.CompareTag("Wall") )
    {
      ReturnToPool();
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
