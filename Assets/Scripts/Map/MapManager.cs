using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가

public class MapManager : MonoBehaviour
{
  public GameObject[] mapPrefabs;
  private GameObject currentMap;
  private int mapCount = 0;

  public TextMeshProUGUI stageText; // TextMeshPro 텍스트 추가
  public int totalStages = 20;     // 전체 스테이지 수

  void Start()
  {
    LoadMap();
  }

  public void LoadMap()
  {
    mapCount++;

    if ( currentMap != null )
    {
      Destroy(currentMap);
    }

    int randomIndex;

    if ( mapCount == 1 )
    {
      randomIndex = 0;
    }
    else if ( mapCount == 5 )
    {
      randomIndex = 5;
    }
    else if ( mapCount % 10 == 0 )
    {
      randomIndex = 0;
    }
    else
    {
      do
      {
        randomIndex = Random.Range(1, mapPrefabs.Length);
      }
      while ( randomIndex == 5 );
    }

    currentMap = Instantiate(mapPrefabs[randomIndex]);
    currentMap.SetActive(true);

    ExperienceManager.Instance.ResetExperience();
    Player.Instance.SetInitialPosition();

    UpdateStageText(); // 스테이지 텍스트 업데이트
  }

  private void UpdateStageText()
  {
    if ( stageText != null )
    {
      stageText.text = mapCount + "/" + totalStages; // "1/20" 형식으로 텍스트 업데이트
    }
    else
    {
      Debug.LogError("Stage Text is not assigned in MapManager!");
    }
  }
}
