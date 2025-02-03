using UnityEngine;

public class ScytheProjectile : MonoBehaviour
{
  public float speed = 5f;
  public float returnSpeed = 7f;
  private Vector3 direction;
  private bool returning = false;

  public void Initialize(Vector3 direction)
  {
    this.direction = direction.normalized;
  }

  private void Update()
  {
    if ( !returning )
    {
      // 앞으로 이동
      transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    else
    {
      // 되돌아가기
      Transform reaper = FindObjectOfType<GrimReaper>().transform;
      Vector3 returnDirection = ( reaper.position - transform.position ).normalized;
      transform.Translate(returnDirection * returnSpeed * Time.deltaTime, Space.World);
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if ( collision.gameObject.CompareTag("Wall") )
    {
      // 벽에 닿으면 되돌아가기 시작
      returning = true;
    }
  }
}
