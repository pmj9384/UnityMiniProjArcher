using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
  public LayerMask whatIsTarget;
  public float findTargetDistance = 10f;
  private LivingEntity targetEntity;
  private NavMeshAgent agent;

  private Animator zombieAnimator;
  private AudioSource audioSource;
  private Coroutine coUpdatePath;

  public List<ParticleSystem> hitEffects = new List<ParticleSystem>();
  public AudioClip hitSound;
  public AudioClip deathSound;

  public GameObject expPrefab;
  public int experienceValue = 10;
  public float damage = 20f;
  public float timeBetAttack = 0.5f;
  private float lastAttackTime;

  private StatusEffectManager statusEffectManager;

  private GameManager gm;

  private void Awake()
  {
    statusEffectManager = GetComponent<StatusEffectManager>();
    zombieAnimator = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
    audioSource = GetComponent<AudioSource>();
    gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    coUpdatePath = StartCoroutine(UpdatePath());
  }

  protected void OnDisable()
  {
    if ( coUpdatePath != null )
    {
      StopCoroutine(coUpdatePath);
      coUpdatePath = null;
    }
  }

  private void Update()
  {
    zombieAnimator.SetBool("HasTarget", hasTarget);
    zombieAnimator.SetFloat("Speed", agent.velocity.magnitude);
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
        agent.SetDestination(targetEntity.transform.position);
      }
      yield return new WaitForSeconds(0.25f);
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
    if ( IsDead ) return; // ✅ 이미 사망한 경우 실행하지 않음

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
    zombieAnimator.SetTrigger("Die");
    agent.isStopped = true;

    if ( coUpdatePath != null )
    {
      StopCoroutine(coUpdatePath);
      coUpdatePath = null;
    }

    foreach ( var col in GetComponents<Collider>() )
    {
      col.enabled = false;
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
    yield return new WaitForSeconds(5f);
    //gameObject.SetActive(false);
    Destroy(gameObject);
  }

  public void ModifySpeed(float newSpeed) => agent.speed = Mathf.Max(0, newSpeed);
  public float GetSpeed() => agent.speed;

  public void ApplyStatusEffect(IStatusEffect effect)
  {
    statusEffectManager.ApplyEffect(effect);

  }

  public void AddHitEffect(ParticleSystem effectPrefab)
  {
    if ( !hitEffects.Contains(effectPrefab) ) // 중복 추가 방지
    {
      hitEffects.Add(effectPrefab);
    }
  }

  public void RemoveHitEffect(ParticleSystem effectPrefab)
  {
    if ( hitEffects.Contains(effectPrefab) )
    {
      hitEffects.Remove(effectPrefab);
    }
  }


}

