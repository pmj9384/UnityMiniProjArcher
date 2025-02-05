using System.Collections;
using UnityEngine;

public class GolemFireAttack : MonoBehaviour, IAttackBehavior
{
  public GameObject firePrefab; // 🔥 불꽃 파티클 프리팹
  public Transform fireSpawnPoint; // 불꽃이 나가는 위치
  public float attackDuration = 2f; // 2초 동안 불을 내뿜음
  private bool isAttacking = false;
  private GameObject activeFireEffect; // 🔥 불꽃 파티클 오브젝트
  private Vector3 attackDirection; // 🔥 공격 방향 고정

  private Animator animator;
  private Enemy enemy;
  private Transform player;

  private void Awake()
  {
    enemy = GetComponent<Enemy>();
    if ( enemy == null )
    {
      Debug.LogError($"{gameObject.name}: Enemy 컴포넌트를 찾을 수 없음!");
    }
  }

  private void Start()
  {
    animator = GetComponent<Animator>();

    // ✅ 플레이어 찾기
    player = GameObject.FindGameObjectWithTag("Player")?.transform;
    if ( player == null )
    {
      Debug.LogWarning($"{gameObject.name}: Start()에서 Player를 찾을 수 없음. Update에서 다시 찾음.");
    }

    if ( fireSpawnPoint == null )
    {
      Debug.LogError($"{gameObject.name}: fireSpawnPoint가 설정되지 않음!");
    }

    if ( firePrefab == null )
    {
      Debug.LogError($"{gameObject.name}: firePrefab이 설정되지 않음!");
    }
  }

  private void Update()
  {
    // ✅ Update에서 지속적으로 Player 찾기
    if ( player == null )
    {
      GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
      if ( playerObject != null )
      {
        player = playerObject.transform;
        Debug.Log($"✅ {gameObject.name}: Update()에서 Player를 찾음!");
      }
    }

    if ( player == null || enemy == null )
    {
      return; // 플레이어나 Enemy가 없으면 Update 종료
    }

    if ( !isAttacking )
    {
      Attack();
    }
  }

  public void Attack()
  {
    if ( isAttacking ) return; // 이미 공격 중이면 실행 안 함

    if ( player == null )
    {
      Debug.LogWarning($"{gameObject.name}: Attack() 호출 실패 - Player가 null.");
      return;
    }

    if ( enemy == null )
    {
      Debug.LogWarning($"{gameObject.name}: Attack() 호출 실패 - Enemy가 null.");
      return;
    }

    if ( firePrefab == null || fireSpawnPoint == null )
    {
      Debug.LogWarning($"{gameObject.name}: Attack() 호출 실패 - firePrefab 또는 fireSpawnPoint가 null.");
      return;
    }

    StartCoroutine(FireAttack());
  }

  private IEnumerator FireAttack()
  {
    isAttacking = true;
    enemy.StartAttack(); // ✅ 이동 멈춤

    // ✅ 플레이어 방향 고정
    attackDirection = ( player.position - transform.position ).normalized;
    attackDirection.y = 0; // Y축 회전 고정
    transform.rotation = Quaternion.LookRotation(attackDirection);

    animator?.SetTrigger("Attack");

    // 🔥 불꽃 파티클 생성
    activeFireEffect = Instantiate(firePrefab, fireSpawnPoint.position, Quaternion.identity);
    if ( activeFireEffect != null )
    {
      activeFireEffect.transform.SetParent(fireSpawnPoint);
    }
    else
    {
      Debug.LogError($"{gameObject.name}: FireEffect 생성 실패!");
    }

    yield return new WaitForSeconds(attackDuration); // 🔥 2초 동안 불 내뿜기

    isAttacking = false;
    if ( activeFireEffect != null )
    {
      Destroy(activeFireEffect); // 불꽃 파티클 제거
    }

    yield return new WaitForSeconds(0.5f); // 🔥 공격 후 0.5초 대기

    enemy.StopAttack(); // ✅ 이동 가능하도록 설정
  }
}
