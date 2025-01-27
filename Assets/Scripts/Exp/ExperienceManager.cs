using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
  public static ExperienceManager Instance { get; private set; }

  public float attractionSpeed = 5f;
  private bool isAllEnemiesDead = false;

  private List<GameObject> expItems = new List<GameObject>();

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
      return;
    }
  }

  void Update()
  {
    if (isAllEnemiesDead)
    {
      AttractExperienceItems();
    }
  }

  public void RegisterExpItem(GameObject expItem)
  {
    expItems.Add(expItem);
  }

  public void OnAllEnemiesDead()
  {
    isAllEnemiesDead = true;
  }

  void AttractExperienceItems()
  {
    for (int i = expItems.Count - 1; i >= 0; i--)
    {
      GameObject expItem = expItems[i];
      if (expItem == null) continue;

      Vector3 directionToPlayer = (Player.Instance.transform.position - expItem.transform.position).normalized;
      expItem.transform.position += directionToPlayer * attractionSpeed * Time.deltaTime;

      if (Vector3.Distance(expItem.transform.position, Player.Instance.transform.position) < 0.5f)
      {
        Player.Instance.AddExperience(expItem.GetComponent<EnemyExp>().expValue);
        Destroy(expItem);
        expItems.RemoveAt(i);
      }
    }
  }

  public void ResetExperience()
  {
    isAllEnemiesDead = false;
  }
}
