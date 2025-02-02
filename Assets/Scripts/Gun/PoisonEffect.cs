using System.Collections;
using UnityEngine;

public class PoisonEffect : IStatusEffect
{
  private float duration;
  private float fixedDamagePerSecond; //  초당 고정 피해

  public PoisonEffect(float duration, float fixedDamagePerSecond)
  {
    this.duration = duration;
    this.fixedDamagePerSecond = fixedDamagePerSecond; //  체력 비율이 아닌 고정 데미지
  }

  public IEnumerator ApplyEffect(LivingEntity entity)
  {
    Debug.Log($"🛑 ApplyEffect 호출됨! 대상: {entity.gameObject.name}");

    float elapsedTime = 0f;
    while ( elapsedTime < duration )
    {
      if ( entity == null || entity.IsDead )
      {
        Debug.Log($"⚠ 대상이 NULL이거나 이미 사망! ({entity?.gameObject.name})");
        yield break;
      }

      // ✅ 고정 피해 적용 (비율이 아니라 정해진 값)
      float poisonDamage = fixedDamagePerSecond;

      Debug.Log($"🔥 독 데미지 적용: {poisonDamage} | 대상: {entity.gameObject.name} | 남은 체력: {entity.Hp}");
      entity.OnDamage(poisonDamage, entity.transform.position, Vector3.zero);

      yield return new WaitForSeconds(1f);
      elapsedTime += 1f;
    }

    Debug.Log($"✅ 독 효과 종료: {entity.gameObject.name}");
  }
}
