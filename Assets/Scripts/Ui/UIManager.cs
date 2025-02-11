using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
  public TextMeshProUGUI scoreText;
  public TextMeshProUGUI waveText;
  public GameObject gameOverPanel;
  public GameObject gameClearPanel;
  public GameObject pausePanel;
  public GameObject slotMachinePanel;

  public VirtualJoyStick joystick;


  // public void UpdateScoreText(int newScore)
  // {
  //     scoreText.text = $"SCORE: {newScore}";
  // }

  // public void UpdateWaveText(int wave, int count)
  // {
  //     waveText.text = $"Wave: {wave}\nEnemy Left: {count}";
  // }

  public void ShowGameOverPanel(bool active)
  {
    Debug.Log("Game Over");
    gameOverPanel.SetActive(active);
    joystick.SetJoystickActive(!active);

  }

  public void ShowGamePausePanel(bool active)
  {
    pausePanel.SetActive(active);
    joystick.SetJoystickActive(!active);

  }
  public void ShowSlotMachinePanel()
  {
    slotMachinePanel.SetActive(true);
    joystick.ResetInput();
    joystick.SetJoystickActive(false);

  }

  public void HideSlotMachinePanel()
  {
    slotMachinePanel.SetActive(false);
    joystick.SetJoystickActive(true);
  }


  public void HideGamePausePanel()
  {
    pausePanel.SetActive(false);
    joystick.SetJoystickActive(true);
  }
  public void ShowGameClearPanel(bool active)
  {
    Debug.Log("🎉 Game Clear!");
    gameClearPanel.SetActive(active);
    joystick.ResetInput();
    joystick.SetJoystickActive(!active);
  }

}
