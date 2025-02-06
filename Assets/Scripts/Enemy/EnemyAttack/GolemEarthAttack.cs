using System.Collections;
using UnityEngine;

public class GolemEarthAttack : MonoBehaviour, IAttackBehavior
{
  public GameObject projectilePrefab; // 돌 투사체 프리팹
  public Transform projectileSpawnPoint; // 발사 위치
  public float attackRate = 1f; // 초당 2발 (0.5초마다 발사)
  public float projectileSpeed = 10f; // 투사체 속도
  private Transform player;
  private Animator animator;
  private bool isAttacking = false;
  private Coroutine attackCoroutine;

  private void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player")?.transform;
    animator = GetComponent<Animator>();

    if ( player == null ) Debug.LogError($"{gameObject.name}: Player를 찾을 수 없음!");
    if ( projectileSpawnPoint == null ) Debug.LogError($"{gameObject.name}: projectileSpawnPoint가 설정되지 않음!");
    if ( projectilePrefab == null ) Debug.LogError($"{gameObject.name}: projectilePrefab이 설정되지 않음!");
  }

  public void Attack()
  {
    if ( isAttacking ) return; // 이미 공격 중이면 실행하지 않음
    isAttacking = true;

    // 2초마다 애니메이션 트리거 설정 (Loop)
    StartCoroutine(AttackLoop());

    // 초당 2발 발사 (0.5초 간격)
    attackCoroutine = StartCoroutine(ShootProjectiles());
  }

  private IEnumerator AttackLoop()
  {
    while ( isAttacking )
    {
      animator?.SetTrigger("Attack"); // 🔥 2초마다 애니메이션 실행
      yield return new WaitForSeconds(2f);
    }
  }

  private IEnumerator ShootProjectiles()
  {
    while ( isAttacking )
    {
      Shoot(); // 발사 실행
      yield return new WaitForSeconds(attackRate); // 0.5초마다 반복
    }
  }

  private void Shoot()
  {
    if ( player == null || projectileSpawnPoint == null || projectilePrefab == null ) return;

    // 플레이어 방향 계산
    Vector3 directionToPlayer = ( player.position - projectileSpawnPoint.position ).normalized;
    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

    // 투사체 생성
    GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
    projectile.transform.rotation = lookRotation;
    Rigidbody rb = projectile.GetComponent<Rigidbody>();

    if ( rb != null )
    {
      rb.velocity = directionToPlayer * projectileSpeed; // 플레이어 방향으로 발사
    }

  }

  public void StopAttack()
  {
    isAttacking = false;
    if ( attackCoroutine != null ) StopCoroutine(attackCoroutine);
    StopAllCoroutines(); // 모든 코루틴 정지
  }
}
