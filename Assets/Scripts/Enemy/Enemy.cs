using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
  // 공통 필드
  public LayerMask whatIsTarget; // 타겟 탐지 레이어
  public float findTargetDistance = 10f; // 탐지 거리
  private LivingEntity targetEntity; // 현재 타겟
  private NavMeshAgent agent; // NavMeshAgent 사용
  private Coroutine coUpdatePath; // 이동 경로 업데이트 코루틴

  private Animator animator;
  private AudioSource audioSource;

  public GameObject expPrefab; // 경험치 프리팹
  public int experienceValue = 10; // 드랍 경험치

  public List<ParticleSystem> hitEffects = new List<ParticleSystem>();
  public AudioClip hitSound;
  public AudioClip deathSound;
  public float damage { get; set; }

  private StatusEffectManager statusEffectManager; // 상태 효과
  private GameManager gm;

  // 동적 이동 및 공격 방식
  private IMoveBehavior moveBehavior; // 이동 방식 인터페이스
  private IAttackBehavior attackBehavior; // 공격 방식 인터페이스

  private void Awake()
  {
    statusEffectManager = GetComponent<StatusEffectManager>();
    animator = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
    audioSource = GetComponent<AudioSource>();
    gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
  }

  // 초기화 (테이블 데이터 기반)
  public void Initialize(int monsterID)
  {
    // ✅ MonsterDatabase가 존재하는지 확인
    MonsterDatabase db = FindObjectOfType<MonsterDatabase>();

    // ✅ 몬스터 데이터 로드 (존재 여부 확인)
    MonsterData data = db.GetMonsterData(monsterID);
    agent = GetComponent<NavMeshAgent>();
    // ✅ 몬스터 속성 적용
    maxHp = data.hp;
    damage = data.attack;
    agent.speed = data.speed;
    experienceValue = data.dropExp;

    // ✅ 이동 및 공격 방식 설정
    SetMoveBehavior(data.moveType);
    SetAttackBehavior(monsterID);

    Debug.Log($"✅ 몬스터 초기화 완료: {data.name} (ID: {monsterID})");
  }


  protected override void OnEnable()
  {
    base.OnEnable(); // ✅ 부모 클래스의 OnEnable() 먼저 실행
    coUpdatePath = StartCoroutine(UpdatePath()); // ✅ 이후 Enemy 고유의 OnEnable 실행
  }

  private void OnDisable()
  {
    if ( coUpdatePath != null )
    {
      StopCoroutine(coUpdatePath);
    }
  }

  private void Update()
  {
    // 이동 및 공격 실행
    moveBehavior?.Move(agent, targetEntity?.transform);
    attackBehavior?.Attack();

    // 애니메이션 업데이트
    animator.SetBool("HasTarget", hasTarget);
    animator.SetFloat("Speed", agent.velocity.magnitude);
  }

  private bool hasTarget => targetEntity != null && !targetEntity.IsDead;

  private IEnumerator UpdatePath()
  {
    while ( true )
    {
      if ( !hasTarget )
      {
        agent.isStopped = true;
        targetEntity = FindTarget();
      }
      else
      {
        agent.isStopped = false;
      }

      yield return new WaitForSeconds(0.25f); // 탐지 주기
    }
  }

  public LivingEntity FindTarget()
  {
    Collider[] cols = Physics.OverlapSphere(transform.position, findTargetDistance, whatIsTarget);
    float shortestDistance = Mathf.Infinity;
    LivingEntity nearestTarget = null;

    foreach ( var col in cols )
    {
      var livingEntity = col.GetComponent<LivingEntity>();
      if ( livingEntity != null && !livingEntity.IsDead )
      {
        float distance = Vector3.Distance(transform.position, livingEntity.transform.position);
        if ( distance < shortestDistance )
        {
          shortestDistance = distance;
          nearestTarget = livingEntity;
        }
      }
    }

    return nearestTarget;
  }

  public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
  {
    if ( IsDead ) return;

    base.OnDamage(damage, hitPoint, hitNormal);

    foreach ( var effect in hitEffects )
    {
      if ( effect != null )
      {
        ParticleSystem effectInstance = Instantiate(effect, hitPoint, Quaternion.LookRotation(hitNormal));
        effectInstance.Play();
        Destroy(effectInstance.gameObject, 2f);
      }
    }

    if ( audioSource != null && hitSound != null )
    {
      audioSource.PlayOneShot(hitSound);
    }
  }

  protected override void Die()
  {
    base.Die();
    statusEffectManager.RemoveAllEffects();
    audioSource.PlayOneShot(deathSound);
    animator.SetTrigger("Die");

    if ( coUpdatePath != null )
    {
      StopCoroutine(coUpdatePath);
    }

    foreach ( var col in GetComponents<Collider>() )
    {
      col.enabled = false;
    }

    Rigidbody rb = GetComponent<Rigidbody>();
    if ( rb != null )
    {
      rb.isKinematic = true;
      rb.useGravity = false;
    }

    if ( expPrefab != null )
    {
      GameObject exp = Instantiate(expPrefab, transform.position, Quaternion.identity);
      exp.GetComponent<EnemyExp>().expValue = experienceValue;
      ExperienceManager.Instance.RegisterExpItem(exp);
    }

    GameManager.Instance.DecrementZombieCount();
    gm?.AddScore(100);

    StartCoroutine(DieRoutine());
  }

  private IEnumerator DieRoutine()
  {
    yield return new WaitForSeconds(1f);
    Destroy(gameObject);
  }

  public void ModifySpeed(float newSpeed) => agent.speed = Mathf.Max(0, newSpeed);

  private void SetMoveBehavior(int moveType)
  {
    switch ( moveType )
    {
      case 0: moveBehavior = new ChaseMove(); break;
      case 1: moveBehavior = new WanderMove(); break;
      case 2: moveBehavior = new TowerMove(); break;
      default: Debug.LogWarning("알 수 없는 이동 방식입니다."); break;
    }
  }

  private void SetAttackBehavior(int monsterID)
  {
    switch ( monsterID )
    {
      case 10001: attackBehavior = gameObject.AddComponent<SpiderAttack>(); break;
      case 10112: attackBehavior = gameObject.AddComponent<ReaperAttack>(); break;
      default: Debug.LogWarning("알 수 없는 공격 방식입니다."); break;
    }
  }
  public void AddHitEffect(ParticleSystem effectPrefab)
  {
    if ( !hitEffects.Contains(effectPrefab) ) // 중복 추가 방지
    {
      hitEffects.Add(effectPrefab);
      ParticleSystem effectInstance = Instantiate(effectPrefab, transform.position, Quaternion.identity);
      effectInstance.transform.SetParent(transform); // 적에 붙여줌
      effectInstance.Play();
    }
  }

  public void RemoveHitEffect(ParticleSystem effectPrefab)
  {
    if ( hitEffects.Contains(effectPrefab) )
    {
      hitEffects.Remove(effectPrefab);
      foreach ( Transform child in transform )
      {
        if ( child.GetComponent<ParticleSystem>() == effectPrefab )
        {
          Destroy(child.gameObject);
          break;
        }
      }
    }
  }
  public float GetSpeed()
  {
    return agent != null ? agent.speed : 0f; // ✅ NavMeshAgent 속도 반환
  }

}
