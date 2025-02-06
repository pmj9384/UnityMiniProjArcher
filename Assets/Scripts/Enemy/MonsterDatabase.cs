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

    Debug.Log($"📄 CSV 데이터 줄 수: {lines.Length}");

    for ( int i = 1; i < lines.Length; i++ ) // ✅ i = 1로 변경 (헤더 제외)
    {
      string line = lines[i].Trim(); // ✅ 공백 제거
      if ( string.IsNullOrEmpty(line) ) continue; // ✅ 빈 줄 방지

      string[] values = line.Split(',');

      if ( values.Length < 9 )
      {
        Debug.LogWarning($"⚠️ 잘못된 데이터 줄 ({i}): {lines[i]}");
        continue;
      }

      // ✅ 몬스터 데이터 생성
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

      // ✅ 중복 방지: Dictionary에 같은 ID가 있으면 추가 안 함
      if ( !monsterDataDict.ContainsKey(monster.id) )
      {
        monsterDataDict.Add(monster.id, monster);
        Debug.Log($"✅ 몬스터 추가: {monster.id} - {monster.name}");
      }
      else
      {
        Debug.LogWarning($"⚠️ 중복된 몬스터 ID 발견: {monster.id} → 스킵됨!");
      }
    }

    Debug.Log($"🎉 몬스터 데이터 로드 완료: 총 {monsterDataDict.Count}개");
  }

  public MonsterData GetMonsterData(int id)
  {
    if ( monsterDataDict.ContainsKey(id) )
      return monsterDataDict[id];

    Debug.LogWarning($"❌ ID {id}에 해당하는 몬스터가 없습니다.");
    return null;
  }
}
