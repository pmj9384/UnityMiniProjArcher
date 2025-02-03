using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData
{
  public int id;
  public string name;
  public int hp;
  public float speed;
  public int moveType;
  public string moveBeacon;
  public int attack;
  public string asset;
  public int dropExp;
}

public class MonsterDatabase : MonoBehaviour
{
  public TextAsset csvFile; // CSV 파일을 Unity Inspector에서 연결
  public Dictionary<int, MonsterData> monsterDataDict = new Dictionary<int, MonsterData>();

  void Awake()
  {
    LoadMonsterData();
  }

  void LoadMonsterData()
  {
    if ( csvFile == null )
    {
      Debug.LogError("CSV 파일이 연결되지 않았습니다!");
      return;
    }

    // ✅ CSV 데이터를 줄 단위로 읽기
    string[] lines = csvFile.text.Split('\n');

    Debug.Log($"CSV 데이터 줄 수: {lines.Length}");

    for ( int i = 2; i < lines.Length; i++ ) // 데이터는 2번째 줄부터 시작
    {
      string[] values = lines[i].Split(','); // ✅ 쉼표(,) 대신 탭(\t)으로 분리

      if ( values.Length < 9 )
      {
        Debug.LogWarning($"잘못된 데이터 줄 ({i}): {lines[i]}");
        continue;
      }

      MonsterData monster = new MonsterData
      {
        id = int.Parse(values[0].Trim()),
        name = values[1].Trim(),
        hp = int.Parse(values[2].Trim()),
        speed = float.Parse(values[3].Trim()),
        moveType = int.Parse(values[4].Trim()),
        moveBeacon = values[5].Trim(),
        attack = int.Parse(values[6].Trim()),
        asset = values[7].Trim(),
        dropExp = int.Parse(values[8].Trim())
      };

      monsterDataDict.Add(monster.id, monster);
    }

    Debug.Log($"몬스터 데이터 {monsterDataDict.Count}개 로드 완료!");
  }


  public MonsterData GetMonsterData(int id)
  {
    if ( monsterDataDict.ContainsKey(id) )
      return monsterDataDict[id];

    Debug.LogWarning($"ID {id}에 해당하는 몬스터가 없습니다.");
    return null;
  }
}
