using UnityEngine;

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
        GameObject leftBullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
        GameObject rightBullet = Instantiate(bulletPrefab, rightShootPoint.position, rightShootPoint.rotation);

        if ( leftBullet != null )
        {
          ApplyFireDamage(leftBullet);
        }

        if ( rightBullet != null )
        {
          ApplyFireDamage(rightBullet);
        }

        Debug.Log("Fire Arrow effect applied with diagonal arrow!");
      }
      else
      {
        GameObject bullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
        ApplyFireDamage(bullet);
        Debug.Log("Fire Arrow effect applied without diagonal arrow!");
      }
    }
    else
    {
      Debug.LogError("BulletPrefab is missing!");
    }
  }

  void ApplyFireDamage(GameObject bullet)
  {
    RaycastHit hit;
    if ( Physics.Raycast(bullet.transform.position, bullet.transform.forward, out hit) )
    {
      Enemy enemy = hit.collider.GetComponent<Enemy>();
      if ( enemy != null )
      {
        enemy.ApplyFireEffect(2f, 5f);
      }
    }
  }

  public void ApplyFrostArrow()
  {
    if ( bulletPrefab != null )
    {
      if ( hasDiagonalArrow )
      {
        GameObject leftBullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
        GameObject rightBullet = Instantiate(bulletPrefab, rightShootPoint.position, rightShootPoint.rotation);

        if ( leftBullet != null )
        {
          ApplyFrostEffect(leftBullet);
        }

        if ( rightBullet != null )
        {
          ApplyFrostEffect(rightBullet);
        }

        Debug.Log("Frost Arrow effect applied with diagonal arrow!");
      }
      else
      {
        GameObject bullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
        ApplyFrostEffect(bullet);
        Debug.Log("Frost Arrow effect applied without diagonal arrow!");
      }
    }
    else
    {
      Debug.LogError("BulletPrefab is missing!");
    }
  }

  void ApplyFrostEffect(GameObject bullet)
  {
    RaycastHit hit;
    if ( Physics.Raycast(bullet.transform.position, bullet.transform.forward, out hit) )
    {
      Enemy enemy = hit.collider.GetComponent<Enemy>();
      if ( enemy != null )
      {
        enemy.ApplyFrostEffect(2f, 1f);
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
