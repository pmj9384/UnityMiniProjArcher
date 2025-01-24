using UnityEngine;

public class PortalPoint : MonoBehaviour
{
    private MapManager mapManager; // MapManager 연결

    private void Start()
    {
        // 자동으로 MapManager를 찾아서 연결
        mapManager = FindObjectOfType<MapManager>();
        if (mapManager == null)
        {
            Debug.LogError("MapManager를 찾을 수 없습니다. 씬에 MapManager가 존재하는지 확인하세요.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 포탈 포인트에 도달했을 때
        if (other.CompareTag("Player"))
        {
            Debug.Log("PortalPoint에 도달! 맵 이동 실행.");
            
            // 플레이어를 첫 번째 시작 위치로 텔레포트
            other.GetComponent<Player>().SetInitialPosition(); 

            // 맵 이동
            if (mapManager != null)
            {
                mapManager.LoadMap(); // MapManager에서 LoadMap 호출하여 맵 이동
            }
        }
    }
}
