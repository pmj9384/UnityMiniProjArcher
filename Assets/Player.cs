using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가

public class Player : MonoBehaviour
{
  public static Player Instance;
  private int currentExperience;
  private int currentLevel = 1;
  public Slider experienceSlider;  // UI 슬라이더를 연결

  public TextMeshProUGUI levelText; // 현재 레벨을 표시할 TextMeshPro 텍스트

  public Vector3 initialSpawnPosition;  // 첫 번째 맵에서의 초기 위치
  private const string initialPositionKeyX = "InitialPosX";
  private const string initialPositionKeyY = "InitialPosY";
  private const string initialPositionKeyZ = "InitialPosZ";

  public int[] experienceForLevels = { 10, 110, 195, 270, 350, 500 }; // 각 레벨에 필요한 경험치
  public int maxLevel = 7;

  void Awake()
  {
    Instance = this;
    LoadPlayerData();  // 게임 시작 시, 플레이어 데이터 불러오기
  }

  void Start()
  {
    // 처음 시작할 때 위치 저장
    if ( !PlayerPrefs.HasKey(initialPositionKeyX) )
    {
      SaveInitialPosition();
    }
    UpdateExperienceSlider();  // 경험치 바 업데이트
    UpdateLevelText();         // 레벨 텍스트 업데이트
  }

  public void AddExperience(int amount)
  {
    if ( currentLevel >= maxLevel ) return;  // 최대 레벨 도달 시 경험치 증가 X

    currentExperience += amount;
    UpdateExperienceSlider();
    // ✅ 경험치 변경될 때 즉시 저장
    SavePlayerData();

    // ✅ 경험치가 다음 레벨을 초과하면 레벨업
    while ( currentLevel < maxLevel && currentExperience >= experienceForLevels[currentLevel - 1] )
    {
      LevelUp();
    }

    Debug.Log($"✅ 경험치 {amount} 획득! 현재 경험치: {currentExperience}");

  }

  private void LevelUp()
  {
    currentLevel++;
    currentExperience -= experienceForLevels[currentLevel - 2];
    if ( currentExperience < 0 ) currentExperience = 0;

    UpdateLevelText();
    NotifyLevelUp();

    // ✅ 레벨 변경될 때 저장
    SavePlayerData();
  }




  private void SavePlayerData()
  {
    // if ( GameManager.Instance != null )
    // {
    //   GameData data = GameManager.Instance.gameData;
    //   data.playerLevel = currentLevel;
    //   data.playerExp = currentExperience;

    //   // ✅ 기존 SaveSystem 대신 SaveLoadManager 사용
    //   GameManager.Instance.saveLoadManager.SaveGame(data);

    //   Debug.Log("💾 플레이어 경험치 & 레벨 저장됨!");
    // }
  }

  private void UpdateExperienceSlider()
  {
    if ( currentLevel >= maxLevel )
    {
      experienceSlider.value = 1f;  // ✅ 최대 레벨이면 경험치 바 꽉 채우기
    }
    else
    {
      experienceSlider.value = ( float )currentExperience / experienceForLevels[currentLevel - 1];
    }
  }


  private void UpdateLevelText()
  {
    // TextMeshPro 텍스트에 현재 레벨 표시
    levelText.text = "Lv." + currentLevel;
  }

  private void NotifyLevelUp()
  {
    // 레벨업 시 GameManager에 알리기
    if ( GameManager.Instance != null )
    {
      GameManager.Instance.HandleLevelUp();  // GameManager에 레벨업 알림
    }
  }

  // 플레이어의 시작 위치 저장
  private void SaveInitialPosition()
  {
    PlayerPrefs.SetFloat(initialPositionKeyX, transform.position.x);
    PlayerPrefs.SetFloat(initialPositionKeyY, transform.position.y);
    PlayerPrefs.SetFloat(initialPositionKeyZ, transform.position.z);
  }

  // 저장된 플레이어의 시작 위치로 이동
  public void SetInitialPosition()
  {
    float x = PlayerPrefs.GetFloat(initialPositionKeyX);
    float y = PlayerPrefs.GetFloat(initialPositionKeyY);
    float z = PlayerPrefs.GetFloat(initialPositionKeyZ);

    // 플레이어의 위치를 초기 위치로 설정
    transform.position = new Vector3(x, y, z);
  }

  // 저장된 플레이어 데이터 불러오기
  public void LoadPlayerData()
  {
    // if ( GameManager.Instance != null && GameManager.Instance.saveLoadManager.HasSaveData() )
    // {
    //   GameData data = GameManager.Instance.saveLoadManager.LoadGame();
    //   currentLevel = data.playerLevel;
    //   currentExperience = data.playerExp;
    // }
    // else
    // {
    //   // 저장된 데이터 없을 경우 기본값 설정
    //   currentLevel = 1;
    //   currentExperience = 0;
    // }

    // UpdateExperienceSlider();
    // UpdateLevelText();
  }
  public void SetExperienceAndLevel(int exp, int level)
  {
    currentExperience = exp;
    currentLevel = Mathf.Clamp(level, 1, maxLevel); // ✅ 레벨을 1~7로 고정

    UpdateExperienceSlider();
    UpdateLevelText();
  }
  public int GetCurrentExperience()
  {
    return currentExperience;
  }

  public int GetCurrentLevel()
  {
    return currentLevel;
  }

}
