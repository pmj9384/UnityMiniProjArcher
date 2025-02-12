using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossMove : IMoveBehavior
{
  private bool isDashing = false;
  private bool isCooldown = false;
  private Vector3 dashDirection;
  private float dashCooldown = 1f;
  private int attackStep = 0;

  public void Move(NavMeshAgent agent, Transform target)
  {
    if ( target == null || isDashing || isCooldown ) return;

    dashDirection = ( target.position - agent.transform.position ).normalized;
    dashDirection.y = 0;

    if ( agent.remainingDistance <= agent.stoppingDistance && agent.velocity.magnitude < 0.1f )
    {
      Debug.Log($"{agent.gameObject.name}: 돌진 준비 시작!");
      agent.isStopped = true;
      isDashing = true;
      isCooldown = true;

      MonoBehaviour mono = agent.GetComponent<MonoBehaviour>();
      if ( mono != null )
      {
        mono.StartCoroutine(PrepareAndDash(agent, target));
      }
    }
  }

  private IEnumerator PrepareAndDash(NavMeshAgent agent, Transform target)
  {
    Animator animator = agent.GetComponent<Animator>(); // 🔥 agent에서 Animator 가져오기

    if ( animator != null )
    {
      animator.SetTrigger("PrepareD");
    }

    float prepTime = 1.0f;
    float elapsedTime = 0f;
    while ( elapsedTime < prepTime )
    {
      RotateTowardsTarget(agent, target, 7f);
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    yield return Dash(agent, target);
    yield return new WaitForSeconds(dashCooldown);
    isCooldown = false;
    agent.isStopped = false;
  }

  private IEnumerator Dash(NavMeshAgent agent, Transform target)
  {
    Animator animator = agent.GetComponent<Animator>(); // 🔥 agent에서 Animator 가져오기
    if ( animator != null )
    {
      animator.SetTrigger("Dash");
    }

    float dashTime = 2f;
    float dashSpeed = agent.speed * 5f;
    float stopDistance = 1.0f;
    float elapsedTime = 0f;

    while ( elapsedTime < dashTime )
    {
      Vector3 nextPosition = agent.transform.position + dashDirection * dashSpeed * Time.deltaTime;

      NavMeshHit hit;
      if ( NavMesh.Raycast(agent.transform.position, nextPosition, out hit, NavMesh.AllAreas) )
      {
        float distanceToWall = Vector3.Distance(agent.transform.position, hit.position);
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

    float postDashRotationTime = 1.0f;
    elapsedTime = 0f;
    while ( elapsedTime < postDashRotationTime )
    {
      RotateTowardsTarget(agent, target, 5f);
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    if ( animator != null )
    {
      animator.SetTrigger("Attack");
    }

    yield return TriggerAttack(agent.gameObject);
  }

  private void RotateTowardsTarget(NavMeshAgent agent, Transform target, float speed)
  {
    if ( target == null ) return;

    Vector3 direction = ( target.position - agent.transform.position ).normalized;
    direction.y = 0;
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
        yield return attackComponent.StartCoroutine(attackComponent.ShootProjectile());
        Debug.Log($"{boss.name}: 콩알탄 공격!");
        attackStep = 1;
      }
      else
      {
        yield return attackComponent.StartCoroutine(attackComponent.SummonSpider());
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
