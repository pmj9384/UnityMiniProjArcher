using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
  public SaveLoadPanelManager saveLoadPanelManager;

  private void Start()
  {
    if ( saveLoadPanelManager == null )
    {
      saveLoadPanelManager = FindObjectOfType<SaveLoadPanelManager>();
    }
  }

  public void StartNewGame()
  {
    Debug.Log("🟢 새 게임 시작 - 선택 창 표시");
    if ( saveLoadPanelManager != null )
    {
      saveLoadPanelManager.StartNewGame(); // ✅ 패널을 통해 새 게임 처리
    }
    else
    {
      SceneManager.LoadScene("GameScene");
    }
  }

  public void LoadGame()
  {
    // Debug.Log("📥 게임 불러오기 - 선택 창 표시");
    // if ( saveLoadPanelManager != null )
    // {
    //   saveLoadPanelManager.LoadGame(); // ✅ 패널을 통해 로드
    // }
    // else
    // {
    SceneManager.LoadScene("GameScene");
    // Time.timeScale = 0f;
    //}
  }
}
