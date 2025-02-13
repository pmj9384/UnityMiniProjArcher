using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
  public int playerHP;
  public int playerGold;
  public int currentStage;
  public List<int> acquiredSkills;  // 플레이어가 획득한 스킬 목록
}

