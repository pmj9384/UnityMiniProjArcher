using UnityEngine;
using UnityEngine.AI;

public class BossMove : IMoveBehavior
{
  public void Move(NavMeshAgent agent, Transform target)
  {
    if ( target == null ) return;

    Vector3 direction = ( target.position - agent.transform.position ).normalized;
    direction.y = 0; // Y축 고정

    // 돌진 방향 설정
    agent.velocity = direction * agent.speed;

    // 벽에 닿으면 멈춤
    if ( agent.remainingDistance <= 0.5f || agent.pathStatus == NavMeshPathStatus.PathComplete )
    {
      agent.isStopped = true;
      agent.velocity = Vector3.zero;
      Debug.Log($"🛑 {agent.gameObject.name}: 벽에 부딪혀 멈춤!");
    }
  }
}
