using UnityEngine;
using UnityEngine.Pool;

public class PlayerSkillController : MonoBehaviour
{
  public GameObject bulletPrefab;
  public Transform leftShootPoint;
  public Transform rightShootPoint;

  private PlayerMovement playerMovement;
  private PlayerHealth playerHealth;
  public GunData gunData;

  private bool hasDiagonalArrow = false;

  private void Start()
  {
    playerMovement = GetComponent<PlayerMovement>();
    playerHealth = GetComponent<PlayerHealth>();
  }

  public void ApplyDiagonalArrow()
  {
    if ( bulletPrefab != null )
    {
      hasDiagonalArrow = true;
      if ( leftShootPoint != null )
      {
        Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation * Quaternion.Euler(0, -45, 0));
      }

      if ( rightShootPoint != null )
      {
        Instantiate(bulletPrefab, rightShootPoint.position, rightShootPoint.rotation * Quaternion.Euler(0, 45, 0));
      }

      Debug.Log("Diagonal Arrow effect applied!");
    }
    else
    {
      Debug.LogError("BulletPrefab is missing!");
    }
  }

  public void ApplyFireArrow()
  {
    if ( bulletPrefab != null )
    {
      if ( hasDiagonalArrow )
      {
        LaunchBulletWithEffect(leftShootPoint, ApplyFireDamage);
        LaunchBulletWithEffect(rightShootPoint, ApplyFireDamage);
      }
      else
      {
        LaunchBulletWithEffect(leftShootPoint, ApplyFireDamage);
      }

      Debug.Log("Fire Arrow effect applied!");
    }
    else
    {
      Debug.LogError("BulletPrefab is missing!");
    }
  }

  public void ApplyFrostArrow()
  {
    if ( bulletPrefab != null )
    {
      if ( hasDiagonalArrow )
      {
        LaunchBulletWithEffect(leftShootPoint, ApplyFrostEffect);
        LaunchBulletWithEffect(rightShootPoint, ApplyFrostEffect);
      }
      else
      {
        LaunchBulletWithEffect(leftShootPoint, ApplyFrostEffect);
      }

      Debug.Log("Frost Arrow effect applied!");
    }
    else
    {
      Debug.LogError("BulletPrefab is missing!");
    }
  }

  private void LaunchBulletWithEffect(Transform shootPoint, System.Action<GameObject> applyEffect)
  {
    GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

    if ( bullet != null )
    {
      applyEffect(bullet); // 특정 효과를 적용
    }
  }

  private void ApplyFireDamage(GameObject bullet)
  {
    Bullet bulletScript = bullet.GetComponent<Bullet>();
    if ( bulletScript != null )
    {
      bulletScript.damage += 5f; // 화염 추가 데미지
      bulletScript.speed += 2f;  // 속도 증가
      bulletScript.lifeTime += 1f; // 생명시간 증가

      // 화염 효과를 시각적으로 표현하는 ParticleSystem 추가
      ParticleSystem fireEffect = bullet.GetComponentInChildren<ParticleSystem>();
      if ( fireEffect != null )
      {
        fireEffect.Play();
      }
    }
  }

  private void ApplyFrostEffect(GameObject bullet)
  {
    Bullet bulletScript = bullet.GetComponent<Bullet>();
    if ( bulletScript != null )
    {
      bulletScript.damage -= 2f; // 얼음 효과로 기본 데미지 감소
      bulletScript.speed -= 2f;  // 속도 감소
      bulletScript.lifeTime += 0.5f; // 생명시간 증가

      // 얼음 효과를 시각적으로 표현하는 ParticleSystem 추가
      ParticleSystem frostEffect = bullet.GetComponentInChildren<ParticleSystem>();
      if ( frostEffect != null )
      {
        frostEffect.Play();
      }
    }
  }

  public void IncreaseAttackPower()
  {
    if ( gunData != null )
    {
      gunData.damage += 10f;
      Debug.Log("Attack power increased!");
    }
    else
    {
      Debug.LogError("GunData is missing!");
    }
  }

  public void IncreaseMovementSpeed()
  {
    if ( playerMovement != null )
    {
      playerMovement.speed += 1f;
      Debug.Log("Movement speed increased!");
    }
    else
    {
      Debug.LogError("PlayerMovement is missing!");
    }
  }

  public void RecoverHealth()
  {
    if ( playerHealth != null )
    {
      playerHealth.AddHp(30f);
      Debug.Log("Health recovered!");
    }
    else
    {
      Debug.LogError("PlayerHealth is missing!");
    }
  }

  public void IncreaseMaxHealth()
  {
    if ( playerHealth != null )
    {
      playerHealth.maxHp += 10f;
      playerHealth.AddHp(10f);
      playerHealth.healthSlider.maxValue = playerHealth.maxHp;
      Debug.Log("Max health increased!");
    }
    else
    {
      Debug.LogError("PlayerHealth is missing!");
    }
  }

  public void IncreaseAttackSpeed()
  {
    if ( gunData != null )
    {
      gunData.fireRate = Mathf.Max(0.1f, gunData.fireRate - 0.1f);
      Debug.Log("Attack speed increased!");
    }
    else
    {
      Debug.LogError("GunData is missing!");
    }
  }
}
