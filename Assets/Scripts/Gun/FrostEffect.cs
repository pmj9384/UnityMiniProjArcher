using System.Collections;
using UnityEngine;

public class FrostEffect : IStatusEffect
{
  private float duration;
  private float slowAmount;
  private float tickDamage;
  private ParticleSystem frostEffectPrefab;

  public FrostEffect(float duration, float slowAmount, float tickDamage, ParticleSystem effectPrefab)
  {
    this.duration = duration;
    this.slowAmount = slowAmount;
    this.tickDamage = tickDamage;
    this.frostEffectPrefab = effectPrefab;
  }
  public IEnumerator ApplyEffect(LivingEntity entity)
  {
    if ( entity == null || entity.IsDead ) yield break;

    Enemy enemy = entity as Enemy;
    if ( enemy != null )
    {
      Debug.Log("❄️ 얼음 효과 적용 및 0.1 데미지 추가!");
      enemy.AddHitEffect(frostEffectPrefab);

      IMoveBehavior moveBehavior = enemy.GetMoveBehavior();
      BossMove bossMove = moveBehavior as BossMove;

      bool isBossDashing = bossMove != null && bossMove.IsDashing();

      float originalSpeed = enemy.GetSpeed();

      if ( !isBossDashing )
      {
        enemy.ModifySpeed(originalSpeed - slowAmount);
      }

      entity.OnDamage(tickDamage, entity.transform.position, Vector3.zero);

      yield return new WaitForSeconds(duration);

      if ( !isBossDashing )
      {
        enemy.ModifySpeed(originalSpeed);
      }

      enemy.RemoveHitEffect(frostEffectPrefab);
    }
  }



}
