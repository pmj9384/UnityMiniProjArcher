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

      float originalSpeed = enemy.GetSpeed();
      enemy.ModifySpeed(originalSpeed - slowAmount);

      // ✅ 첫 번째 적중 시 0.1의 미세한 데미지 적용 (파티클 트리거)
      entity.OnDamage(tickDamage, entity.transform.position, Vector3.zero);

      yield return new WaitForSeconds(duration);

      enemy.ModifySpeed(originalSpeed);
      enemy.RemoveHitEffect(frostEffectPrefab);
    }
  }
}
