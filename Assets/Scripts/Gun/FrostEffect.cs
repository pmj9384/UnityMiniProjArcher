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
    //if ( entity == null || entity.IsDead ) yield break;

    Enemy enemy = entity as Enemy;
    if ( enemy != null )
    {
      Debug.Log("❄️ 얼음 효과 적용 및 0.1 데미지 추가!");

      // ✅ **이펙트 먼저 적용**
      enemy.AddHitEffect(frostEffectPrefab);

      // ✅ **데미지를 먼저 적용**
      entity.OnDamage(tickDamage, entity.transform.position, Vector3.zero);

      // ✅ **한 프레임 대기 후 사망 여부 확인**
      yield return null;

      if ( entity.IsDead )
      {
        // 💡 적이 즉사해도 이펙트는 0.3초 유지 후 제거
        yield return new WaitForSeconds(0.3f);
        enemy.RemoveHitEffect(frostEffectPrefab);
        yield break;
      }

      // ✅ **속도 감소 적용**
      IMoveBehavior moveBehavior = enemy.GetMoveBehavior();
      BossMove bossMove = moveBehavior as BossMove;

      bool isBossDashing = bossMove != null && bossMove.IsDashing();
      float originalSpeed = enemy.GetSpeed();

      if ( !isBossDashing )
      {
        enemy.ModifySpeed(originalSpeed - slowAmount);
      }

      yield return new WaitForSeconds(duration);

      // ✅ **이펙트 지속 시간 후 속도 복구**
      if ( !isBossDashing )
      {
        enemy.ModifySpeed(originalSpeed);
      }

      // ✅ **이펙트 제거**
      enemy.RemoveHitEffect(frostEffectPrefab);
    }
  }
}
