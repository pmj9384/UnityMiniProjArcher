using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemies;  // 현재 맵의 적들
    public int experienceReward = 100;  // 적을 모두 처치한 후 얻는 경험치
    private int deadEnemies = 0;  // 죽은 적의 수

    void Start()
    {
        // 적 상태 초기화
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); // 맵에 있는 모든 적을 찾아 배열에 저장
    }

    void Update()
    {
        // 모든 적이 죽었는지 체크
        CheckEnemiesStatus();
    }

    void CheckEnemiesStatus()
    {
        deadEnemies = 0;  // 죽은 적 수 초기화

        // 적들의 상태를 확인하여 죽은 적 수를 셈
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)  // 적이 사라졌다면 (죽었다면)
            {
                deadEnemies++;
            }
        }

        // 모든 적이 죽었다면 경험치를 지급
        if (deadEnemies == enemies.Length)
        {
            GiveExperience();
        }
    }

    void GiveExperience()
    {
        // 경험치를 지급하는 로직
        Debug.Log("All enemies defeated! Gaining " + experienceReward + " experience.");
        // 여기에 실제 경험치 시스템을 연결하거나 레벨업 시스템을 추가할 수 있음
    }
}
