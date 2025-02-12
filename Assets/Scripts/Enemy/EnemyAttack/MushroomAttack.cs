using UnityEngine;

public class MushroomAttack : MonoBehaviour, IAttackBehavior
{
  public GameObject projectilePrefab; // 버섯 폭탄 프리팹
  public Transform projectileSpawnPoint; // 투사체 발사 위치
  public float attackCooldown = 3f; // 공격 쿨다운
  public float launchForce = 8f; // 초기 발사 힘
  public float arcHeight = 5f; // 포물선 높이 조절

  private float lastAttackTime;
  private Transform player;
  private Animator animator;

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
    if ( Time.time - lastAttackTime < attackCooldown ) return;
    if ( player == null || projectileSpawnPoint == null || projectilePrefab == null ) return;

    animator?.SetTrigger("Attack");

    // 🔥 플레이어가 있는 방향을 향하도록 회전
    Vector3 directionToPlayer = ( player.position - transform.position ).normalized;
    directionToPlayer.y = 0; // Y축 고정 (바닥에서 회전)

    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

    // 🔥 투사체 생성
    GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
    Rigidbody rb = projectile.GetComponent<Rigidbody>();

    if ( rb != null )
    {
      // 🔥 타겟 위치를 플레이어 머리 높이까지 보정
      Vector3 targetPosition = player.position + Vector3.up * 1.5f;
      Vector3 launchDirection = CalculateLaunchVelocity(projectileSpawnPoint.position, targetPosition, arcHeight);
      rb.velocity = launchDirection;
    }

    lastAttackTime = Time.time;
    Debug.Log($"🍄 {gameObject.name}: 공격! 버섯이 플레이어 방향으로 투사체 발사");
  }

  private void Update()
  {
    if ( player != null )
    {
      Vector3 directionToPlayer = ( player.position - transform.position ).normalized;
      directionToPlayer.y = 0; // Y축 고정하여 회전

      Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
      transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f); // 부드러운 회전
    }

  }


  // 포물선 궤적 계산 함수
  private Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 target, float height)
  {
    Vector3 direction = target - start;
    direction.y = 0; // 수평 방향 유지
    float distance = direction.magnitude;
    float gravity = Physics.gravity.y;

    // 🔥 플레이어 머리 높이까지 도달하도록 y값 보정
    target.y += 1.5f; // 플레이어 머리 높이 추가 (필요 시 조정)

    // 수직 속도 계산
    float verticalVelocity = Mathf.Sqrt(-2 * gravity * height);

    // 🔥 전체 비행 시간 계산 (조정된 target.y 사용)
    float timeToApex = Mathf.Sqrt(-2 * height / gravity);
    float timeFromApex = Mathf.Sqrt(2 * ( target.y - start.y + height ) / -gravity);
    float totalTime = timeToApex + timeFromApex;

    // 수평 속도 계산
    Vector3 horizontalVelocity = direction / totalTime;

    return horizontalVelocity + Vector3.up * verticalVelocity;
  }

}
