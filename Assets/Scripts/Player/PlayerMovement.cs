using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float speed = 5f;
  public VirtualJoyStick joystick;
  public LayerMask targetLayer;
  public LayerMask wallLayer; // 🔥 벽 감지 레이어 추가
  public float targetRange = 10f;

  private Transform target;
  private Rigidbody rb;
  private float rotationSpeed = 20f;
  private Animator animator;

  private void Awake()
  {
    rb = GetComponent<Rigidbody>();
    animator = GetComponent<Animator>();
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    // 🔥 Rigidbody 설정 (떨림 방지 + 부드러운 이동)
    rb.freezeRotation = true; // 물리 충돌로 인한 회전 방지
    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 빠른 충돌 감지
    rb.interpolation = RigidbodyInterpolation.Interpolate; // 🔥 부드러운 물리 이동
    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 🔥 불필요한 축 회전 방지
  }

  private void Update()
  {
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

      // 🔥 벽 감지 및 이동 보정
      moveDirection = AdjustMovementWithWall(rb.position, moveDirection);

      rb.MovePosition(rb.position + moveDirection); // 🔥 이동 적용

      animator.SetBool("Walk", true);
      RotateTowardsDirection(moveInput);
    }
    else
    {
      animator.SetBool("Walk", false);
      RotateTowardsTarget();
    }
  }

  private void HandleTargeting()
  {
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
    Vector3 directionToEnemy = enemy.position - transform.position;
    RaycastHit hit;

    if ( Physics.Raycast(transform.position, directionToEnemy, out hit, targetRange) )
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

  // 🔥 벽 충돌 감지 및 이동 제한 적용 (벽 앞에서 멈추기)
  private Vector3 AdjustMovementWithWall(Vector3 position, Vector3 moveDirection)
  {
    float checkDistance = 0.6f; // 🔥 벽 감지 거리 (적절히 조정 가능)

    RaycastHit hit;
    if ( Physics.Raycast(position, moveDirection.normalized, out hit, checkDistance, wallLayer) )
    {
      float distanceToWall = hit.distance;

      // 🔥 너무 가까우면 이동을 멈춤
      if ( distanceToWall < 0.4f )
      {
        return Vector3.zero;
      }

      // 🔥 벽을 따라 미끄러지도록 조정
      Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
      return slideDirection * Mathf.Clamp01(distanceToWall / checkDistance); // 거리 비율에 따라 감속
    }

    return moveDirection;
  }
}
