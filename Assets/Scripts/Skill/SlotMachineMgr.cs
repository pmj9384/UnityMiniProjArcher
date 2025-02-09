using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가

public class SlotMachineMgr : MonoBehaviour
{
  public GameObject[] SlotSkillObject; // 슬롯 오브젝트 배열
  public Button[] Slot;               // 슬롯 버튼 배열
  public Sprite[] SkillSprite;        // 스킬 스프라이트 배열

  public string[] SkillNames;         // 스킬 이름 배열
  public string[] SkillDescriptions;  // 스킬 설명 배열

  [System.Serializable]
  public class DisplayItemSlot
  {
    public List<Image> SlotSprite = new List<Image>(); // 슬롯 이미지 배열
    public TextMeshProUGUI SkillNameText;       // 슬롯 위에 표시될 스킬 이름 텍스트
    public TextMeshProUGUI SkillDescriptionText; // 슬롯 아래에 표시될 스킬 설명 텍스트
  }
  public DisplayItemSlot[] DisplayItemSlots; // 슬롯 UI 정보 배열

  public Image DisplayResultImage; // 선택된 스킬 결과 이미지
  public List<int> StartList = new List<int>();
  public List<int> ResultIndexList = new List<int>();
  public List<int> SelectedSkills = new List<int>();

  int ItemCnt = 3; // 슬롯 이미지 개수

  private void OnEnable()
  {
    // ✅ 기존에 선택한 스킬을 유지하면서 초기화
    ResultIndexList.Clear();
    StartList.Clear();

    for ( int i = 0; i < ItemCnt * Slot.Length; i++ )
    {
      StartList.Add(i);
    }

    for ( int i = 0; i < Slot.Length; i++ )
    {
      Slot[i].interactable = false;

      int selectedIndex;
      do
      {
        int randomIndex = Random.Range(0, StartList.Count);
        selectedIndex = StartList[randomIndex];

      } while ( SelectedSkills.Contains(selectedIndex) ); // ✅ 이미 선택된 스킬은 다시 등장하지 않음

      ResultIndexList.Add(selectedIndex);

      int correctPos = Mathf.Clamp(ItemCnt / 2, 0, ItemCnt - 2);
      DisplayItemSlots[i].SlotSprite[correctPos].sprite = SkillSprite[selectedIndex];

      for ( int j = 0; j < ItemCnt; j++ )
      {
        if ( j == correctPos ) continue;

        int randomSkillIndex;
        do
        {
          randomSkillIndex = Random.Range(0, SkillSprite.Length);
        } while ( SelectedSkills.Contains(randomSkillIndex) ); // ✅ 슬롯 연출에서도 다시 등장하지 않음

        DisplayItemSlots[i].SlotSprite[j].sprite = SkillSprite[randomSkillIndex];
      }

      StartList.Remove(selectedIndex);
    }

    for ( int i = 0; i < Slot.Length; i++ )
    {
      StartCoroutine(StartSlot(i));
    }
  }

  IEnumerator StartSlot(int SlotIndex)
  {
    int resultIndex = ResultIndexList[SlotIndex];
    int totalSpins = 5 * ItemCnt;
    int targetStopIndex = ItemCnt / 2;

    for ( int i = 0; i < totalSpins; i++ )
    {
      SlotSkillObject[SlotIndex].transform.localPosition -= new Vector3(0, 50f, 0);
      if ( SlotSkillObject[SlotIndex].transform.localPosition.y < 50f )
      {
        SlotSkillObject[SlotIndex].transform.localPosition += new Vector3(0, 300f, 0);
      }
      yield return new WaitForSeconds(0.02f);
    }

    float finalYPosition = 50f + ( targetStopIndex * 50f );
    SlotSkillObject[SlotIndex].transform.localPosition = new Vector3(
        SlotSkillObject[SlotIndex].transform.localPosition.x,
        finalYPosition,
        SlotSkillObject[SlotIndex].transform.localPosition.z
    );

    // ✅ 연출에서도 선택한 스킬이 다시 등장하지 않도록 방지
    if ( SelectedSkills.Contains(resultIndex) )
    {
      Debug.Log("⚠️ 이미 선택된 스킬이므로 연출에서도 제외됨: " + SkillNames[resultIndex]);
      yield break;
    }

    DisplayItemSlots[SlotIndex].SlotSprite[targetStopIndex].sprite = SkillSprite[resultIndex];
    DisplayItemSlots[SlotIndex].SkillNameText.text = SkillNames[resultIndex];
    DisplayItemSlots[SlotIndex].SkillDescriptionText.text = SkillDescriptions[resultIndex];

    Slot[SlotIndex].interactable = true;
  }

  public void ClickBtn(int index)
  {
    int selectedSkillIndex = ResultIndexList[index];

    // ✅ 선택한 스킬을 전역 리스트에 추가하여 다시 등장하지 않도록 함
    if ( !SelectedSkills.Contains(selectedSkillIndex) )
    {
      SelectedSkills.Add(selectedSkillIndex);
    }

    DisplayResultImage.sprite = SkillSprite[selectedSkillIndex];

    PausePanelManager pausePanelManager = FindObjectOfType<PausePanelManager>();
    if ( pausePanelManager != null )
    {
      pausePanelManager.UpdateSkillIcons(SelectedSkills, SkillSprite);
    }
    else
    {
      Debug.LogError("PausePanelManager not found!");
    }

    ApplySkillEffect(selectedSkillIndex);
    GameManager.Instance.EndSlotMachine();
  }


  private void ApplySkillEffect(int selectedSkillIndex)
  {
    string selectedSkillName = SkillSprite[selectedSkillIndex].name;
    GameObject playerObject = GameObject.FindWithTag("Player");

    if ( playerObject != null )
    {
      PlayerSkillController playerSkillController = playerObject.GetComponent<PlayerSkillController>();
      if ( playerSkillController != null )
      {
        switch ( selectedSkillName )
        {
          case "DiagonalArrow":
            playerSkillController.ApplyDiagonalArrow();
            break;
          case "PoisonArrow":
            playerSkillController.ApplyPoisonArrow();
            break;
          case "IceArrow":
            playerSkillController.ApplyFrostArrow();
            break;
          case "ATK_UP":
            playerSkillController.IncreaseAttackPower();
            break;
          case "SpeedUp":
            playerSkillController.IncreaseMovementSpeed();
            break;
          case "Heal":
            playerSkillController.RecoverHealth();
            break;
          case "HpUp":
            playerSkillController.IncreaseMaxHealth();
            break;
          case "ShotSpeedUp":
            playerSkillController.IncreaseAttackSpeed();
            break;
          case "DoubleShot":
            playerSkillController.ApplyDoubleShot();
            break;
          case "MultiShot":
            playerSkillController.ApplyMultiShot();
            break;
          case "BounceShot":
            playerSkillController.ApplyBounceShot();
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
