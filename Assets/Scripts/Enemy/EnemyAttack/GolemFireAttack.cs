using System.Collections;
using UnityEngine;

public class GolemFireAttack : MonoBehaviour, IAttackBehavior
{
  public ParticleSystem fireEffect; // 🔥 불꽃 파티클
  public Transform fireSpawnPoint; // 🔥 불꽃 위치
  public float attackDuration = 2f; // 2초 동안 불을 내뿜음
  public float fireDamage = 5f; // 🔥 지속 데미지 (초당)
  public float damageInterval = 0.5f; // 🔥 데미지 적용 간격
  public LineRenderer lineRenderer; // 🔥 조준선 (불꽃 위치 표시)

  private Animator animator;
  private Transform player;
  private bool isAttacking = false;
  private Vector3 attackDirection;
  private bool isDealingDamage = false; // ✅ 불꽃이 활성화 중인지 확인

  private void Start()
  {
    animator = GetComponent<Animator>();
    player = GameObject.FindGameObjectWithTag("Player")?.transform;

    if ( fireEffect == null )
    {
      Debug.LogError($"{gameObject.name}: fireEffect가 설정되지 않음!");
    }
    else
    {
      fireEffect.gameObject.SetActive(true);
      fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    if ( lineRenderer == null )
    {
      Debug.LogError($"{gameObject.name}: LineRenderer가 설정되지 않음!");
    }
    else
    {
      lineRenderer.enabled = false;
    }
  }

  private void Update()
  {
    if ( !isAttacking && player != null )
    {
      SmoothRotateToTarget(player.position);
    }
  }

  public void Attack()
  {
    if ( isAttacking ) return;
    if ( player == null ) return;

    StartCoroutine(FireAttack());
  }

  private IEnumerator FireAttack()
  {
    isAttacking = true;

    yield return StartCoroutine(SmoothRotateToTarget(player.position));
    yield return StartCoroutine(ShowTargetingLine());

    // 🔥 불꽃 켜기 + 데미지 적용 시작
    animator?.SetTrigger("Attack");
    fireEffect.Play();
    isDealingDamage = true; // ✅ 데미지 활성화
    StartCoroutine(ApplyFireDamage()); // ✅ 지속 데미지 시작

    Debug.Log($"🔥 {gameObject.name}: 불꽃 시작!");
    yield return new WaitForSeconds(attackDuration);

    // 🔥 불꽃 끄기 + 데미지 중단
    fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    isDealingDamage = false; // ✅ 데미지 중단
    Debug.Log($"✅ {gameObject.name}: 불꽃 중단!");

    yield return new WaitForSeconds(1f);
    isAttacking = false;
  }

  // 🔥 **지속적인 데미지 적용 (불꽃이 켜진 동안)**
  private IEnumerator ApplyFireDamage()
  {
    while ( isDealingDamage )
    {
      Collider[] hitColliders = Physics.OverlapSphere(fireSpawnPoint.position, 3f); // 🔥 불꽃 범위 내 적 감지
      foreach ( Collider hit in hitColliders )
      {
        if ( hit.CompareTag("Player") )
        {
          PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
          if ( playerHealth != null )
          {
            playerHealth.OnDamage(fireDamage, hit.transform.position, Vector3.zero);
            Debug.Log($"🔥 불꽃 지속 데미지 적용: {fireDamage}");
          }
        }
      }
      yield return new WaitForSeconds(damageInterval); // 🔥 0.5초마다 데미지 적용
    }
  }

  private IEnumerator SmoothRotateToTarget(Vector3 targetPosition)
  {
    float rotateSpeed = 5f;
    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position);

    while ( Quaternion.Angle(transform.rotation, targetRotation) > 1f )
    {
      transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
      yield return null;
    }
    transform.rotation = targetRotation;
  }

  private IEnumerator ShowTargetingLine()
  {
    lineRenderer.enabled = true;
    lineRenderer.SetPosition(0, fireSpawnPoint.position);

    float fireRange = 3f;
    Vector3 targetPosition = fireSpawnPoint.position + ( transform.forward * fireRange );
    lineRenderer.SetPosition(1, targetPosition);

    yield return new WaitForSeconds(1f);
    lineRenderer.enabled = false;
  }
}
