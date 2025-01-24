using UnityEngine;

public class PlayerHpBarPs : MonoBehaviour
{
    public Transform player; // 플레이어 Transform
    public Vector3 offset;   // UI 위치 오프셋 (월드 좌표 기준)

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        // 플레이어 월드 좌표를 기준으로 RectTransform 위치 설정
        rectTransform.position = player.position + offset;

        // 항상 카메라를 향하도록 회전 조정
        rectTransform.rotation = Camera.main.transform.rotation;
    }
}
