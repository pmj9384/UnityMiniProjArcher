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
    playerMovement = GetComponent<PlayerMovement>();
  }

  private void Update()
  {
    bool isMoving = animator.GetBool("Walk");
    bool isFiring = !isMoving;


    if ( GameManager.Instance.remainingZombies <= 0 )
    {
      animator.SetBool("Attack", false);
      animator.SetBool("Idle", true);
      return;
    }


    if ( isFiring )
    {
      animator.SetBool("Attack", true);
      animator.SetBool("Idle", false);
    }
    else
    {
      animator.SetBool("Attack", false);
      animator.SetBool("Idle", true);
    }
  }
}
