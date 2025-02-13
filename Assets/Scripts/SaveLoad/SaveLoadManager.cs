using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
  private string saveFilePath;

  private void Awake()
  {
    saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
  }

  public void SaveGame(GameData data)
  {
    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(saveFilePath, json);
    Debug.Log("✅ 게임 데이터 저장 완료!");
  }

  public GameData LoadGame()
  {
    if ( File.Exists(saveFilePath) )
    {
      string json = File.ReadAllText(saveFilePath);
      GameData data = JsonUtility.FromJson<GameData>(json);
      Debug.Log("✅ 게임 데이터 불러오기 성공!");
      return data;
    }
    else
    {
      Debug.LogWarning("⚠️ 저장된 데이터가 없습니다.");
      return null;
    }
  }

  public bool HasSaveData()
  {
    return File.Exists(saveFilePath);
  }

  public void DeleteSaveData()
  {
    if ( File.Exists(saveFilePath) )
    {
      File.Delete(saveFilePath);
      Debug.Log("🗑️ 저장 데이터 삭제 완료!");
    }
  }
}
