using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
  public enum State { Ready, Empty, Reloading }
  public State GunState { get; private set; }
  public GunData gundata;

  public GameObject bulletPrefab;
  public Transform firePoint;
  public Transform leftFirePoint;  // 🔥 좌측 발사점 (대각선 화살 용)
  public Transform rightFirePoint; // 🔥 우측 발사점 (대각선 화살 용)

  private PlayerSkillController skillController;
  private AudioSource audioSource;
  public ParticleSystem muzzleEffect;
  public ParticleSystem shellEffect;

  private float lastFireTime;
  private int currentAmmo;
  public float cooldownTime = 1f;
  public VirtualJoyStick joystick;

  private IObjectPool<GameObject> bulletPool;

  private void Awake()
  {
    skillController = GetComponentInParent<PlayerSkillController>();
    audioSource = GetComponent<AudioSource>();
    bulletPool = new ObjectPool<GameObject>(
        createFunc: () => Instantiate(bulletPrefab),
        actionOnGet: bullet => bullet.SetActive(true),
        actionOnRelease: bullet =>
        {
          bullet.SetActive(false);
          if ( bulletPool.CountInactive > 50 ) // 🛑 최대 개수 초과 시 삭제
          {
            Destroy(bullet);
          }
        },
        actionOnDestroy: bullet => Destroy(bullet), // ✅ 삭제 시 명시적으로 Destroy()
        collectionCheck: false,
        maxSize: 50 // 🎯 최대 50개까지만 유지
    );

  }

  private void OnEnable()
  {
    GunState = State.Ready;
    lastFireTime = 0f;
  }

  public void Fire()
  {
    if ( GunState == State.Ready && Time.time >= lastFireTime + cooldownTime )
    {
      lastFireTime = Time.time;

      // 🔥 기본 탄환 발사
      // ShootBullet(firePoint);
    }
  }

  public void ShootBullet() // ✅ 애니메이션 이벤트 전용 메서드
  {
    ShootBullet(firePoint); // 기본 발사점 사용

    if ( skillController != null && skillController.HasDiagonalArrow )
    {
      ShootBullet(leftFirePoint);
      ShootBullet(rightFirePoint);
    }

  }

  private void ShootBullet(Transform shootPoint)
  {
    if ( shootPoint == null )
    {
      Debug.LogError("⚠ firePoint가 설정되지 않았음!");
      return;
    }

    GameObject bulletObject = bulletPool.Get();
    bulletObject.transform.position = shootPoint.position;
    bulletObject.transform.rotation = shootPoint.rotation;

    Bullet bulletScript = bulletObject.GetComponent<Bullet>();
    if ( bulletScript != null )
    {
      bulletScript.Launch(shootPoint.forward, bulletPool);

      if ( skillController != null )
      {
        skillController.ModifyBullet(bulletScript);
      }
    }
  }

}

