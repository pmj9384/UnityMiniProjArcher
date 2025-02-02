using System.Collections;
using UnityEngine;

public class PoisonEffect : IStatusEffect
{
  private float duration;
  private float damage;
  private ParticleSystem poisonEffectPrefab; // ✅ 독 파티클 프리팹

  public PoisonEffect(float duration, float damage, ParticleSystem effectPrefab)
  {
    this.duration = duration;
    this.damage = damage;
    this.poisonEffectPrefab = effectPrefab;
  }

  public IEnumerator ApplyEffect(LivingEntity entity)
  {
    if ( entity == null || entity.IsDead ) yield break;

    Enemy enemy = entity as Enemy;
    if ( enemy != null ) enemy.AddHitEffect(poisonEffectPrefab); // ✅ 적중 시 파티클 추가

    float elapsedTime = 0f;
    while ( elapsedTime < duration )
    {
      if ( entity == null || entity.IsDead ) yield break;

      entity.OnDamage(damage, entity.transform.position, Vector3.zero);
      yield return new WaitForSeconds(1f);
      elapsedTime += 1f;
    }

    if ( enemy != null ) enemy.RemoveHitEffect(poisonEffectPrefab); // ✅ 지속시간 끝나면 제거
  }
}
