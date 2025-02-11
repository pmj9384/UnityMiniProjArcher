using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
  public void ChangeScene(string sceneName)
  {
    SceneManager.LoadScene(sceneName); // 씬 전환 실행
  }
}
