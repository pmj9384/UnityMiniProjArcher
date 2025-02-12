using UnityEngine;

public class MushroomAttack : MonoBehaviour, IAttackBehavior
{
  public GameObject projectilePrefab; // 버섯 폭탄 프리팹
  public Transform projectileSpawnPoint; // 투사체 발사 위치
  public float attackCooldown = 3f; // 공격 쿨다운
  public float launchForce = 8f; // 초기 발사 힘
  public float arcHeight = 3f; // 포물선 높이 조절

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
    // 🔥 버섯이 플레이어 방향을 바라보도록 회전
    Vector3 directionToPlayer = ( player.position - transform.position ).normalized;
    directionToPlayer.y = 0; // Y축 고정 (땅에서 회전하도록)

    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 부드러운 회전

    // 투사체 생성
    GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
    Rigidbody rb = projectile.GetComponent<Rigidbody>();

    if ( rb != null )
    {
      // 🔥 투사체가 플레이어 방향으로 발사되도록 설정
      Vector3 targetPosition = player.position;
      Vector3 launchDirection = CalculateLaunchVelocity(projectileSpawnPoint.position, targetPosition, arcHeight);
      rb.velocity = launchDirection;
    }

    lastAttackTime = Time.time;
    Debug.Log($"🍄 {gameObject.name}: 공격! 버섯이 플레이어 방향으로 회전 후 투사체 발사");
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

    // 수직 속도 계산
    float verticalVelocity = Mathf.Sqrt(-2 * gravity * height);

    // 전체 비행 시간 계산
    float totalTime = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * ( target.y - start.y + height ) / -gravity);

    // 수평 속도 계산
    Vector3 horizontalVelocity = direction / totalTime;

    return horizontalVelocity + Vector3.up * verticalVelocity;
  }
}
