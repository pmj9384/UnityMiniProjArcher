using UnityEngine;
using UnityEngine.AI;

public class ChaseDash : IMoveBehavior
{
  public float chaseSpeed = 2.0f;   // 기본 추적 속도
  public float dashSpeed = 5.0f;    // 돌진 속도
  public float dashDuration = 1f; // 돌진 지속 시간
  public float cooldown = 2.0f;     // 돌진 후 대기 시간
  public float prepTime = 1.0f;     // 🔥 돌진 준비 시간 (1초로 증가)

  private Transform player;
  private bool isPreparingDash = false;
  private bool isDashing = false;
  private float nextDashTime = 0f;
  private float dashStartTime = 0f;
  private float prepStartTime = 0f;

  public void Move(NavMeshAgent agent, Transform target)
  {
    if ( target == null ) return;
    player = target;

    float currentTime = Time.time;

    if ( isPreparingDash ) // 🔥 돌진 전 1초 동안 멈추고 방향 회전
    {
      if ( currentTime >= prepStartTime + prepTime )
      {
        isPreparingDash = false;
        isDashing = true;
        dashStartTime = currentTime;
        agent.isStopped = true; // ✅ SetDestination 비활성화
        Debug.Log($"🚀 {agent.gameObject.name}: 돌진 시작! (속도 {dashSpeed})");
      }
      else
      {
        agent.isStopped = true; // ✅ 이동 멈춤
        RotateTowardsPlayer(agent); // 🔥 돌진 준비 중 플레이어 방향으로 회전
        Debug.Log($"🛑 {agent.gameObject.name}: 돌진 준비 중 (속도 0), 플레이어 방향으로 회전");
      }
    }
    else if ( isDashing ) // 🔥 돌진 중
    {
      if ( currentTime >= dashStartTime + dashDuration )
      {
        agent.speed = chaseSpeed;  // ✅ 돌진 후 다시 기본 속도
        agent.isStopped = false;   // ✅ 다시 이동 활성화
        isDashing = false;
        nextDashTime = currentTime + cooldown;  // ✅ 다음 돌진 시간 설정
        Debug.Log($"✅ {agent.gameObject.name}: 돌진 종료, 다음 돌진까지 {cooldown}초");
      }
      else
      {
        Vector3 dashDirection = ( player.position - agent.transform.position ).normalized;
        agent.Move(dashDirection * dashSpeed * Time.deltaTime); // ✅ 직접 이동 처리
      }
    }
    else if ( currentTime >= nextDashTime ) // 🔥 돌진 준비 시작
    {
      agent.isStopped = true;  // ✅ 이동 멈춤
      isPreparingDash = true;
      prepStartTime = currentTime;
      Debug.Log($"⚡ {agent.gameObject.name}: 돌진 준비 시작 (1초 후 돌진)");
    }
    else // 🔥 기본 추적 상태 (SetDestination 활성화)
    {
      agent.isStopped = false;
      agent.speed = chaseSpeed;
      agent.SetDestination(player.position);
    }
  }

  // 🔥 플레이어 방향으로 회전 (부드럽게 회전)
  private void RotateTowardsPlayer(NavMeshAgent agent)
  {
    if ( player == null ) return;

    Vector3 direction = ( player.position - agent.transform.position ).normalized;
    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Y축 회전만 반영
    agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 5f); // 🔥 부드러운 회전
  }
}
