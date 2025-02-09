using UnityEngine;
using Cinemachine;

public class CameraXLock : MonoBehaviour
{
  public CinemachineVirtualCamera virtualCamera;
  public Transform player; // 플레이어 Transform
  public float fixedX = 0f; // X축 고정값
  public float cameraHeight = 10f; // 카메라 Y축 고정값
  public float cameraDistance = -10f; // 카메라 Z축 거리

  private void LateUpdate()
  {
    if ( virtualCamera != null && player != null )
    {
      // 새로운 위치 설정
      Vector3 newPosition = new Vector3(
          fixedX,                     // X축 고정
          player.position.y + cameraHeight,  // 플레이어 Y축 따라가기
          player.position.z + cameraDistance // 플레이어 기준 Z 위치
      );

      // 적용
      virtualCamera.transform.position = newPosition;
    }
  }
}
