using UnityEngine;

public class ReaperAttack : MonoBehaviour, IAttackBehavior
{
  public GameObject scytheProjectilePrefab; // 회전하는 낫 프리팹
  public Transform projectileSpawnPoint; // 발사 위치
  public float attackCooldown = 3f; // 3초마다 공격
  private float lastAttackTime;

  public void Attack()
  {
    // if ( Time.time - lastAttackTime < attackCooldown ) return;

    // // 회전하는 낫 생성
    // GameObject scythe = Instantiate(scytheProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
    // ScytheProjectile projectileScript = scythe.GetComponent<ScytheProjectile>();

    // if ( projectileScript != null )
    // {
    //   projectileScript.Initialize(Vector3.forward); // 방향 설정
    // }

    // lastAttackTime = Time.time; // 공격 시간 갱신
    // Debug.Log("Reaper가 낫을 발사했습니다!");
  }
}
