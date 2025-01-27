using UnityEngine;

public class EnemyManager : MonoBehaviour
{
  public GameObject[] enemies;
  public int experienceReward = 100;
  private int deadEnemies = 0;

  void Start()
  {
    enemies = GameObject.FindGameObjectsWithTag("Enemy");
  }

  void Update()
  {
    CheckEnemiesStatus();
  }

  void CheckEnemiesStatus()
  {
    deadEnemies = 0;

    foreach (GameObject enemy in enemies)
    {
      if (enemy == null)
      {
        deadEnemies++;
      }
    }

    if (deadEnemies == enemies.Length)
    {
      GiveExperience();
    }
  }

  void GiveExperience()
  {
    Debug.Log("All enemies defeated! Gaining " + experienceReward + " experience.");
  }
}
