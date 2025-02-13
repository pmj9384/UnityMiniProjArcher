using UnityEngine;
using UnityEngine.UI;

public class SaveLoadPanelManager : MonoBehaviour
{
  public GameObject saveLoadPanel; // 🎯 세이브 로드 패널
  public Button newGameButton; // "새 게임" 버튼
  public Button loadGameButton; // "불러오기" 버튼

  private void Start()
  {
    // ✅ 버튼 클릭 시 올바른 함수 연결
    if ( newGameButton != null )
    {
      newGameButton.onClick.AddListener(StartNewGame);
    }

    if ( loadGameButton != null )
    {
      loadGameButton.onClick.AddListener(LoadGame);
    }

    // ✅ 저장 데이터가 없으면 불러오기 버튼 비활성화
    if ( !GameManager.Instance || !GameManager.Instance.HasSaveData() )
    {
      if ( loadGameButton != null )
      {
        loadGameButton.interactable = false;
      }
    }
  }

  public void StartNewGame()
  {
    if ( saveLoadPanel != null )
    {
      saveLoadPanel.SetActive(false); // ✅ 패널 숨김
    }

    GameManager.Instance?.StartNewGame();
  }

  public void LoadGame()
  {
    if ( saveLoadPanel != null )
    {
      saveLoadPanel.SetActive(false); // ✅ 패널 숨김
    }

    GameManager.Instance?.LoadGame();
  }
}
