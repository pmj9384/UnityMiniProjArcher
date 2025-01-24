using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 패턴
    private int score = 0;
    public bool IsGameOver { get; private set; }
    public bool IsGamePause { get; private set; }

    public UiManager uiManager;

    public int remainingZombies; // 총 적의 수
    private List<GameObject> expItems = new List<GameObject>(); // 생성된 경험치 리스트

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 초기화 작업
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsGamePause)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void AddScore(int add)
    {
        if (IsGameOver) return;
        score += add;
        // uiManager.UpdateScoreText(score);
    }

    public void IncrementZombieCount()
    {
        remainingZombies++;
    }

    // 적이 처치될 때 호출되는 메서드
    public void DecrementZombieCount()
    {
        remainingZombies--;

        if (remainingZombies <= 0)
        {
            Debug.Log("모든 적 처치 완료!");

            OpenAllDoors();
            // 경험치 아이템 끌어오기 시작
            ExperienceManager.Instance.OnAllEnemiesDead();
        }

        // uiManager.UpdateWaveText(CurrentWave, remainingZombies);
    }

    // 모든 문 열기
    public void OpenAllDoors()
    {
        // Tag로 모든 문 검색
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doors)
        {
            // Door 스크립트를 통해 방향 확인
            Door doorScript = door.GetComponent<Door>();

            if (doorScript != null)
            {
                RotateDoor(door, doorScript.direction);
            }
            else
            {
                Debug.LogWarning($"문에 Door 스크립트가 없습니다: {door.name}");
            }
        }
    }

    private void RotateDoor(GameObject door, Door.DoorDirection direction)
    {
        // 방향에 따라 회전값 설정
        Quaternion targetRotation;

        if (direction == Door.DoorDirection.Left)
        {
            targetRotation = Quaternion.Euler(0, -90, 0); // Left: -90°로 회전
        }
        else // DoorDirection.Right
        {
            targetRotation = Quaternion.Euler(0, 90, 0); // Right: 90°로 회전
        }

        // 문 회전 애니메이션 실행
        StartCoroutine(OpenDoorCoroutine(door, targetRotation));
    }

    IEnumerator OpenDoorCoroutine(GameObject door, Quaternion targetRotation)
    {
        Quaternion initialRotation = door.transform.rotation;
        float duration = 1.0f; // 애니메이션 지속 시간
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            door.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.transform.rotation = targetRotation; // 최종적으로 목표 회전값 설정
    }

    // 경험치 아이템을 등록하는 메서드
    public void RegisterExp(GameObject expItem)
    {
        expItems.Add(expItem); // 생성된 경험치 아이템 등록
    }

    private void EnableAllExpItems()
    {
        foreach (var expItem in expItems)
        {
            if (expItem != null)
            {
                expItem.GetComponent<EnemyExp>().EnableCollection();
            }
        }
        expItems.Clear(); // 리스트 초기화
    }

    public void OnGameOver()
    {
        IsGameOver = true;
        uiManager.ShowGameOverPanel(true);
        Time.timeScale = 0f;
    }

    public void PauseGame()
    {
        IsGamePause = true;
        Time.timeScale = 0f;
        uiManager.ShowGamePausePanel(true);
    }

    public void ResumeGame()
    {
        IsGamePause = false;
        Time.timeScale = 1f;
        uiManager.HideGamePausePanel();
    }

    public void HandleLevelUp()
    {
        // PauseGame();
        uiManager.ShowSlotMachinePanel(true); 
    }

    public void EndSlotMachine()
    {
        // ResumeGame();  // 게임을 재개
        uiManager.HideSlotMachinePanel();  // 슬롯 머신 UI 비활성화
    }
}
