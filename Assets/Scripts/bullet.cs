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

    public void Launch(Vector3 direction, IObjectPool<GameObject> objectPool)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * speed; // 탄환 발사 방향 설정
        }

        pool = objectPool; 
        Invoke(nameof(ReturnToPool), lifeTime); // 일정 시간 후 풀로 반환
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            ReturnToPool(); // 풀로 반환

            // 적의 Enemy 컴포넌트 가져오기
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 적에게 데미지 전달
                enemy.OnDamage(damage, transform.position, -transform.forward);

                // 적의 상태 효과 처리 (Enemy 내부에서 구현)
                // 필요한 효과를 Enemy 클래스에서 처리
            }
        }
        else if (other.CompareTag("Wall"))
        {
            ReturnToPool(); // 벽에 닿아도 풀로 반환
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Release(gameObject); // 오브젝트 풀에 반환
        }
        else
        {
            Destroy(gameObject); // 풀이 없으면 삭제
        }
    }
}
