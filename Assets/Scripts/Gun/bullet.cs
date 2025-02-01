using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;
public class Bullet : MonoBehaviour
{
  public float speed = 50f;        // 탄환 속도
  public float damage;            // 탄환 데미지
  public float lifeTime = 3f;     // 탄환 수명

  private IObjectPool<GameObject> pool;  // 오브젝트 풀

  // 추가된 상태 효과
  private float fireEffectDuration = 0f;
  private float fireEffectDamage = 0f;
  private float frostEffectDuration = 0f;
  private float frostEffectSlowAmount = 0f;

  public void Launch(Vector3 direction, IObjectPool<GameObject> objectPool)
  {
    Rigidbody rb = GetComponent<Rigidbody>();
    if ( rb != null )
    {
      rb.velocity = direction * speed; // 탄환 발사 방향 설정
    }

    pool = objectPool;
    Invoke(nameof(ReturnToPool), lifeTime); // 일정 시간 후 풀로 반환
  }

  public void ApplyFireEffect(float duration, float damagePerSecond)
  {
    fireEffectDuration = duration;
    fireEffectDamage = damagePerSecond;
  }

  public void ApplyFrostEffect(float duration, float slowAmount)
  {
    frostEffectDuration = duration;
    frostEffectSlowAmount = slowAmount;
  }

  private void OnTriggerEnter(Collider other)
  {
    if ( other.CompareTag("Enemy") )
    {
      ReturnToPool(); // 풀로 반환

      // 적의 Enemy 컴포넌트 가져오기
      Enemy enemy = other.GetComponent<Enemy>();
      if ( enemy != null )
      {
        // 기본 데미지 전달
        enemy.OnDamage(damage, transform.position, -transform.forward);

        // 불화살 효과 적용
        if ( fireEffectDuration > 0 )
        {
          enemy.ApplyFireEffect(fireEffectDuration, fireEffectDamage);
        }

        // 얼음화살 효과 적용
        if ( frostEffectDuration > 0 )
        {
          enemy.ApplyFrostEffect(frostEffectDuration, frostEffectSlowAmount);
        }
      }
    }
    else if ( other.CompareTag("Wall") )
    {
      ReturnToPool(); // 벽에 닿아도 풀로 반환
    }
  }

  private void ReturnToPool()
  {
    if ( pool != null )
    {
      gameObject.SetActive(false); // 풀에 반환되면 비활성화
      pool.Release(gameObject);
    }
    else
    {
      Debug.LogWarning($"Bullet {gameObject.name} tried to return to a null pool!");
    }
  }
}

