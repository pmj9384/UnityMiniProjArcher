using System.Collections;
using UnityEngine;

public class QueenSpiderAttack : MonoBehaviour, IAttackBehavior
{
  public Transform spiderSpawnPoint; // 🕷 스파이더가 소환될 위치
  public float attackCooldown = 3f; // 공격 쿨다운
  private bool isAttacking = false;

  private Animator animator;
  private EnemySpawner enemySpawner; // ✅ EnemySpawner 사용

  private void Start()
  {
    animator = GetComponent<Animator>();
    enemySpawner = FindObjectOfType<EnemySpawner>(); // ✅ EnemySpawner 찾기

    if ( spiderSpawnPoint == null )
    {
      Debug.LogError($"{gameObject.name}: SpiderSpawnPoint가 설정되지 않음!");
    }

    if ( enemySpawner == null )
    {
      Debug.LogError($"{gameObject.name}: EnemySpawner를 찾을 수 없음!");
    }
  }

  public void Attack()
  {
    if ( isAttacking ) return; // 공격 중이면 실행 안 함
    StartCoroutine(SummonSpider());
  }

  private IEnumerator SummonSpider()
  {
    isAttacking = true;

    // 🕷 소환 애니메이션 실행
    animator?.SetTrigger("CastSpell");

    // 1초 후 소환 실행
    yield return new WaitForSeconds(1f);

    if ( enemySpawner != null && spiderSpawnPoint != null )
    {
      // ✅ EnemySpawner를 통해 ID 10001(일반 스파이더) 소환
      enemySpawner.SpawnEnemyAtPoint(10001, spiderSpawnPoint.position);
      Debug.Log($"🕷️ {gameObject.name}: EnemySpawner를 통해 스파이더 소환 완료!");
    }
    else
    {
      Debug.LogError($"{gameObject.name}: 스파이더 소환 실패! EnemySpawner 또는 spawnPoint가 null");
    }

    // 공격 쿨다운
    yield return new WaitForSeconds(attackCooldown);

    isAttacking = false;
  }
}
