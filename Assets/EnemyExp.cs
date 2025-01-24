using UnityEngine;

public class EnemyExp : MonoBehaviour
{
    public int expValue; // 경험치 값
    public bool isCollectible = false; // 초기 상태는 수집 불가

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌하고 수집 가능 상태일 때만 처리
        if (collision.CompareTag("Player") && isCollectible)
        {
            GameManager.Instance.AddScore(expValue); // 경험치 획득 처리
            Destroy(gameObject); // 경험치 오브젝트 제거
        }
    }

    public void EnableCollection()
    {
        isCollectible = true;
    }
}
