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

  public void Launch(Vector3 direction, IObjectPool<GameObject> objectPool)
  {
    Rigidbody rb = GetComponent<Rigidbody>();
    if ( rb != null )
    {
      rb.velocity = direction * speed;
    }

    pool = objectPool;
    Invoke(nameof(ReturnToPool), lifeTime);
  }

  public void ApplyStatusEffect(IStatusEffect effect)
  {
    statusEffects.Add(effect);
  }

  private void OnTriggerEnter(Collider other)
  {
    if ( other.CompareTag("Enemy") || other.CompareTag("Player") ) // ✅ LivingEntity가 적용될 수 있도록 수정
    {
      LivingEntity entity = other.GetComponent<LivingEntity>();
      if ( entity != null )
      {
        entity.OnDamage(damage, entity.transform.position, -transform.forward);

        foreach ( var effect in statusEffects )
        {
          //effect.ApplyEffect(entity);  // ✅ LivingEntity에 효과 적용
          entity.StartCoroutine(effect.ApplyEffect(entity));
        }
      }

      ReturnToPool();
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
      Rigidbody rb = GetComponent<Rigidbody>();
      if ( rb != null )
      {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
      }

      gameObject.SetActive(false);
      pool.Release(gameObject);
    }
  }

  public void ClearStatusEffects()
  {
    statusEffects.Clear();
  }
}
