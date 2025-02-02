using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
  private Dictionary<Type, Coroutine> activeEffects = new Dictionary<Type, Coroutine>();
  private LivingEntity entity;

  private void Awake()
  {
    entity = GetComponent<LivingEntity>();
  }

  public void ApplyEffect(IStatusEffect effect)
  {
    if ( entity == null || entity.IsDead ) return;

    Type effectType = effect.GetType();

    // ✅ 이미 적용된 효과인지 확인 후 중복 방지
    if ( activeEffects.ContainsKey(effectType) ) return;

    Coroutine effectCoroutine = StartCoroutine(effect.ApplyEffect(entity));
    activeEffects[effectType] = effectCoroutine;
  }

  public void RegisterEffect(IStatusEffect effect)
  {
    Type effectType = effect.GetType();
    if ( !activeEffects.ContainsKey(effectType) )
    {
      activeEffects[effectType] = null; // 효과 등록 (코루틴 실행 전)
    }
  }

  public bool HasEffect<T>() where T : IStatusEffect
  {
    return activeEffects.ContainsKey(typeof(T));
  }

  public void RemoveEffect<T>() where T : IStatusEffect
  {
    Type effectType = typeof(T);
    if ( activeEffects.ContainsKey(effectType) )
    {
      if ( activeEffects[effectType] != null )
      {
        StopCoroutine(activeEffects[effectType]);
      }
      activeEffects.Remove(effectType);
    }
  }

  public void RemoveAllEffects()
  {
    foreach ( var effect in activeEffects.Values )
    {
      if ( effect != null )
      {
        StopCoroutine(effect);
      }
    }
    activeEffects.Clear();
  }
}
