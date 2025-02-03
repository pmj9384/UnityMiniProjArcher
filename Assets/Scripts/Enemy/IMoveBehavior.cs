using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public interface IMoveBehavior
{
  void Move(NavMeshAgent agent, Transform target);
}
