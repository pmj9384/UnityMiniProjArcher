using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
  public Button pauseButton;
  public Button resumeButton;
  public Button mainMenuButton;

  public GameObject muteButton;        // 음소거 버튼 GameObject
  public GameObject unmuteButton;     // 소리 활성화 버튼 GameObject

  private bool isMuted = false;       // 음소거 상태

  private void Start()
  {
    pauseButton.onClick.AddListener(() => GameManager.Instance.PauseGame());
    resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
    mainMenuButton.onClick.AddListener(GoToMainMenu);
    // 버튼 클릭 이벤트 연결
    muteButton.GetComponent<Button>().onClick.AddListener(ToggleSound);
    unmuteButton.GetComponent<Button>().onClick.AddListener(ToggleSound);

    // 게임 시작 시 소리 상태 확인 후 버튼 초기화
    isMuted = PlayerPrefs.GetInt("Muted", 0) == 1; // PlayerPrefs에서 이전 상태 로드
    UpdateSoundState();
  }

  private void ToggleSound()
  {
    isMuted = !isMuted; // 🔹 음소거 상태 먼저 변경

    // 🔹 변경된 상태를 기반으로 UI와 소리 업데이트
    UpdateSoundState();

    // 상태를 저장하여 다음 게임 실행 시 유지
    PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    PlayerPrefs.Save();
  }

  private void UpdateSoundState()
  {
    AudioListener.volume = isMuted ? 0 : 1; // 소리 조절

    // 🔹 UI 버튼 활성화/비활성화
    muteButton.SetActive(!isMuted);  // 음소거 버튼 (소리 켜져있을 때 표시)
    unmuteButton.SetActive(isMuted); // 소리 활성화 버튼 (소리 꺼졌을 때 표시)
  }
  private void GoToMainMenu()
  {
    // 🔹 메인 메뉴 씬으로 이동
    SceneManager.LoadScene("StartMenu"); // "MainMenu"는 메인 메뉴 씬 이름
    Time.timeScale = 1;
  }

}
