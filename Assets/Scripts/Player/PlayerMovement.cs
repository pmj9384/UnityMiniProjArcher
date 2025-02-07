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

    // 🔥 Rigidbody 설정 변경 (벽에 끼임 방지)
    rb.freezeRotation = true; // 회전 방지
    rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 빠른 충돌 감지
  }

  private void Update()
  {
    HandleTargeting();
    CheckPlayerPosition(); // 🔥 벽에 끼이면 자동 복구
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
      Vector3 moveDirection = moveInput * speed;

      // 🔥 벽 감지 후 이동 제한
      if ( IsWallAhead(moveDirection) )
      {
        moveDirection = Vector3.zero; // 벽 감지 시 이동 차단
      }

      rb.AddForce(moveDirection, ForceMode.VelocityChange); // 🔥 부드러운 이동
      animator.SetBool("Walk", true);
      RotateTowardsDirection(moveInput);
    }
    else
    {
      rb.velocity = Vector3.zero; // 🔥 멈출 때 물리적 이동도 멈추기
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

  // 🔥 벽 감지 후 이동 제한 (밀리는 문제 해결)
  private bool IsWallAhead(Vector3 moveDirection)
  {
    RaycastHit hit;
    if ( Physics.Raycast(rb.position, moveDirection, out hit, 0.5f, wallLayer) )
    {
      return true; // 벽 감지 → 이동 금지
    }
    return false;
  }

  // 🔥 플레이어가 벽에 끼이면 자동 복구 (맵 밖으로 나가지 않도록 방지)
  private void CheckPlayerPosition()
  {
    if ( !Physics.Raycast(transform.position, Vector3.down, 1f, LayerMask.GetMask("Ground")) )
    {
      Debug.Log($"{gameObject.name}: 벽에 끼였음! 위치 복구 중...");
      if ( Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f, LayerMask.GetMask("Ground")) )
      {
        transform.position = hit.point; // 🔥 바닥이 감지되면 강제 이동
      }
    }
  }
}
