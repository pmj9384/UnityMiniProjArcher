using UnityEngine;
using UnityEngine.AI;

public class WanderMove : IMoveBehavior
{
  private float wanderRadius = 10f;
  private float wanderInterval = 5f; // 이동 간격 (5초)
  private float stopDuration = 1.5f; // 멈추는 시간 (1.5초)

  private float nextMoveTime = 0f;
  private bool isStopped = false;
  private int maxRetry = 3; // 목표 위치 재탐색 횟수

  public void Move(NavMeshAgent agent, Transform target)
  {
    if ( Time.time >= nextMoveTime )
    {
      if ( !isStopped ) // 🔥 1.5초 멈춘 후 이동 시작
      {
        agent.isStopped = true; // ✅ 이동 멈춤
        isStopped = true;
        nextMoveTime = Time.time + stopDuration;
        Debug.Log($"🛑 {agent.gameObject.name}: 이동 멈춤 (1.5초 대기)");
      }
      else // 🔥 멈춘 후 랜덤 이동 시작
      {
        agent.isStopped = false;
        Vector3 newDestination = GetRandomPoint(agent);

        if ( newDestination != Vector3.zero )
        {
          agent.SetDestination(newDestination);
          Debug.Log($"🚶 {agent.gameObject.name}: 새로운 목적지 {newDestination}");
        }
        else
        {
          Debug.LogWarning($"⚠️ {agent.gameObject.name}: 유효한 목적지 찾기 실패!");
        }

        isStopped = false;
        nextMoveTime = Time.time + wanderInterval; // ✅ 다음 이동까지 5초 대기
      }
    }

    // 🔥 이동 중 벽에 막히면 새로운 방향 찾기
    if ( agent.remainingDistance > 0 && agent.remainingDistance < 1f && agent.velocity.magnitude < 0.1f )
    {
      Debug.Log($"🧱 {agent.gameObject.name}: 벽에 막힘! 새로운 목적지 찾기");
      agent.SetDestination(GetRandomPoint(agent));
    }
  }

  // ✅ 랜덤한 위치 반환 (최대 3번 시도)
  private Vector3 GetRandomPoint(NavMeshAgent agent)
  {
    for ( int i = 0; i < maxRetry; i++ )
    {
      Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
      randomDirection += agent.transform.position;

      if ( NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas) )
      {
        return hit.position;
      }
    }
    return Vector3.zero; // 유효한 위치를 찾지 못한 경우
  }
}
