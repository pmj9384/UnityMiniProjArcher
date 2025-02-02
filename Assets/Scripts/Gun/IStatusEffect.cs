using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IStatusEffect
{
  //void ApplyEffect(Enemy enemy); // 적에게 상태 효과 적용
  IEnumerator ApplyEffect(LivingEntity entity);
}
