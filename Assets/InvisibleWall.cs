using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    private Collider wallCollider;

    private void Awake()
    {
        wallCollider = GetComponent<Collider>();
        if (wallCollider == null)
        {
            Debug.LogError("Collider가 설정되지 않았습니다!");
        }
    }

    private void Start()
    {
        EnableWall(); // 초기에는 벽 활성화
    }

    public void EnableWall()
    {
        if (wallCollider != null)
        {
            wallCollider.isTrigger = false; // 벽 활성화 (물리적으로 막음)
        }
    }

    public void DisableWall()
    {
        if (wallCollider != null)
        {
            wallCollider.isTrigger = true; // 벽 비활성화 (통과 가능)
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어가 벽에 부딪혔습니다!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (wallCollider.isTrigger && other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 투명 벽을 통과하려고 합니다.");
        }
    }
}
