using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class QueenSpiderAttack : MonoBehaviour, IAttackBehavior
{
  public Transform spiderSpawnPoint;
  public Transform projectileSpawnPoint;
  public GameObject projectilePrefab;
  private EnemySpawner enemySpawner;
  private bool isAttacking = false;

  private void Start()
  {
    enemySpawner = FindObjectOfType<EnemySpawner>();
  }
  public void Attack()
  {

  }

  public IEnumerator SummonSpider()
  {
    isAttacking = true;
    Debug.Log("🕷️ 거미 소환 준비!");

    yield return new WaitForSeconds(1f);

    if ( enemySpawner != null && spiderSpawnPoint != null )
    {
      for ( int i = 0; i < 3; i++ ) // 🔥 3마리 소환
      {
        enemySpawner.SpawnEnemyAtPoint(10001, spiderSpawnPoint.position);
        GameManager.Instance?.IncrementZombieCount(); // 🔥 개별 카운트 증가
      }

      Debug.Log("🕷️ 거미 3마리 소환 완료!");
    }

    yield return new WaitForSeconds(3f);
    isAttacking = false;
  }

  public IEnumerator ShootProjectile()
  {
    isAttacking = true;
    Debug.Log("🔥 콩알탄 퍼짐 발사 준비!");

    yield return new WaitForSeconds(1f);

    if ( projectilePrefab != null && projectileSpawnPoint != null )
    {
      // 🔥 3방향으로 발사 (좌 -15도, 중앙 0도, 우 +15도)
      FireProjectileAtAngle(-15f);
      yield return new WaitForSeconds(0.3f);
      FireProjectileAtAngle(0f);
      yield return new WaitForSeconds(0.3f);
      FireProjectileAtAngle(15f);

      Debug.Log("🔥 콩알탄 3발 퍼짐 발사!");
    }

    yield return new WaitForSeconds(3f);
    isAttacking = false;
  }

  private void FireProjectileAtAngle(float angleOffset)
  {
    Vector3 forwardDirection = projectileSpawnPoint.forward;
    Quaternion spreadRotation = Quaternion.Euler(0, angleOffset, 0);
    Vector3 spreadDirection = spreadRotation * forwardDirection;

    GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(spreadDirection));

    ProjectileMoveScript projectileScript = projectile.GetComponent<ProjectileMoveScript>();
    if ( projectileScript != null )
    {
      projectileScript.damage = 15f; // 🔥 콩알탄 데미지 설정 가능
    }
  }


}
