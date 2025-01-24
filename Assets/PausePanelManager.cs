using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PausePanelManager : MonoBehaviour
{
    public Image[] skillIcons; // Pause Panel에서 스킬 아이콘을 표시할 Image 배열
    private Sprite[] SkillSprites; // 슬롯머신에서 전달받은 스킬 스프라이트 리스트

    // 스프라이트 리스트 초기화
    public void Initialize(Sprite[] skillSprites)
    {
        SkillSprites = skillSprites;
    }

    // 스킬 아이콘 업데이트
    public void UpdateSkillIcons(List<int> acquiredSkillIndices)
    {
        // acquiredSkillIndices의 크기만큼 아이콘을 채워넣음
        int index = 0;
        for (int i = 0; i < skillIcons.Length; i++)
        {
            // acquiredSkillIndices에 추가할 스킬이 있다면
            if (index < acquiredSkillIndices.Count)
            {
                skillIcons[i].sprite = SkillSprites[acquiredSkillIndices[index]];  // 스프라이트 설정
                skillIcons[i].enabled = true;  // 아이콘 활성화
                index++;  // 다음 인덱스 위치로 이동
            }
            else
            {
                skillIcons[i].sprite = null;  // 빈 자리는 비활성화
                skillIcons[i].enabled = false;
            }
        }
    }




}
