using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PausePanelManager : MonoBehaviour
{
  public Image[] skillIcons; // Pause Panel에서 스킬 아이콘을 표시할 Image 배열
  private Sprite[] skillSprites; // 슬롯머신에서 전달받은 스프라이트

  // 스킬 아이콘 업데이트
  public void UpdateSkillIcons(List<int> acquiredSkillIndices, Sprite[] allSprites)
  {
    for ( int i = 0; i < skillIcons.Length; i++ )
    {
      if ( i < acquiredSkillIndices.Count )
      {
        skillIcons[i].sprite = allSprites[acquiredSkillIndices[i]];  // 선택된 스프라이트 설정
        skillIcons[i].enabled = true;  // 아이콘 활성화
      }
      else
      {
        skillIcons[i].sprite = null;  // 빈 자리는 비활성화
        skillIcons[i].enabled = false;
      }
    }
  }
}
