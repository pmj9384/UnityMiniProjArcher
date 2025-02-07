using UnityEngine;


public class PlayerSkillController : MonoBehaviour
{
  public Gun gun;

  private PlayerMovement playerMovement;
  private PlayerHealth playerHealth;
  public GunData gunData;
  public ParticleSystem poisonEffectPrefab;
  public ParticleSystem frostEffectPrefab;
  public bool HasDiagonalArrow = false;
  public bool HasPoisonArrow = false;
  public bool HasFrostArrow = false;
  public bool HasDoubleShot = false;
  public bool HasMultiShot = false;


  private void Start()
  {
    playerMovement = GetComponent<PlayerMovement>();
    playerHealth = GetComponent<PlayerHealth>();
  }

  public void ApplyDiagonalArrow() => HasDiagonalArrow = true;
  public void ApplyPoisonArrow() => HasPoisonArrow = true;
  public void ApplyFrostArrow() => HasFrostArrow = true;
  public void ApplyDoubleShot() => HasDoubleShot = true;
  public void ApplyMultiShot() => HasMultiShot = true;


  public void ModifyBullet(Bullet bulletScript)
  {
    if ( bulletScript == null ) return;

    bulletScript.ClearStatusEffects();

    if ( HasFrostArrow )
    {
      FrostEffect frostEffect = new FrostEffect(2f, 1.5f, 0.1f, frostEffectPrefab);
      bulletScript.ApplyStatusEffect(frostEffect);
    }
    if ( HasPoisonArrow )
    {
      PoisonEffect poisonEffect = new PoisonEffect(2f, 10f, poisonEffectPrefab);
      bulletScript.ApplyStatusEffect(poisonEffect);
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
      playerMovement.speed += 0.1f;
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
