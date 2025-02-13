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
  public List<string> GetCurrentSkillSpriteNames()
  {
    List<string> spriteNames = new List<string>();

    for ( int i = 0; i < skillIcons.Length; i++ )
    {
      if ( skillIcons[i].enabled && skillIcons[i].sprite != null )
      {
        spriteNames.Add(skillIcons[i].sprite.name); // ✅ 스프라이트의 이름을 저장
      }
      else
      {
        spriteNames.Add(""); // ✅ 빈 슬롯도 저장 (순서 유지)
      }
    }

    return spriteNames;
  }
  public void RestoreSkillIcons(List<string> acquiredSkillSprites, Sprite[] allSprites)
  {
    if ( allSprites == null || allSprites.Length == 0 )
    {
      Debug.LogError("❌ RestoreSkillIcons: allSprites가 null이거나 비어 있음! 슬롯 아이콘 복원 실패.");
      return;
    }

    for ( int i = 0; i < skillIcons.Length; i++ )
    {
      if ( i < acquiredSkillSprites.Count && !string.IsNullOrEmpty(acquiredSkillSprites[i]) )
      {
        // ✅ 저장된 이름과 일치하는 스프라이트 찾기 (디버깅 추가)
        Sprite matchingSprite = System.Array.Find(allSprites, sprite => sprite.name == acquiredSkillSprites[i]);

        if ( matchingSprite != null )
        {
          skillIcons[i].sprite = matchingSprite;
          skillIcons[i].enabled = true;
          Debug.Log($"✅ 아이콘 복원됨: {matchingSprite.name} (슬롯 {i})");
        }
        else
        {
          Debug.LogWarning($"⚠️ 저장된 스킬 아이콘 '{acquiredSkillSprites[i]}'을 찾을 수 없음. (슬롯 {i})");
          skillIcons[i].sprite = null;
          skillIcons[i].enabled = false;
        }
      }
      else
      {
        skillIcons[i].sprite = null;
        skillIcons[i].enabled = false;
      }
    }
  }



}
