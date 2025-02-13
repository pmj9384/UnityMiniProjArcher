using UnityEngine;
using TMPro;

public class MapManager : MonoBehaviour
{
  public GameObject[] mapPrefabs;
  private GameObject currentMap;
  private int mapCount = 1; // ✅ 기본값을 1로 설정 (첫 번째 맵이 로드되도록 함)

  public TextMeshProUGUI stageText;
  public int totalStages = 20;
  public UiManager uiManager;

  void Start()
  {
    // GameManager에서 스테이지 로드를 관리하므로 여기서는 실행 X
  }

  public void LoadMap()
  {
    mapCount++; // ✅ 다음 스테이지로 이동
    if ( mapCount > totalStages )
    {
      Debug.Log("✅ 모든 스테이지 완료!");
      uiManager.ShowGameClearPanel(true); // 게임 클리어 UI 표시
      return;
    }

    LoadSpecificStage(mapCount); // ✅ `mapCount`를 기반으로 스테이지 로드

    if ( GameManager.Instance != null )
    {
      GameManager.Instance.SaveGame();

    }
  }

  public void LoadSpecificStage(int stage)
  {
    if ( mapPrefabs == null || mapPrefabs.Length != 20 )
    {
      Debug.LogError("❌ `mapPrefabs` 배열이 올바르게 설정되지 않았습니다! 20개의 맵을 추가하세요.");
      return;
    }

    if ( stage < 1 || stage > 20 )
    {
      Debug.LogError($"❌ 잘못된 스테이지 번호 ({stage})! 1~20 사이여야 합니다.");
      stage = 1; // ✅ 잘못된 값이면 1로 초기화
    }

    if ( mapPrefabs[stage - 1] == null )
    {
      Debug.LogError($"❌ `mapPrefabs[{stage - 1}]`가 `null`입니다! 올바른 맵을 설정하세요.");
      return;
    }

    if ( currentMap != null )
    {
      Destroy(currentMap);
    }

    currentMap = Instantiate(mapPrefabs[stage - 1]);
    currentMap.SetActive(true);

    ExperienceManager.Instance.ResetExperience();
    Player.Instance.SetInitialPosition();

    mapCount = stage; // ✅ 현재 스테이지 값 업데이트
    UpdateStageText();
  }

  public void StartNewGame()
  {
    mapCount = 1; // ✅ 새 게임을 시작할 때 첫 번째 스테이지로 초기화
    LoadSpecificStage(mapCount);
  }

  private void UpdateStageText()
  {
    if ( stageText != null )
    {
      stageText.text = $"{mapCount} / {totalStages}"; // ✅ 1부터 시작하는 값으로 표시
    }
    else
    {
      Debug.LogError("Stage Text is not assigned in MapManager!");
    }
  }

  public int GetCurrentStage()
  {
    return mapCount;
  }
}
