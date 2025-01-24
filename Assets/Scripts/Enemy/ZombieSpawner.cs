using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy[] prefabs;
    [SerializeField] private ZombieData[] datas;
    [SerializeField] private Transform[] spawnPoints; // 여러 스폰 포인트
    [SerializeField] private LayerMask playerLayer;

    private GameManager gm;
    private int wave = 1;
    private List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (gm.IsGameOver)
            return;

        // 모든 스폰 포인트에 적을 소환
        if (enemies.Count == 0)
        {
            CreateEnemies();
        }
    }

    private void CreateEnemies()
    {
        // 각 스폰 포인트에 맞춰서 적을 소환
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int index = Random.Range(0, prefabs.Length);  // 랜덤한 적 타입 선택
            var spawnPoint = spawnPoints[i];  // 해당 인덱스의 스폰 포인트 선택
            var enemy = Instantiate(prefabs[index], spawnPoint.position, spawnPoint.rotation);
            enemy.gameObject.SetActive(true);

            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
                if (!agent.Warp(spawnPoint.position))
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

            var data = datas[index];
            enemy.maxHp = data.hp;
            enemy.damage = data.damage;
            agent.speed = data.speed;

            enemy.whatIsTarget = LayerMask.GetMask("Player");
            enemies.Add(enemy);

            GameManager.Instance.IncrementZombieCount(); // 좀비 카운트 증가
        }
    }
}
