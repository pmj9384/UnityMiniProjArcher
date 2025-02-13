using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveLoadManager : MonoBehaviour
{
  public static SaveLoadManager Instance { get; private set; }

  private void Awake()
  {
    if ( Instance == null )
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);  // 씬 변경 시 유지
    }
    else
    {
      Destroy(gameObject);  // 중복 생성 방지
    }
  }

  public bool HasSaveData()
  {
    return PlayerPrefs.HasKey("SaveData"); // 저장 데이터 체크 (예시)
  }

  public void SaveGame(GameData data)
  {
    string jsonData = JsonUtility.ToJson(data);
    PlayerPrefs.SetString("SaveData", jsonData);
    PlayerPrefs.Save();
  }

  public GameData LoadGame()
  {
    if ( HasSaveData() )
    {
      string jsonData = PlayerPrefs.GetString("SaveData");
      return JsonUtility.FromJson<GameData>(jsonData);
    }
    return null;
  }
}

