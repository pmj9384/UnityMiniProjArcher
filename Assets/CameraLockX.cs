using UnityEngine;
using Cinemachine;

public class CameraXLock : MonoBehaviour
{
  public CinemachineVirtualCamera virtualCamera;
  public Transform player; // 플레이어 Transform

  public float cameraHeight = 10f; // 카메라 Y축 고정값
  public float cameraDistance = -10f; // 카메라 Z축 거리

  // 🔥 카메라 이동 제한 영역 (바운드 설정)
  public float minX = -2f;
  public float maxX = 2f;
  public float minY = 5f;
  public float maxY = 15f;
  public float minZ = -20f;
  public float maxZ = 0f;

  private void LateUpdate()
  {
    if ( virtualCamera != null && player != null )
    {
      // 플레이어를 따라가되, 바운드 내에서만 이동하도록 설정
      float clampedX = Mathf.Clamp(player.position.x, minX, maxX);
      float clampedY = Mathf.Clamp(player.position.y + cameraHeight, minY, maxY);
      float clampedZ = Mathf.Clamp(player.position.z + cameraDistance, minZ, maxZ);

      // 카메라 위치 업데이트
      virtualCamera.transform.position = new Vector3(clampedX, clampedY, clampedZ);
    }
  }
}
