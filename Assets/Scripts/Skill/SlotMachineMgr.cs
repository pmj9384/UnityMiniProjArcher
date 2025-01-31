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

    DisplayItemSlots[SlotIndex].SlotSprite[targetStopIndex].sprite = SkillSprite[resultIndex];

    Slot[SlotIndex].interactable = true;
  }

  public void ClickBtn(int index)
  {
    int selectedSkillIndex = ResultIndexList[index];
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
          case "FireArrow":
            playerSkillController.ApplyFireArrow();
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
