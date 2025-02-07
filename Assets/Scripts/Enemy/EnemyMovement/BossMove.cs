using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossMove : IMoveBehavior
{
  private bool isDashing = false;
  private bool isCooldown = false; // 🛑 5초 대기 상태 플래그 (돌진 + 공격 포함)
  private Vector3 dashDirection;
  private float dashCooldown = 1f; // 🛑 5초 멈춤 (돌진 후 + 공격 후)
  private int attackStep = 0; // 🛑 공격 순서 (0: 콩알탄, 1: 거미 소환)

  public void Move(NavMeshAgent agent, Transform target)
  {
    if ( target == null || isDashing || isCooldown ) return; // 돌진 중이거나 쿨다운이면 실행 안함

    dashDirection = ( target.position - agent.transform.position ).normalized;
    dashDirection.y = 0;

    if ( agent.remainingDistance <= agent.stoppingDistance && agent.velocity.magnitude < 0.1f )
    {
      Debug.Log($"{agent.gameObject.name}: 돌진 준비 시작 (방향 회전)!");
      agent.isStopped = true;
      isDashing = true;
      isCooldown = true; // 🔥 돌진 시작과 동시에 쿨다운 유지

      MonoBehaviour mono = agent.GetComponent<MonoBehaviour>();
      if ( mono != null )
      {
        mono.StartCoroutine(PrepareAndDash(agent, target));
      }
    }
  }

  private IEnumerator PrepareAndDash(NavMeshAgent agent, Transform target)
  {
    float prepTime = 1.0f; // 🔥 돌진 전에 1초 동안 방향 회전
    float rotationSpeed = 7f; // 회전 속도

    // 🔥 1초 동안 플레이어 방향으로 회전
    float elapsedTime = 0f;
    while ( elapsedTime < prepTime )
    {
      RotateTowardsTarget(agent, target, rotationSpeed);
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    // 🔥 회전 후 돌진 실행
    yield return Dash(agent, target);

    // 🔥 공격 후 5초 대기 후 다음 돌진 가능
    yield return new WaitForSeconds(dashCooldown);
    isCooldown = false;
    agent.isStopped = false;
  }

  private IEnumerator Dash(NavMeshAgent agent, Transform target)
  {
    float dashTime = 2f;
    float dashSpeed = agent.speed * 5f;
    float stopDistance = 1.0f; // 🔥 벽과 이 거리 이내로 가까워지면 멈추도록 설정

    float elapsedTime = 0f;

    while ( elapsedTime < dashTime )
    {
      Vector3 nextPosition = agent.transform.position + dashDirection * dashSpeed * Time.deltaTime;

      // 🔥 NavMesh.Raycast()를 사용하여 다음 위치가 벽인지 체크
      NavMeshHit hit;
      if ( NavMesh.Raycast(agent.transform.position, nextPosition, out hit, NavMesh.AllAreas) )
      {
        float distanceToWall = Vector3.Distance(agent.transform.position, hit.position);

        // 벽과 일정 거리(1m 이하)라면 돌진 중단
        if ( distanceToWall <= stopDistance )
        {
          Debug.Log($"{agent.gameObject.name}: 벽과 {distanceToWall}m 남음! 돌진 중단!");
          break;
        }
      }

      agent.Move(dashDirection * dashSpeed * Time.deltaTime);
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    Debug.Log($"{agent.gameObject.name}: 돌진 종료!");

    agent.velocity = Vector3.zero;
    agent.isStopped = true;
    isDashing = false;

    // 🔥 돌진 후 다시 플레이어 방향으로 회전
    float postDashRotationTime = 1.0f;
    elapsedTime = 0f;
    while ( elapsedTime < postDashRotationTime )
    {
      RotateTowardsTarget(agent, target, 5f);
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    // 🔥 돌진 후 공격 실행
    yield return TriggerAttack(agent.gameObject);
  }

  private void RotateTowardsTarget(NavMeshAgent agent, Transform target, float speed)
  {
    if ( target == null ) return;

    Vector3 direction = ( target.position - agent.transform.position ).normalized;
    direction.y = 0; // Y축 고정
    Quaternion lookRotation = Quaternion.LookRotation(direction);

    agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * speed);
  }

  private IEnumerator TriggerAttack(GameObject boss)
  {
    QueenSpiderAttack attackComponent = boss.GetComponent<QueenSpiderAttack>();

    if ( attackComponent != null )
    {
      if ( attackStep == 0 )
      {
        yield return attackComponent.StartCoroutine(attackComponent.ShootProjectile()); // 🔥 콩알탄 공격 실행
        Debug.Log($"{boss.name}: 콩알탄 공격!");
        attackStep = 1;
      }
      else
      {
        yield return attackComponent.StartCoroutine(attackComponent.SummonSpider()); // 🔥 거미 소환 공격 실행
        Debug.Log($"{boss.name}: 거미 소환!");
        attackStep = 0;
      }
    }
  }
  public bool IsDashing()
  {
    return isDashing;
  }

}
