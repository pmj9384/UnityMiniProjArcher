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

   
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        
        int randomIndex = (mapCount  == 1 || mapCount % 10 == 0) ? 0 : Random.Range(1, mapPrefabs.Length);

      
        currentMap = Instantiate(mapPrefabs[randomIndex]);
        currentMap.SetActive(true);
        
        
        ExperienceManager.Instance.ResetExperience();

     
        Player.Instance.SetInitialPosition(); 
    }
}
