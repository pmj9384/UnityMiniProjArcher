using System.Collections;
using UnityEngine;

public class FrostEffect : IStatusEffect
{
  private float duration;
  private float slowAmount;

  public FrostEffect(float duration, float slowAmount)
  {
    this.duration = duration;
    this.slowAmount = slowAmount;
  }

  public IEnumerator ApplyEffect(LivingEntity entity)
  {
    if ( entity is Enemy enemy )
    {
      if ( enemy == null || enemy.IsDead ) yield break;

      float originalSpeed = enemy.GetSpeed();
      enemy.ModifySpeed(originalSpeed - slowAmount); // ✅ 속도 감소 적용

      yield return new WaitForSeconds(duration);

      if ( enemy != null )
      {
        enemy.ModifySpeed(originalSpeed); // ✅ 원래 속도로 복구
      }
    }
  }
}
