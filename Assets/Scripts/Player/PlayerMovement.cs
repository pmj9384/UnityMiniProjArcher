using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float speed = 5f;
  public VirtualJoyStick joystick;
  public LayerMask targetLayer;
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
      Vector3 moveDirection = moveInput * speed * Time.deltaTime;
      rb.MovePosition(rb.position + moveDirection);
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
}
