using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private int currentExperience;
    private int currentLevel = 1; 
    public Slider experienceSlider;  // UI 슬라이더를 연결

    public Vector3 initialSpawnPosition;  // 첫 번째 맵에서의 초기 위치
    private const string initialPositionKeyX = "InitialPosX";
    private const string initialPositionKeyY = "InitialPosY";
    private const string initialPositionKeyZ = "InitialPosZ";

    public int[] experienceForLevels = {10, 110, 195, 270, 350, 500}; // 각 레벨에 필요한 경험치
    public int maxLevel = 7;

    void Awake()
    {
        Instance = this;
        LoadPlayerData();  // 게임 시작 시, 플레이어 데이터 불러오기
    }

    void Start()
    {
        // 처음 시작할 때 위치 저장
        if (!PlayerPrefs.HasKey(initialPositionKeyX))
        {
            SaveInitialPosition();
        }
        UpdateExperienceSlider();  // 경험치 바 업데이트
    }

    public void AddExperience(int amount)
    {
        if (currentLevel >= maxLevel) return;  // 최대 레벨 도달 시 더 이상 경험치 증가하지 않음

        currentExperience += amount;
        
        // 레벨업 처리를 위해 경험치가 현재 레벨에 필요한 경험치를 넘었는지 확인
        while (currentLevel < maxLevel && currentExperience >= experienceForLevels[currentLevel - 1])
        {
            LevelUp();  // 레벨업
        }

        Debug.Log("Gained " + amount + " experience!");
        UpdateExperienceSlider();  // 경험치 바 업데이트
    }

    private void LevelUp()
    {
        currentLevel++;  // 레벨 증가
        currentExperience -= experienceForLevels[currentLevel - 2];  // 레벨업 후 남은 경험치는 다음 레벨을 위해 남겨둠
        if (currentExperience < 0) currentExperience = 0;

        if (currentLevel < maxLevel)
        {
            NotifyLevelUp();    
        }
    }

    private void UpdateExperienceSlider()
    {
        // 슬라이더의 값은 경험치 / 현재 레벨에서 다음 레벨로 가기 위한 경험치
        if (currentLevel < maxLevel)
        {
            experienceSlider.value = (float)currentExperience / experienceForLevels[currentLevel - 1];
        }
        else
        {
            experienceSlider.value = 1f;  // 최대 레벨에 도달하면 슬라이더를 꽉 채운다.
        }
    }

    private void NotifyLevelUp()
    {
        // 레벨업 시 GameManager에 알리기
        if (GameManager.Instance != null)
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
        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("PlayerLevel");
            currentExperience = PlayerPrefs.GetInt("PlayerExperience");
        }
        else
        {
            // 데이터가 없다면 기본 시작 값으로 설정
            currentLevel = 1;
            currentExperience = 0;
        }
    }
}
