using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour
{
  public TextMeshProUGUI fpsText;
  private bool isFpsVisible = false;
  private float deltaTime = 0.0f;

  private void Start()
  {
#if UNITY_EDITOR || DEVELOPMENT_BUILD ||UNITY_ANDROID
    isFpsVisible = true;
    fpsText.gameObject.SetActive(true);
#else
            isFpsVisible = false;
            fpsText.gameObject.SetActive(false);
#endif
  }

  private void Update()
  {
    if ( !isFpsVisible ) return;

    deltaTime += ( Time.unscaledDeltaTime - deltaTime ) * 0.1f;
    float fps = 1.0f / deltaTime;
    fpsText.text = $"FPS: {Mathf.Ceil(fps)}";

    if ( Input.GetKeyDown(KeyCode.F) )
    {
      ToggleFPSDisplay();
    }
  }

  public void ToggleFPSDisplay()
  {
    isFpsVisible = !isFpsVisible;
    fpsText.gameObject.SetActive(isFpsVisible);
  }
}
