using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  [SerializeField] private List<EnemyPrefabData> enemyPrefabs; // ✅ 몬스터 ID와 프리팹 매핑
  private MonsterDatabase monsterDatabase; // ✅ CSV 데이터베이스 참조

  private void Start()
  {
    monsterDatabase = FindObjectOfType<MonsterDatabase>(); // ✅ MonsterDatabase 가져오기
    if ( monsterDatabase == null )
    {
      Debug.LogError("❌ MonsterDatabase를 찾을 수 없음!");
    }
  }

  public void SpawnEnemyAtPoint(int monsterID, Vector3 spawnPosition)
  {
    if ( monsterDatabase == null )
    {
      Debug.LogError("MonsterDatabase가 설정되지 않음!");
      return;
    }

    MonsterData data = monsterDatabase.GetMonsterData(monsterID);
    if ( data == null )
    {
      Debug.LogWarning($"몬스터 ID {monsterID}에 해당하는 데이터가 없음.");
      return;
    }

    Enemy prefab = GetPrefabByMonsterID(monsterID);
    if ( prefab == null )
    {
      Debug.LogWarning($"몬스터 ID {monsterID}에 대한 프리팹이 설정되지 않음.");
      return;
    }

    // 적 생성
    Enemy enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
    enemy.Initialize(monsterID); // ✅ Monster ID로 초기화
    enemy.whatIsTarget = LayerMask.GetMask("Player");

    Debug.Log($"✅ 몬스터 ID {monsterID} 소환 완료: {enemy.name}");
  }

  // ✅ 특정 몬스터 ID에 해당하는 프리팹 반환
  public Enemy GetPrefabByMonsterID(int monsterID)
  {
    foreach ( var enemyData in enemyPrefabs )
    {
      if ( enemyData.monsterID == monsterID )
      {
        return enemyData.prefab;
      }
    }
    Debug.LogWarning($"⚠️ 몬스터 ID {monsterID}에 대한 프리팹이 설정되지 않음.");
    return null;
  }
  
}
