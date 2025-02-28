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
  private float currentHp;
  private StatusEffectManager statusEffectManager; // 상태 효과
  private GameManager gm;
  public GameObject hpBarPrefab; // ✅ 체력바 프리팹 (Inspector에서 할당)
  private MonsterHpBar hpBar;

  // 동적 이동 및 공격 방식
  private IMoveBehavior moveBehavior; // 이동 방식 인터페이스

  private bool isAttacking = false;
  public bool IsAttacking
  {
    get { return isAttacking; }
  }
  public IAttackBehavior attackBehavior; // 공격 방식 인터페이스

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
    currentHp = maxHp;
    damage = data.attack;
    agent.speed = data.speed;
    experienceValue = data.dropExp;

    // ✅ 이동 및 공격 방식 설정
    SetMoveBehavior(data.moveType);
    SetAttackBehavior(monsterID);

    Debug.Log($"✅ 몬스터 초기화 완료: {data.name} (ID: {monsterID})");
  }
  private void Start()
  {
    // ✅ 프리팹에 있는 MonsterHpBar 찾아서 사용 (새로 생성 X)
    hpBar = GetComponentInChildren<MonsterHpBar>();

    if ( hpBar != null )
    {
      hpBar.monster = transform; // ✅ 몬스터와 연결
      hpBar.UpdateHealthBar(currentHp, maxHp);
    }
    else
    {
      Debug.LogWarning($"⚠️ {gameObject.name}: 체력바가 프리팹에 포함되지 않음!");
    }
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

    if ( !isAttacking ) // ✅ 공격 중이 아닐 때만 이동하도록 수정
    {
      moveBehavior?.Move(agent, targetEntity?.transform);
    }

    attackBehavior?.Attack(); // ✅ 공격 호출은 한 번만 실행

    // ✅ 애니메이션 상태 업데이트
    animator.SetBool("HasTarget", hasTarget);
    animator.SetFloat("Speed", agent.velocity.magnitude);

  }

  public void StartAttack()
  {
    isAttacking = true;
    agent.isStopped = true;
    agent.ResetPath(); // ✅ 현재 이동 경로 제거
    agent.velocity = Vector3.zero; // ✅ 이동 즉시 멈춤
  }

  public void StopAttack()
  {
    isAttacking = false;
    agent.isStopped = false; // ✅ 공격이 끝나면 다시 이동 가능
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

    foreach ( var effect in hitEffects )
    {
      if ( effect != null )
      {
        ParticleSystem effectInstance = Instantiate(effect, hitPoint, Quaternion.LookRotation(hitNormal));
        effectInstance.Play();
        Destroy(effectInstance.gameObject, 2f);
      }
    }
    
    currentHp -= damage;
    if ( currentHp < 0 ) currentHp = 0;
    base.OnDamage(damage, hitPoint, hitNormal);
    if ( hpBar != null )
    {
      hpBar.UpdateHealthBar(currentHp, maxHp);
    }

    UpdateHealthBar();


    if ( audioSource != null && hitSound != null )
    {
      audioSource.PlayOneShot(hitSound);
    }
  }
  protected override void Die()
  {
    base.Die();
    if ( hpBar != null )
    {
      Destroy(hpBar.gameObject);
    }
    // ✅ 상태 효과 제거

    isAttacking = false;  // 공격 상태 해제
    if ( attackBehavior != null )
    {
      attackBehavior = null; // 🔥 공격 비활성화
    }

    // ✅ NavMeshAgent 멈춤
    if ( agent != null )
    {
      agent.isStopped = true; // 이동 멈춤
      agent.velocity = Vector3.zero; // 즉시 정지
    }

    // ✅ 모든 Collider 비활성화
    foreach ( var col in GetComponents<Collider>() )
    {
      col.enabled = false;
    }

    // ✅ Rigidbody 설정
    Rigidbody rb = GetComponent<Rigidbody>();
    if ( rb != null )
    {
      rb.isKinematic = true;
      rb.useGravity = false;
    }

    // ✅ 사운드 재생
    if ( audioSource != null && deathSound != null )
    {
      audioSource.PlayOneShot(deathSound);
    }

    // ✅ 죽는 애니메이션 트리거
    animator.SetTrigger("Die");
    statusEffectManager.RemoveAllEffects();
    // ✅ 경험치 아이템 생성
    if ( expPrefab != null )
    {
      GameObject exp = Instantiate(expPrefab, transform.position, Quaternion.identity);
      exp.GetComponent<EnemyExp>().expValue = experienceValue;
      ExperienceManager.Instance.RegisterExpItem(exp);
    }

    // ✅ 적의 경로 업데이트 중지
    if ( coUpdatePath != null )
    {
      StopCoroutine(coUpdatePath);
    }

    // ✅ 좀비 수 감소
    GameManager.Instance.DecrementZombieCount();

    // ✅ 1초 후 오브젝트 삭제
    StartCoroutine(DieRoutine());
  }


  private IEnumerator DieRoutine()
  {
    yield return new WaitForSeconds(1f);
    Destroy(gameObject);
  }

  public void ModifySpeed(float newSpeed) => agent.speed = Mathf.Max(0, newSpeed);

  public IMoveBehavior GetMoveBehavior()
  {
    return moveBehavior;
  }

  private void SetMoveBehavior(int moveType)
  {
    Debug.Log($"🛠️ {gameObject.name}의 moveType: {moveType}");

    switch ( moveType )
    {
      case 0:
        moveBehavior = new ChaseMove();
        Debug.Log($"✅ {gameObject.name}: ChaseMove 적용됨");
        break;
      case 1:
        moveBehavior = new WanderMove();
        Debug.Log($"✅ {gameObject.name}: WanderMove 적용됨");
        break;
      case 2:
        moveBehavior = new TowerMove();
        Debug.Log($"✅ {gameObject.name}: TowerMove 적용됨");
        break;
      case 3:
        moveBehavior = new ChaseDash();
        Debug.Log($"🚀 {gameObject.name}: ChaseDash 적용됨!");
        break;

      case 4:
        moveBehavior = new BossMove();
        Debug.Log($"🚀 {gameObject.name}: BossMove 적용됨!");
        break;

      default:
        Debug.LogWarning($"❌ {gameObject.name}: 알 수 없는 이동 방식 ({moveType})");
        break;
    }
  }


  private void SetAttackBehavior(int monsterID)
  {
    // ✅ 기존 컴포넌트 찾기 (Inspector에서 설정한 것 유지)
    attackBehavior = GetComponent<IAttackBehavior>();

    switch ( monsterID )
    {
      case 10001:
        if ( attackBehavior == null ) attackBehavior = gameObject.AddComponent<SpiderAttack>();
        break;
      case 10003:
        if ( attackBehavior == null ) attackBehavior = gameObject.AddComponent<SpiderAttack>();
        break;
      case 10112:
        if ( attackBehavior == null ) attackBehavior = gameObject.AddComponent<ReaperAttack>();
        break;
      case 10014:
        if ( attackBehavior == null ) attackBehavior = gameObject.AddComponent<MushroomAttack>();
        break;
      case 10115:
        if ( attackBehavior == null ) attackBehavior = gameObject.AddComponent<GolemEarthAttack>();
        break;
      case 10106:
        if ( attackBehavior == null ) attackBehavior = gameObject.AddComponent<GolemFireAttack>();
        break;
      case 11007:
        if ( attackBehavior == null ) attackBehavior = gameObject.AddComponent<QueenSpiderAttack>();
        break;
      default:
        Debug.LogWarning($"❌ {gameObject.name}: 알 수 없는 공격 방식 (ID: {monsterID})");
        break;
    }

    if ( attackBehavior == null )
    {
      Debug.LogError($"❌ {gameObject.name}: 공격 방식이 설정되지 않음!");
    }
    else
    {
      Debug.Log($"✅ {gameObject.name}: 공격 방식 설정됨 → {attackBehavior.GetType().Name}");
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

  // 🔥 추가: 코루틴 실행을 위한 인터페이스
  public void RunCoroutine(IEnumerator coroutine)
  {
    StartCoroutine(coroutine);
  }
  public float GetCurrentHealth()
  {
    return currentHp; // 🔥 현재 체력 반환
  }

  public float GetMaxHealth()
  {
    return maxHp; // 🔥 최대 체력 반환
  }
  private void UpdateHealthBar()
  {
    BossHealthBar bossHealthBar = FindObjectOfType<BossHealthBar>(); // ✅ 체력바 찾기
    if ( bossHealthBar != null )
    {
      bossHealthBar.UpdateHealth(currentHp); // ✅ 체력 업데이트 호출
    }
  }



}