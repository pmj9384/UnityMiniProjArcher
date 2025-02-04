using UnityEngine;

public class ReaperAttack : MonoBehaviour, IAttackBehavior
{
  public GameObject scytheProjectilePrefab; // 낫 프리팹
  public Transform projectileSpawnPoint; // 발사 위치
  public float attackCooldown = 3f; // 3초마다 공격
  private float lastAttackTime;
  private Transform player;

  private void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player")?.transform; // 🔥 오류 수정
    Debug.Log($"🔍 {gameObject.name}: ReaperAttack 초기화 완료");

    // ✅ Null 체크 추가
    if ( player == null ) Debug.LogError($"❌ {gameObject.name}: Player를 찾을 수 없음!");
    if ( projectileSpawnPoint == null ) Debug.LogError($"❌ {gameObject.name}: projectileSpawnPoint가 Inspector에서 설정되지 않음!");
    if ( scytheProjectilePrefab == null ) Debug.LogError($"❌ {gameObject.name}: scytheProjectilePrefab이 Inspector에서 설정되지 않음!");
  }

  public void Attack()
  {
    if ( Time.time - lastAttackTime < attackCooldown ) return;
    if ( player == null || projectileSpawnPoint == null || scytheProjectilePrefab == null )
    {
      Debug.LogWarning($"⚠️ {gameObject.name}: 공격 실패! 필요한 요소가 없음.");
      return;
    }

    // ✅ 올바르게 플레이어 방향 설정
    Vector3 direction = ( player.position - projectileSpawnPoint.position ).normalized;

    // ✅ 회전하는 낫 생성
    GameObject scythe = Instantiate(scytheProjectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(direction));
    ScytheProjectile projectileScript = scythe.GetComponent<ScytheProjectile>();

    if ( projectileScript != null )
    {
      projectileScript.Initialize(direction); // 플레이어 방향으로 발사
    }

    lastAttackTime = Time.time;
    Debug.Log($"⚔️ {gameObject.name}: 낫을 발사했습니다! → 방향: {direction}");
  }
}