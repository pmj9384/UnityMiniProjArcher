using UnityEngine;

public class MapManager : MonoBehaviour
{
  public GameObject[] mapPrefabs;
  private GameObject currentMap;
  private int mapCount = 0;

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
  }
}
