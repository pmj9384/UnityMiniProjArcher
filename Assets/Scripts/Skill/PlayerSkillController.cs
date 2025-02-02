using UnityEngine;


public class PlayerSkillController : MonoBehaviour
{
  public Gun gun;

  private PlayerMovement playerMovement;
  private PlayerHealth playerHealth;
  public GunData gunData;

  public bool HasDiagonalArrow = false;
  public bool HasPoisonArrow = false;
  public bool HasFrostArrow = false;

  private void Start()
  {
    playerMovement = GetComponent<PlayerMovement>();
    playerHealth = GetComponent<PlayerHealth>();
  }

  public void ApplyDiagonalArrow() => HasDiagonalArrow = true;
  public void ApplyPoisonArrow() => HasPoisonArrow = true;
  public void ApplyFrostArrow() => HasFrostArrow = true;

  public void ModifyBullet(Bullet bulletScript)
  {
    if ( bulletScript == null ) return;

    bulletScript.ClearStatusEffects();

    if ( HasPoisonArrow )
    {
      bulletScript.ApplyStatusEffect(new PoisonEffect(2f, 10f)); //2초동안 20뎀
    }

    if ( HasFrostArrow )
    {
      bulletScript.ApplyStatusEffect(new FrostEffect(2f, 1.5f)); // ✅ 2초 동안 속도 감소
    }
  }

  public void IncreaseAttackPower()
  {
    if ( gunData != null )
    {
      gunData.damage += 10f;
      Debug.Log("Attack power increased!");
    }
  }

  public void IncreaseMovementSpeed()
  {
    if ( playerMovement != null )
    {
      playerMovement.speed += 1f;
      Debug.Log("Movement speed increased!");
    }
  }

  public void RecoverHealth()
  {
    if ( playerHealth != null )
    {
      playerHealth.AddHp(30f);
      Debug.Log("Health recovered!");
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
  }

  public void IncreaseAttackSpeed()
  {
    if ( gunData != null )
    {
      gunData.fireRate = Mathf.Max(0.1f, gunData.fireRate - 0.1f);
      Debug.Log("Attack speed increased!");
    }
  }
}
