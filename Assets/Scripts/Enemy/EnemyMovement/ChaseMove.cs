using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChaseMove : IMoveBehavior
{
  public void Move(UnityEngine.AI.NavMeshAgent agent, Transform target)
  {
    if ( target != null )
    {
      agent.SetDestination(target.position);
    }
  }
}
