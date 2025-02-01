using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
  public Transform gunPivot;
  public Gun gun;

  [SerializeField]
  private PlayerInput input;
  [SerializeField]
  private Animator animator;
  private PlayerMovement playerMovement;

  private void Awake()
  {
    input = GetComponent<PlayerInput>();
    animator = GetComponent<Animator>();
    playerMovement = GetComponent<PlayerMovement>(); // ✅ PlayerMovement 참조
  }

  private void Update()
  {
    bool isMoving = animator.GetBool("Walk"); // ✅ 이동 상태 가져오기
    bool isFiring = !isMoving; // ✅ 가만히 있을 때만 공격

    if ( isFiring )
    {
      //gun.Fire();
      animator.SetBool("Attack", true);
    }
    else
    {
      animator.SetBool("Attack", false);
    }
  }
}
