using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }
  private int score = 0;
  public bool IsGameOver { get; private set; }
  public bool IsGamePause { get; private set; }

  public UiManager uiManager;
  public int remainingZombies;
  public SaveLoadManager saveLoadManager;
  public GameData gameData;
  public GameObject saveLoadPanel;
  public GameObject uiCanvas;
  private MapManager mapManager;

  private void Awake()
  {
    Instance = this;
    saveLoadManager = FindObjectOfType<SaveLoadManager>();
    mapManager = FindObjectOfType<MapManager>();

    if ( saveLoadManager == null )
      Debug.LogError("❌ SaveLoadManager가 씬에 존재하지 않습니다!");

    if ( mapManager == null )
      Debug.LogError("❌ MapManager가 씬에 존재하지 않습니다!");

    if ( saveLoadManager != null && saveLoadManager.HasSaveData() )
    {
      ShowSaveLoadPanel();
    }
    else
    {
      StartCoroutine(DelayedStartNewGame());
    }

    QualitySettings.vSyncCount = 0;
#if UNITY_ANDROID
    Application.targetFrameRate = 120;
#else
        Application.targetFrameRate = -1;
#endif
  }

  private IEnumerator DelayedStartNewGame()
  {
    yield return new WaitForSeconds(0.1f);
    StartNewGame();
  }

  private void ShowSaveLoadPanel()
  {
    if ( uiCanvas == null )
    {
      Debug.LogError("❌ uiCanvas가 설정되지 않았습니다! Unity Inspector에서 연결하세요.");
      return;
    }

    if ( saveLoadPanel == null )
    {
      Debug.LogError("❌ saveLoadPanel이 설정되지 않았습니다! Unity Inspector에서 연결하세요.");
      return;
    }

    saveLoadPanel.SetActive(true);
    Debug.Log("📥 SaveLoadPanel 활성화 완료!");
  }
  public void SaveGame()
  {
    if ( saveLoadManager == null ) return;

    PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
    Player player = Player.Instance;
    PausePanelManager pausePanel = FindObjectOfType<PausePanelManager>();

    gameData = new GameData
    {
      playerHP = playerHealth != null ? ( int )playerHealth.Hp : 100,
      currentStage = mapManager != null ? mapManager.GetCurrentStage() : 1,
      acquiredSkills = SlotMachineMgr.Instance != null ? new List<int>(SlotMachineMgr.Instance.SelectedSkills) : new List<int>(),
      isSlotMachineActive = uiManager != null && uiManager.IsSlotMachineActive(),
      playerExp = player != null ? player.GetCurrentExperience() : 0,
      playerLevel = player != null ? player.GetCurrentLevel() : 1,

      // ✅ `skillIcons[].sprite.name`을 저장
      acquiredSkillSprites = pausePanel != null ? new List<string>(pausePanel.GetCurrentSkillSpriteNames()) : new List<string>()
    };

    Debug.Log("📤 저장된 스킬 스프라이트 목록:");
    foreach ( string spriteName in gameData.acquiredSkillSprites )
    {
      Debug.Log($"🔹 {spriteName}");
    }

    saveLoadManager.SaveGame(gameData);
  }

  public void LoadGame()
  {
    if ( saveLoadManager == null ) return;

    gameData = saveLoadManager.LoadGame();

    if ( gameData != null )
    {
      Debug.Log($"🎮 로드된 데이터 - HP: {gameData.playerHP}, 스테이지: {gameData.currentStage}, 레벨: {gameData.playerLevel}, 경험치: {gameData.playerExp}");

      ClearExistingEnemies();

      PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
      if ( playerHealth != null )
      {
        playerHealth.Hp = gameData.playerHP;
        playerHealth.healthSlider.value = gameData.playerHP;
      }
      else
      {
        Debug.LogError("❌ PlayerHealth를 찾을 수 없습니다!");
      }

      if ( mapManager != null )
      {
        mapManager.LoadSpecificStage(gameData.currentStage);
      }

      if ( SlotMachineMgr.Instance != null )
      {
        SlotMachineMgr.Instance.SelectedSkills = new List<int>(gameData.acquiredSkills);
        foreach ( int skillIndex in gameData.acquiredSkills )
        {
          SlotMachineMgr.Instance.ApplySkillEffect(skillIndex);
        }
        Debug.Log("✅ 스킬 복원 완료!");
      }
      else
      {
        Debug.LogError("❌ SlotMachineMgr 인스턴스를 찾을 수 없습니다.");
      }

      if ( uiManager != null )
      {
        if ( gameData.isSlotMachineActive )
        {
          uiManager.ShowSlotMachinePanel();
        }
        else
        {
          uiManager.HideSlotMachinePanel();
        }
      }

      // ✅ PausePanelManager에서 스킬 아이콘 복원
      PausePanelManager pausePanel = FindObjectOfType<PausePanelManager>();
      if ( pausePanel != null )
      {
        // ✅ SlotMachineMgr가 비활성화 상태라면 강제로 활성화
        GameObject slotMachineMgrObj = GameObject.Find("SlotMachineMgr");
        bool wasInactive = false; // 원래 비활성화 상태였는지 저장

        if ( slotMachineMgrObj != null && !slotMachineMgrObj.activeSelf )
        {
          slotMachineMgrObj.SetActive(true);
          wasInactive = true;
          Debug.Log("✅ SlotMachineMgr가 비활성화 상태였음 → 활성화 완료!");
        }

        if ( SlotMachineMgr.Instance == null )
        {
          Debug.LogError("❌ LoadGame: SlotMachineMgr.Instance가 null임! SlotMachineMgr가 씬에 존재하는지 확인하세요.");
          return;
        }

        Sprite[] allSprites = SlotMachineMgr.Instance.GetAllSkillSprites();

        if ( allSprites == null || allSprites.Length == 0 )
        {
          Debug.LogError("❌ LoadGame: SlotMachineMgr.GetAllSkillSprites()가 null을 반환함. 아이콘 복원 불가능!");
          return;
        }

        Debug.Log("📥 불러온 스킬 스프라이트 목록:");
        foreach ( Sprite sprite in allSprites )
        {
          Debug.Log($"🔹 {sprite.name}");
        }

        pausePanel.RestoreSkillIcons(gameData.acquiredSkillSprites, allSprites);
        Debug.Log("✅ Pause Panel 스킬 아이콘 복원 완료!");

        // ✅ LoadGame이 끝난 후, SlotMachineMgr를 다시 비활성화
        if ( wasInactive && slotMachineMgrObj != null )
        {
          slotMachineMgrObj.SetActive(false);
          Debug.Log("🔄 LoadGame 완료 후 SlotMachineMgr 다시 비활성화!");
        }
        if ( Player.Instance != null )
        {
          Player.Instance.SetExperienceAndLevel(gameData.playerExp, gameData.playerLevel);
          Debug.Log("✅ 경험치 & 레벨 복원 완료!");
        }
        else
        {
          Debug.LogError("❌ Player 인스턴스를 찾을 수 없습니다.");
        }
      }
    }
  }



  private void ClearExistingEnemies()
  {
    GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

    foreach ( GameObject enemy in existingEnemies )
    {
      Destroy(enemy);
    }

    remainingZombies = 0; // ✅ 적 수 초기화
  }


  public void StartNewGame()
  {
    if ( saveLoadManager != null )
      saveLoadManager.DeleteSaveData();

    Debug.Log("🚀 새 게임 시작!");

    if ( mapManager != null )
      mapManager.StartNewGame();
    SaveGame();
  }

  private void OnApplicationQuit()
  {
    //SaveGame();
  }

  private void Update()
  {
    if ( Input.GetKeyDown(KeyCode.Escape) )
    {
      if ( !IsGamePause )
        PauseGame();
      else
        ResumeGame();
    }
  }

  public void OnGameOver()
  {
    IsGameOver = true;
    uiManager?.ShowGameOverPanel(true);
    Time.timeScale = 0f;
  }

  public void PauseGame()
  {
    IsGamePause = true;
    Time.timeScale = 0f;
    uiManager?.ShowGamePausePanel(true);
  }

  public void ResumeGame()
  {
    IsGamePause = false;
    Time.timeScale = 1f;
    uiManager?.HideGamePausePanel();
  }

  public void HandleLevelUp()
  {
    uiManager?.ShowSlotMachinePanel();
  }

  public void EndSlotMachine()
  {
    uiManager?.HideSlotMachinePanel();
  }

  public void AddScore(int add)
  {
    if ( IsGameOver ) return;
    score += add;
  }
  public void IncrementZombieCount()
  {
    remainingZombies++;
  }

  public void DecrementZombieCount()
  {
    remainingZombies--;

    if ( remainingZombies <= 0 )
    {
      Debug.Log("모든 적 처치 완료!");
      OpenAllDoors();
      ExperienceManager.Instance?.OnAllEnemiesDead();
    }
  }
  public void OpenAllDoors()
  {
    GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

    foreach ( GameObject door in doors )
    {
      Door doorScript = door.GetComponent<Door>();

      if ( doorScript != null )
      {
        RotateDoor(door, doorScript.direction);
      }
      else
      {
        Debug.LogWarning($"문에 Door 스크립트가 없습니다: {door.name}");
      }
    }
  }
  public bool HasSaveData()
  {
    return saveLoadManager != null && saveLoadManager.HasSaveData();
  }
  public void RotateDoor(GameObject door, Door.DoorDirection direction)
  {
    Quaternion targetRotation = direction == Door.DoorDirection.Left
        ? Quaternion.Euler(0, -90, 0)
        : Quaternion.Euler(0, 90, 0);

    StartCoroutine(OpenDoorCoroutine(door, targetRotation));
  }
  private IEnumerator OpenDoorCoroutine(GameObject door, Quaternion targetRotation)
  {
    Quaternion initialRotation = door.transform.rotation;
    float duration = 1.0f;
    float elapsedTime = 0f;

    while ( elapsedTime < duration )
    {
      door.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    door.transform.rotation = targetRotation;
  }


}
