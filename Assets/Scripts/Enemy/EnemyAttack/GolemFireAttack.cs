using System.Collections;
using UnityEngine;

public class GolemFireAttack : MonoBehaviour, IAttackBehavior
{
  public ParticleSystem fireEffect; // 🔥 불꽃 파티클
  public Transform fireSpawnPoint; // 🔥 불꽃 위치
  public float attackDuration = 2f; // 2초 동안 불을 내뿜음

  private Animator animator;
  private Transform player;
  private bool isAttacking = false; // ✅ 중복 실행 방지

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

    if ( fireEffect == null || fireSpawnPoint == null )
    {
      Debug.LogWarning($"{gameObject.name}: Attack() 호출 실패 - fireEffect 또는 fireSpawnPoint가 null.");
      return;
    }

    StartCoroutine(FireAttack());
  }

  private IEnumerator FireAttack()
  {
    isAttacking = true; // ✅ 공격 시작

    // ✅ 플레이어 방향 고정
    Vector3 attackDirection = ( player.position - transform.position ).normalized;
    attackDirection.y = 0; // Y축 회전 고정
    transform.rotation = Quaternion.LookRotation(attackDirection);

    animator?.SetTrigger("Attack");

    // 🔥 불꽃 파티클 활성화
    fireEffect.Play();
    Debug.Log($"🔥 {gameObject.name}: 불꽃 시작!");

    yield return new WaitForSeconds(attackDuration); // 🔥 2초 동안 불 내뿜기

    // 🔥 불꽃 정지 (한 번만 실행)
    fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    Debug.Log($"✅ {gameObject.name}: 불꽃 중단!");

    yield return new WaitForSeconds(3f);

    isAttacking = false; // ✅ 공격 가능 상태로 변경
  }
}
