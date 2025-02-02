using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float speed = 5f;
  public float knockbackForce = 5f;
  public float knockbackDuration = 0.2f;
  public VirtualJoyStick joystick;
  public LayerMask targetLayer;
  public float targetRange = 10f;
  private Transform target;
  private Rigidbody rb;
  private float rotationSpeed = 10f;
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

    foreach ( Collider enemy in enemiesInRange )
    {
      float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
      if ( distanceToEnemy < closestDistance )
      {
        closestDistance = distanceToEnemy;
        closestEnemy = enemy.transform;
      }
    }

    target = closestEnemy;
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
