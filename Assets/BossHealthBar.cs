using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
  public Slider healthSlider;
  private Enemy boss;
  private Player player;

  private void Start()
  {
    boss = GetComponent<Enemy>();
    if ( boss == null )
    {
      boss = FindObjectOfType<Enemy>();
    }

    if ( boss == null )
    {
      Debug.LogError("❌ BossHealthBar: Enemy (보스) 컴포넌트를 찾을 수 없음!");
      return;
    }

    player = Player.Instance; // ✅ 플레이어 찾기
    if ( player != null && player.experienceSlider != null )
    {
      player.experienceSlider.gameObject.SetActive(false); // ✅ 보스 등장 시 경험치 바 숨김
    }

    if ( healthSlider != null )
    {
      healthSlider.maxValue = boss.GetMaxHealth();
      healthSlider.value = boss.GetCurrentHealth();
    }
  }

  private void Update()
  {
    if ( boss != null && healthSlider != null )
    {
      healthSlider.value = boss.GetCurrentHealth();
    }

    if ( boss != null && boss.IsDead ) // ✅ 보스가 죽었을 때
    {
      if ( player != null && player.experienceSlider != null )
      {
        player.experienceSlider.gameObject.SetActive(true); // ✅ 경험치 바 다시 활성화
      }

      //Destroy(gameObject); // ✅ 보스 체력바 제거
    }
  }

  public void UpdateHealth(float newHealth)
  {
    if ( healthSlider != null )
    {
      healthSlider.value = newHealth;
    }
  }
}
