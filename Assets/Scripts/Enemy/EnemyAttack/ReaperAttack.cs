using UnityEngine;

public class ReaperAttack : MonoBehaviour, IAttackBehavior
{
  public GameObject scytheProjectilePrefab; // 낫 프리팹
  public Transform projectileSpawnPoint; // 발사 위치
  public float attackCooldown = 3f; // 3초마다 공격
  private float lastAttackTime;
  private Transform player;
  private Animator animator;
 
  private void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player")?.transform;
    animator = GetComponent<Animator>();

    Debug.Log($"🔍 {gameObject.name}: ReaperAttack 초기화 완료");

    if ( player == null ) Debug.LogError($"❌ {gameObject.name}: Player를 찾을 수 없음!");
    if ( projectileSpawnPoint == null ) Debug.LogError($"❌ {gameObject.name}: projectileSpawnPoint가 설정되지 않음!");
    if ( scytheProjectilePrefab == null ) Debug.LogError($"❌ {gameObject.name}: scytheProjectilePrefab이 설정되지 않음!");
  }

  public void Attack()
  {
    if ( Time.time - lastAttackTime < attackCooldown ) return;
    if ( player == null || projectileSpawnPoint == null || scytheProjectilePrefab == null )
    {
      Debug.LogWarning($"⚠️ {gameObject.name}: 공격 실패! 필요한 요소가 없음.");
      return;
    }

    animator?.SetTrigger("Attack");

    // 🔥 공격 직전에 한 번만 플레이어 방향으로 회전
    RotateTowardsPlayer();

    // ✅ 올바르게 플레이어 방향 설정
    Vector3 direction = ( player.position - projectileSpawnPoint.position ).normalized;

    // ✅ 회전하는 낫 생성
    GameObject scythe = Instantiate(scytheProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
    Rigidbody rb = scythe.GetComponent<Rigidbody>();

    if ( rb != null )
    {
      // 🔥 투사체도 플레이어 방향으로 회전
      scythe.transform.rotation = Quaternion.LookRotation(direction);
      rb.velocity = direction * 10f; // 원하는 속도로 조정
    }

    lastAttackTime = Time.time;
    Debug.Log($"⚔️ {gameObject.name}: 낫을 발사했습니다! → 방향: {direction}");
  }

  // 🔥 공격 직전에 한 번만 플레이어 방향으로 회전
  private void RotateTowardsPlayer()
  {
    if ( player == null ) return;

    Vector3 directionToPlayer = ( player.position - transform.position ).normalized;
    directionToPlayer.y = 0; // Y축 회전만 반영
    transform.rotation = Quaternion.LookRotation(directionToPlayer);
  }
}
