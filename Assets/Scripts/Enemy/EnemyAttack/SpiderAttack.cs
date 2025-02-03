using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAttack : MonoBehaviour, IAttackBehavior
{
  public float damage = 10f;
  public float attackRange = 2f;

  public void Attack()
  {
    // 근접 공격 로직
  }
}
