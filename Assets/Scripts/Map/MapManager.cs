using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가

public class MapManager : MonoBehaviour
{
  public GameObject[] mapPrefabs;
  private GameObject currentMap;
  private int mapCount = 0;

  public TextMeshProUGUI stageText; // TextMeshPro 텍스트 추가
  public int totalStages = 20; // 전체 스테이지 수
  public UiManager uiManager;

  void Start()
  {
    LoadMap();
  }

  public void LoadMap()
  {
    mapCount++;
    if ( mapCount > totalStages )
    {
      uiManager.ShowGameClearPanel(true); // ✅ UIManager로 신호
      return; // 더 이상 맵 로딩하지 않음
    }

    if ( currentMap != null )
    {
      Destroy(currentMap);
    }


    int mapIndex = ( mapCount - 1 ) % mapPrefabs.Length; // 배열 순환

    currentMap = Instantiate(mapPrefabs[mapIndex]);
    currentMap.SetActive(true);

    ExperienceManager.Instance.ResetExperience();
    Player.Instance.SetInitialPosition();

    UpdateStageText();
  }

  private void UpdateStageText()
  {
    if ( stageText != null )
    {
      stageText.text = mapCount + "/" + totalStages;
    }
    else
    {
      Debug.LogError("Stage Text is not assigned in MapManager!");
    }
  }
}
