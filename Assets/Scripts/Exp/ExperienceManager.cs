using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; } // 싱글톤

    public float attractionSpeed = 5f; // 경험치 끌어당기는 속도
    private bool isAllEnemiesDead = false; // 모든 적이 죽었는지 체크

    private List<GameObject> expItems = new List<GameObject>(); // 경험치 오브젝트 리스트

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 삭제
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

    // 경험치 오브젝트 등록
    public void RegisterExpItem(GameObject expItem)
    {
        expItems.Add(expItem);
    }

    // 모든 적이 죽었을 때 호출
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

            // 일정 범위 안에 들어오면 경험치 획득
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
        isAllEnemiesDead = false;  // 적이 다시 생성되도록 isAllEnemiesDead를 false로 설정
    }


 
}
