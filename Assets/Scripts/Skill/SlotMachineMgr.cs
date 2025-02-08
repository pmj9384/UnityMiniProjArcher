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
    ResultIndexList.Clear();
    StartList.Clear();

    for ( int i = 0; i < ItemCnt * Slot.Length; i++ )
    {
      StartList.Add(i);
    }

    for ( int i = 0; i < Slot.Length; i++ )
    {
      Slot[i].interactable = false;

      int randomIndex = Random.Range(0, StartList.Count);
      int selectedIndex = StartList[randomIndex];
      ResultIndexList.Add(selectedIndex);

      int correctPos = Mathf.Clamp(ItemCnt / 2, 0, ItemCnt - 2);
      DisplayItemSlots[i].SlotSprite[correctPos].sprite = SkillSprite[selectedIndex];

      for ( int j = 0; j < ItemCnt; j++ )
      {
        if ( j == correctPos ) continue;
        int randomSkillIndex = Random.Range(0, SkillSprite.Length);
        DisplayItemSlots[i].SlotSprite[j].sprite = SkillSprite[randomSkillIndex];
      }

      StartList.RemoveAt(randomIndex);
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

    // 슬롯에 표시된 최종 결과 스킬 이름 및 설명 업데이트
    DisplayItemSlots[SlotIndex].SlotSprite[targetStopIndex].sprite = SkillSprite[resultIndex];
    DisplayItemSlots[SlotIndex].SkillNameText.text = SkillNames[resultIndex];
    DisplayItemSlots[SlotIndex].SkillDescriptionText.text = SkillDescriptions[resultIndex];

    Slot[SlotIndex].interactable = true;
  }

  public void ClickBtn(int index)
  {
    int selectedSkillIndex = ResultIndexList[index];

    // 최종 선택된 스킬의 결과 표시
    DisplayResultImage.sprite = SkillSprite[selectedSkillIndex];
    SelectedSkills.Add(selectedSkillIndex);

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
