using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Camera mainCamera;
    public VirtualJoyStick joystick; // 가상 조이스틱
    public LayerMask targetLayer; // 타겟 레이어
    public float targetRange = 10f; // 타겟을 찾을 범위
    private Transform target; // 가장 가까운 타겟 (적)
    private Rigidbody rb;
    private float rotationSpeed = 10f; // 회전 속도

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
    

        // 타겟팅
        HandleTargeting();
    }
    private void FixedUpdate() 
    {

       HandleMovementAndRotation();   
    }

    private void HandleMovementAndRotation()
    {
        Vector2 joystickInput = joystick.Input; // 가상 조이스틱 입력 받기

        if (joystickInput.sqrMagnitude > 0.01f) // 입력이 있을 때
        {
            // 이동 처리
            Vector3 moveInput = new Vector3(joystickInput.x, 0f, joystickInput.y).normalized;
            Vector3 moveDirection = moveInput * speed * Time.deltaTime;
            rb.MovePosition(rb.position + moveDirection);

            // 이동 중일 때 회전
            RotateTowardsDirection(moveInput);
        }
        else
        {
            // 입력이 없을 때 타겟 방향으로 회전
            RotateTowardsTarget();
        }
    }

    private void HandleTargeting()
    {
        // 타겟을 찾는 부분 (OverlapSphere로 범위 내의 타겟을 탐색)
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, targetRange, targetLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        target = closestEnemy; // 가장 가까운 적을 타겟으로 설정
    }

    private void RotateTowardsDirection(Vector3 moveInput)
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }
    }

    private void RotateTowardsTarget()
    {
        if (target != null)
        {
            // 타겟 방향으로 회전
            Vector3 directionToTarget = target.position - rb.position;
            directionToTarget.y = 0f; // y축 방향 회전 제외
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed)); // Rigidbody로 회전
        }
    }

    public Transform GetTarget()
    {
        return target;
    }
}
