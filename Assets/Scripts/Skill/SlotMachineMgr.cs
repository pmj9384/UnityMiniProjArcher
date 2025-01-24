using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineMgr : MonoBehaviour
{
    public GameObject[] SlotSkillObject;
    public Button[] Slot;

    public Sprite[] SkillSprite;

    [System.Serializable]
    public class DisplayItemSlot
    {
        public List<Image> SlotSprite = new List<Image>();
    }
    public DisplayItemSlot[] DisplayItemSlots;

    public Image DisplayResultImage;

    public List<int> StartList = new List<int>();
    public List<int> ResultIndexList = new List<int>();
    public List<int> SelectedSkills = new List<int>(); 
    int ItemCnt = 3;
    int[] answer = { 2, 3, 1 };

    // 추가: 최종 인덱스를 저장할 배열
    private int[] finalSkillIndices;

    // Start is called before the first frame update
    void Start()
    {
        // StartList에 값 추가
        for (int i = 0; i < ItemCnt * Slot.Length; i++)
        {
            StartList.Add(i);
        }

        // 슬롯 회전 및 결과 처리
        for (int i = 0; i < Slot.Length; i++)
        {
            for (int j = 0; j < ItemCnt; j++)
            {
                Slot[i].interactable = false;

                // 랜덤으로 인덱스를 선택
                int randomIndex = Random.Range(0, StartList.Count);

                // 결과 리스트에 값 추가
                if (i == 0 && j == 1 || i == 1 && j == 0 || i == 2 && j == 2)
                {
                    ResultIndexList.Add(StartList[randomIndex]);
                }

                // 스프라이트 설정
                DisplayItemSlots[i].SlotSprite[j].sprite = SkillSprite[StartList[randomIndex]];

                // 추가: 0번째 인덱스는 결과를 2배 설정
                if (j == 0)
                {
                    DisplayItemSlots[i].SlotSprite[ItemCnt].sprite = SkillSprite[StartList[randomIndex]];
                }

                // 사용한 인덱스를 제거
                StartList.RemoveAt(randomIndex);
            }
        }

        // 최종 인덱스를 저장
        finalSkillIndices = new int[Slot.Length];
        for (int i = 0; i < Slot.Length; i++)
        {
            finalSkillIndices[i] = ResultIndexList[i];
        }

        // 슬롯 회전 후 코루틴 시작
        for (int i = 0; i < Slot.Length; i++)
        {
            StartCoroutine(StartSlot(i));
        }
    }

    IEnumerator StartSlot(int SlotIndex)
    {
        // 슬롯 회전 애니메이션 처리
        for (int i = 0; i < (ItemCnt * (6 + SlotIndex * 4) + answer[SlotIndex]) * 2; i++)
        {
            SlotSkillObject[SlotIndex].transform.localPosition -= new Vector3(0, 50f, 0);
            if (SlotSkillObject[SlotIndex].transform.localPosition.y < 50f)
            {
                SlotSkillObject[SlotIndex].transform.localPosition += new Vector3(0, 300f, 0);
            }
            yield return new WaitForSeconds(0.02f);
        }

        // 슬롯 회전이 끝난 후, 버튼을 활성화
        for (int i = 0; i < ItemCnt; i++)
        {
            Slot[i].interactable = true;
        }
    }
public void ClickBtn(int index)
{
    // 버튼 클릭 시 해당 인덱스를 통해 결과 스프라이트 설정
    DisplayResultImage.sprite = SkillSprite[finalSkillIndices[index]];
    
    // PausePanelManager에 선택 결과 전달
    PausePanelManager pausePanelManager = FindObjectOfType<PausePanelManager>();
    if (pausePanelManager != null)
    {
        pausePanelManager.Initialize(SkillSprite);
        
        // 기존 스킬들을 유지하면서 새로운 스킬만 추가
        SelectedSkills.Add(finalSkillIndices[index]); // 새로 선택된 스킬 추가
        pausePanelManager.UpdateSkillIcons(SelectedSkills);  // 업데이트된 SelectedSkills를 전달
    }
    else
    {
        Debug.LogError("PausePanelManager를 찾을 수 없습니다.");
    }

    // 스킬 적용
    ApplySkillEffect(finalSkillIndices[index]);

    // 슬롯 선택 완료 처리
    OnSlotSelectionComplete(index);
}



    public void OnSlotSelectionComplete(int selectedIndex)
    {
        // 슬롯 머신에서 선택된 결과를 처리
        DisplayResultImage.sprite = SkillSprite[finalSkillIndices[selectedIndex]];

        // // 선택된 스킬을 Pause Panel에 전달
        // PausePanelManager pausePanelManager = FindObjectOfType<PausePanelManager>();
        // if (pausePanelManager != null)
        // {
        //     pausePanelManager.Initialize(SkillSprite); 
        //     // **선택된 하나의 스킬만 전달**
        //     pausePanelManager.UpdateSkillIcons(new List<int> { finalSkillIndices[selectedIndex] }); // 선택된 스킬 아이콘 업데이트
        // }

        // GameManager에 선택된 스킬 전달
        GameManager.Instance.EndSlotMachine();

        // 필요한 경우 선택된 스킬 데이터를 GameManager에 전달
        // 예: GameManager.Instance.ApplySelectedSkill(selectedSkillData);
    }


    // 스킬 적용 함수
    private void ApplySkillEffect(int selectedSkillIndex)
    {
        string selectedSkillName = SkillSprite[selectedSkillIndex].name;

        // PlayerSkillController 찾기
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            PlayerSkillController playerSkillController = playerObject.GetComponent<PlayerSkillController>();

            if (playerSkillController != null)
            {
                Debug.Log($"선택된 스킬: {selectedSkillName} - 효과 적용 중...");

                // 스프라이트 이름에 따라 스킬 효과 적용
                switch (selectedSkillName)
                {
                    case "DiagonalArrow":
                        playerSkillController.ApplyDiagonalArrow();  // 사선 화살 효과
                        break;
                    case "FireArrow":
                        playerSkillController.ApplyFireArrow();  // 화염 화살 효과
                        break;
                    case "IceArrow":
                        playerSkillController.ApplyFrostArrow();  // 얼음 화살 효과
                        break;
                    case "ATK_UP":
                        playerSkillController.IncreaseAttackPower();  // 공격력 증가
                        break;
                    case "SpeedUp":
                        playerSkillController.IncreaseMovementSpeed();  // 이동속도 증가
                        break;
                    case "Heal":
                        playerSkillController.RecoverHealth();  // 체력 회복
                        break;
                    case "HpUp":
                        playerSkillController.IncreaseMaxHealth();  // 최대 체력 증가
                        break;
                    case "ShotSpeedUp":
                        playerSkillController.IncreaseAttackSpeed();  // 공격속도 증가
                        break;
                    default:
                        Debug.LogError("알 수 없는 스프라이트입니다.");
                        break;
                }
            }
            else
            {
                Debug.LogError("PlayerSkillController를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다.");
        }
    }
}
