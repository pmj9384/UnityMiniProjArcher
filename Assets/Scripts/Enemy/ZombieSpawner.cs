using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
  [SerializeField] private List<EnemyPrefabData> enemyPrefabs; // ✅ 몬스터 ID와 프리팹 매핑
  [SerializeField] private Transform[] spawnPoints; // 스폰 위치 리스트
  private GameManager gm;
  private List<Enemy> enemies = new List<Enemy>();
  private MonsterDatabase monsterDatabase; // ✅ CSV 데이터베이스 참조

  private void Start()
  {
    gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    monsterDatabase = FindObjectOfType<MonsterDatabase>(); // ✅ MonsterDatabase 가져오기
  }

  private void Update()
  {
    if ( gm.IsGameOver )
      return;

    if ( enemies.Count == 0 )
    {
      CreateEnemies();
    }
  }

  private void CreateEnemies()
  {
    foreach ( Transform spawnPoint in spawnPoints )
    {
      int monsterID = GetRandomMonsterID(); // ✅ 랜덤한 몬스터 ID 선택
      MonsterData data = monsterDatabase.GetMonsterData(monsterID); // ✅ CSV에서 데이터 로드

      if ( data == null )
      {
        Debug.LogWarning($"몬스터 ID {monsterID}에 해당하는 데이터가 없음.");
        continue;
      }

      Enemy prefab = GetPrefabByMonsterID(monsterID); // ✅ ID로 Prefab 찾기
      if ( prefab == null )
      {
        Debug.LogWarning($"몬스터 ID {monsterID}에 대한 프리팹이 설정되지 않음.");
        continue;
      }

      Enemy enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
      enemy.Initialize(monsterID); // ✅ CSV 데이터로 몬스터 초기화
      enemy.gameObject.SetActive(true);

      var agent = enemy.GetComponent<NavMeshAgent>();
      if ( agent != null )
      {
        agent.enabled = true;
        if ( !agent.Warp(spawnPoint.position) )
        {
          Destroy(enemy.gameObject);
          continue;
        }
      }
      else
      {
        Destroy(enemy.gameObject);
        continue;
      }

      enemy.whatIsTarget = LayerMask.GetMask("Player");
      enemies.Add(enemy);
      GameManager.Instance.IncrementZombieCount();
    }
  }

  private int GetRandomMonsterID()
  {
    if ( monsterDatabase == null || monsterDatabase.monsterDataDict.Count == 0 )
    {
      Debug.LogError("MonsterDatabase가 비어 있음!");
      return -1;
    }

    List<int> monsterIDs = new List<int>(monsterDatabase.monsterDataDict.Keys);
    return monsterIDs[Random.Range(0, monsterIDs.Count)];
  }

  private Enemy GetPrefabByMonsterID(int monsterID)
  {
    foreach ( var enemyData in enemyPrefabs )
    {
      if ( enemyData.monsterID == monsterID )
      {
        return enemyData.prefab;
      }
    }
    return null;
  }
}
