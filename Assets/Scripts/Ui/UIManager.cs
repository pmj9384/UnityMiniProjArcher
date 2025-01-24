using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;  
    public GameObject slotMachinePanel;

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
        gameOverPanel.SetActive(active);
    }

    public void ShowGamePausePanel(bool active)
    {
        pausePanel.SetActive(active);  
    }
    public void ShowSlotMachinePanel(bool active)
    {
        slotMachinePanel.SetActive(active);  
    }

    public void HideSlotMachinePanel()
    {
        if (slotMachinePanel != null)
        {
            slotMachinePanel.SetActive(false);  // 슬롯 머신 UI 비활성화
        }
    }

    public void HideGamePausePanel()
    {
        pausePanel.SetActive(false);  
    }
}
