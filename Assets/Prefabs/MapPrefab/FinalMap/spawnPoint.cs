using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
  public int monsterID; // ✅ 이 스폰포인트에서 소환할 몬스터 ID
  private EnemySpawner spawner; // ✅ EnemySpawner 참조

  private void Start()
  {
    spawner = FindObjectOfType<EnemySpawner>(); // ✅ EnemySpawner 찾기
    if ( spawner == null )
    {
      Debug.LogError("❌ EnemySpawner를 찾을 수 없음!");
      return;
    }

    // ✅ 설정된 몬스터 ID가 있을 경우, 자동으로 소환
    if ( monsterID > 0 )
    {
      SpawnEnemy();
    }
  }

  private void SpawnEnemy()
  {
    Enemy prefab = spawner.GetPrefabByMonsterID(monsterID); // ✅ 몬스터 프리팹 가져오기
    if ( prefab == null )
    {
      Debug.LogError($"❌ 몬스터 ID {monsterID}에 대한 프리팹을 찾을 수 없음!");
      return;
    }

    // ✅ 몬스터 소환
    Enemy enemy = Instantiate(prefab, transform.position, Quaternion.identity);
    enemy.Initialize(monsterID); // ✅ Monster ID로 초기화
    enemy.whatIsTarget = LayerMask.GetMask("Player");
    GameManager.Instance?.IncrementZombieCount();
    Debug.Log($"✅ 몬스터 ID {monsterID}가 특정 위치에서 소환됨: {enemy.name}");
  }
}
