using System.Collections;
using UnityEngine;

public class GolemFireAttack : MonoBehaviour, IAttackBehavior
{
  public ParticleSystem fireEffect; // 🔥 불꽃 파티클
  public Transform fireSpawnPoint; // 🔥 불꽃 위치
  public float attackDuration = 2f; // 2초 동안 불을 내뿜음
  public LineRenderer lineRenderer; // 🔥 조준선 (불꽃 위치 표시)

  private Animator animator;
  private Transform player;
  private bool isAttacking = false; // ✅ 중복 실행 방지
  private Vector3 attackDirection; // 공격 방향 고정

  private void Start()
  {
    animator = GetComponent<Animator>();
    player = GameObject.FindGameObjectWithTag("Player")?.transform;

    if ( player == null )
    {
      Debug.LogError($"{gameObject.name}: Player를 찾을 수 없음!");
    }

    if ( fireSpawnPoint == null )
    {
      Debug.LogError($"{gameObject.name}: fireSpawnPoint가 설정되지 않음!");
    }

    if ( fireEffect == null )
    {
      Debug.LogError($"{gameObject.name}: fireEffect가 설정되지 않음!");
    }
    else
    {
      fireEffect.gameObject.SetActive(true); // ✅ 오브젝트 활성화 후 바로 멈춤
      fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    if ( lineRenderer == null )
    {
      Debug.LogError($"{gameObject.name}: LineRenderer가 설정되지 않음!");
    }
    else
    {
      lineRenderer.enabled = false; // 시작 시 비활성화
    }
  }

  private void Update()
  {
    if ( !isAttacking && player != null )
    {
      // 🔥 불 공격 중이 아닐 때만 플레이어 방향으로 부드럽게 회전
      SmoothRotateToTarget(player.position);
    }
  }

  public void Attack()
  {
    if ( isAttacking ) // ✅ 공격 중이라면 실행 안 함
    {
      Debug.Log($"{gameObject.name}: 이미 공격 중이므로 실행 안 함.");
      return;
    }

    if ( player == null )
    {
      Debug.LogWarning($"{gameObject.name}: Attack() 호출 실패 - Player가 null.");
      return;
    }

    if ( fireEffect == null || fireSpawnPoint == null || lineRenderer == null )
    {
      Debug.LogWarning($"{gameObject.name}: Attack() 호출 실패 - 필요한 컴포넌트가 null.");
      return;
    }

    StartCoroutine(FireAttack());
  }

  private IEnumerator FireAttack()
  {
    isAttacking = true; // ✅ 공격 시작

    // 🔄 **부드러운 회전 후 공격**
    yield return StartCoroutine(SmoothRotateToTarget(player.position));

    // 🔥 1초 동안 조준선 표시
    yield return StartCoroutine(ShowTargetingLine());

    // 🔥 불꽃 공격 실행
    animator?.SetTrigger("Attack");
    fireEffect.Play();
    Debug.Log($"🔥 {gameObject.name}: 불꽃 시작!");

    yield return new WaitForSeconds(attackDuration); // 🔥 2초 동안 불 내뿜기

    // 🔥 불꽃 정지
    fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    Debug.Log($"✅ {gameObject.name}: 불꽃 중단!");

    yield return new WaitForSeconds(1f);

    isAttacking = false; // ✅ 공격 가능 상태로 변경
  }

  // 🔄 **부드러운 회전 함수**
  private IEnumerator SmoothRotateToTarget(Vector3 targetPosition)
  {
    float rotateSpeed = 5f;
    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position);

    while ( Quaternion.Angle(transform.rotation, targetRotation) > 1f ) // 각도가 1도 이상이면 계속 회전
    {
      transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
      yield return null;
    }
    transform.rotation = targetRotation; // 최종적으로 정확한 방향 고정
  }

  // 🔥 **조준선 표시 함수**
  private IEnumerator ShowTargetingLine()
  {
    lineRenderer.enabled = true; // ✅ 조준선 활성화
    lineRenderer.SetPosition(0, fireSpawnPoint.position); // 시작 위치 (불꽃 위치)

    // 🔥 불꽃 파티클의 박스 콜라이더 크기를 가져와서 거리 설정
    float fireRange = 3f; // 기본값
    BoxCollider fireCollider = fireEffect.GetComponent<BoxCollider>();
    if ( fireCollider != null )
    {
      fireRange = fireCollider.bounds.extents.z * 2; // 🔥 불꽃 범위 반영
    }

    // 🔥 플레이어까지가 아니라 불꽃 범위까지만 선을 그림
    Vector3 targetPosition = fireSpawnPoint.position + ( transform.forward * fireRange );
    lineRenderer.SetPosition(1, targetPosition);

    yield return new WaitForSeconds(1f); // 🔥 1초 동안 조준선 표시

    lineRenderer.enabled = false; // ✅ 조준선 비활성화
  }
}
