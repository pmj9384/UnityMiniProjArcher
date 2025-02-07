using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
  public GameObject gameOverPanel;


  public void RestartGame()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
  }


  public void QuitGame()
  {
    Debug.Log("게임 종료!");
    Application.Quit(); // 게임 종료 (빌드 환경에서만 작동)

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
  }
}
