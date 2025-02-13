using UnityEngine;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{
  public float speed = 5f;
  public VirtualJoyStick joystick;
  public LayerMask targetLayer;
  public LayerMask wallLayer;
  public LayerMask groundLayer;
  public float targetRange = 10f;

  private Transform target;
  private Rigidbody rb;
  private float rotationSpeed = 20f;
  private Animator animator;
  private float targetLockTime = 0.5f;
  private float lastTargetUpdateTime;

  private bool isGrounded;
  private float gravityForce = -9.8f; // 🌟 중력 보정값 추가

  private AudioSource footstepAudioSource;
  public AudioClip footstepClip;
  private float footstepDelay = 0.5f;
  private float lastFootstepTime = 0f;
  public AudioMixerGroup sfxMixerGroup;
  private void Awake()
  {
    rb = GetComponent<Rigidbody>();
    animator = GetComponent<Animator>();

    rb.freezeRotation = true;
    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    rb.interpolation = RigidbodyInterpolation.Interpolate;
    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    rb.useGravity = true;

    footstepAudioSource = gameObject.AddComponent<AudioSource>();
    footstepAudioSource.clip = footstepClip;
    footstepAudioSource.loop = false;
    footstepAudioSource.playOnAwake = false;
    footstepAudioSource.outputAudioMixerGroup = sfxMixerGroup;


  }

  private void Update()
  {
    CheckGrounded(); // ✅ 바닥 감지
    HandleTargeting();
  }

  private void FixedUpdate()
  {
    HandleMovementAndRotation();
  }

  private void HandleMovementAndRotation()
  {
    Vector2 joystickInput = joystick.Input;

    if ( joystickInput.sqrMagnitude > 0.01f )
    {
      Vector3 moveInput = new Vector3(joystickInput.x, 0f, joystickInput.y).normalized;
      Vector3 moveDirection = moveInput * speed * Time.fixedDeltaTime;

      moveDirection = AdjustMovementWithWall(rb.position, moveDirection);

      if ( !isGrounded )
      {
        rb.velocity = new Vector3(rb.velocity.x, gravityForce, rb.velocity.z); // 🌟 중력 보정
      }

      rb.MovePosition(rb.position + moveDirection);
      animator.SetBool("Walk", true);
      RotateTowardsDirection(moveInput);

      if ( Time.time > lastFootstepTime + footstepDelay )
      {
        PlayFootstepSound();
        lastFootstepTime = Time.time;
      }
    }
    else
    {
      animator.SetBool("Walk", false);
      StopFootstepSound();
      RotateTowardsTarget();
    }
  }
  private void PlayFootstepSound()
  {
    //  if ( footstepAudioSource != null && footstepClip != null && !footstepAudioSource.isPlaying )
    // {
    footstepAudioSource.Play();
    Debug.Log("Audio!!");
    //}
  }
  private void StopFootstepSound()
  {
    if ( footstepAudioSource != null && footstepAudioSource.isPlaying )
    {
      footstepAudioSource.Stop();
    }
  }
  public void SetSFXMixerGroup(AudioMixerGroup mixerGroup)
  {
    if ( footstepAudioSource != null )
    {
      footstepAudioSource.outputAudioMixerGroup = mixerGroup;
    }
  }


  private void HandleTargeting()
  {
    if ( Time.time - lastTargetUpdateTime < targetLockTime )
    {
      return;
    }

    Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, targetRange, targetLayer);

    float closestDistance = Mathf.Infinity;
    Transform closestEnemy = null;
    Transform visibleEnemy = null;
    float closestVisibleDistance = Mathf.Infinity;

    foreach ( Collider enemy in enemiesInRange )
    {
      float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

      if ( !IsObstructed(enemy.transform) )
      {
        if ( distanceToEnemy < closestVisibleDistance )
        {
          closestVisibleDistance = distanceToEnemy;
          visibleEnemy = enemy.transform;
        }
      }

      if ( distanceToEnemy < closestDistance )
      {
        closestDistance = distanceToEnemy;
        closestEnemy = enemy.transform;
      }
    }

    target = visibleEnemy != null ? visibleEnemy : closestEnemy;
  }

  private bool IsObstructed(Transform enemy)
  {
    Vector3 rayStart = transform.position + Vector3.up * 1.0f;
    Vector3 rayEnd = enemy.position + Vector3.up * 1.0f;
    Vector3 directionToEnemy = rayEnd - rayStart;
    float distanceToEnemy = directionToEnemy.magnitude;

    RaycastHit hit;
    if ( Physics.Raycast(rayStart, directionToEnemy.normalized, out hit, distanceToEnemy) )
    {
      if ( hit.transform != enemy )
      {
        return true;
      }
    }
    return false;
  }

  private void RotateTowardsDirection(Vector3 moveInput)
  {
    if ( moveInput.sqrMagnitude > 0.01f )
    {
      Quaternion targetRotation = Quaternion.LookRotation(moveInput);
      rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
    }
  }

  private void RotateTowardsTarget()
  {
    if ( target != null )
    {
      Vector3 directionToTarget = target.position - rb.position;
      directionToTarget.y = 0f;
      Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
      rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
    }
  }

  public Transform GetTarget()
  {
    return target;
  }

  // ✅ 바닥 감지 Raycast (중복 감지 최소화)
  private void CheckGrounded()
  {
    RaycastHit hit;
    float groundCheckDistance = 1.0f;

    if ( Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, groundCheckDistance, groundLayer) )
    {
      isGrounded = true;
    }
    else
    {
      isGrounded = false;
      HandleFall();
    }
  }

  private void HandleFall()
  {
    if ( transform.position.y < -2f )
    {
      Debug.LogWarning("❗ 플레이어가 떨어짐! 안전한 위치로 복구");
      ResetPlayerPosition();
    }
  }

  private void ResetPlayerPosition()
  {
    Vector3 safePosition = new Vector3(transform.position.x, 1f, transform.position.z);

    RaycastHit hit;
    if ( Physics.Raycast(new Vector3(transform.position.x, 10f, transform.position.z), Vector3.down, out hit, 20f, groundLayer) )
    {
      safePosition = hit.point + Vector3.up * 0.5f;
    }

    rb.velocity = Vector3.zero;
    transform.position = safePosition;
  }

  // ✅ 벽 충돌 감지 및 이동 제한 적용 (벽 앞에서 멈추기)
  private Vector3 AdjustMovementWithWall(Vector3 position, Vector3 moveDirection)
  {
    float checkDistance = 0.6f;
    float characterRadius = 0.25f;

    RaycastHit hit;
    if ( Physics.CapsuleCast(position + Vector3.up * 0.5f, position + Vector3.up * 1.5f, characterRadius, moveDirection.normalized, out hit, checkDistance, wallLayer) )
    {
      float distanceToWall = hit.distance;
      float speedFactor = Mathf.Clamp01(distanceToWall / checkDistance);
      Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
      return slideDirection * speedFactor;
    }

    return moveDirection;
  }


}
