using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] mapPrefabs; // 맵 Prefab 배열
    private GameObject currentMap; // 현재 활성화된 맵
    private int mapCount = 0; // 맵 이동 횟수

    void Start()
    {
        LoadMap(); // 첫 번째 맵 로드
    }

    public void LoadMap()
    {
        mapCount++;

        // 이전 맵 제거
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        // 랜덤 맵 선택 (10번째는 Map1)
        int randomIndex = (mapCount  == 1 || mapCount % 10 == 0) ? 0 : Random.Range(1, mapPrefabs.Length);

        // 맵 생성
        currentMap = Instantiate(mapPrefabs[randomIndex]);
        currentMap.SetActive(true);
        
        // 경험치 초기화
        ExperienceManager.Instance.ResetExperience();

        // 플레이어를 저장된 초기 위치로 이동
        Player.Instance.SetInitialPosition(); // Player 객체는 싱글톤으로 접근
    }
}
