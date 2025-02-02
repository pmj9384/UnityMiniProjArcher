using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
  public Button pauseButton;
  public Button resumeButton;

  private void Start()
  {
    pauseButton.onClick.AddListener(() => GameManager.Instance.PauseGame());
    resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
  }
}
