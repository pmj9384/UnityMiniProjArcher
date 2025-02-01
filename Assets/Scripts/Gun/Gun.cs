using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
  public enum State { Ready, Empty, Reloading }

  public State GunState { get; private set; }
  public GunData gundata;

  public GameObject bulletPrefab;
  public Transform firePoint;

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
    audioSource = GetComponent<AudioSource>();

    bulletPool = new ObjectPool<GameObject>(
      createFunc: () => Instantiate(bulletPrefab),
      actionOnGet: bullet => bullet.SetActive(true),  // 오브젝트 풀에서 가져올 때 활성화
      actionOnRelease: bullet =>
      {
        bullet.SetActive(false); // 풀로 반환될 때 비활성화
      },
      actionOnDestroy: bullet =>
      {
        Debug.LogWarning($"Bullet {bullet.name} is destroyed because pool exceeded max size.");
        Destroy(bullet); // 최대 개수를 초과한 경우만 삭제
      },
      collectionCheck: false,
      maxSize: 50
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
      ShootBullet();
    }
  }

  private void ShootBullet()
  {
    if ( firePoint == null )
    {
      Debug.LogError("⚠ firePoint가 설정되지 않았음!");
      return;
    }

    GameObject bullet = bulletPool.Get();
    bullet.transform.position = firePoint.position;
    bullet.transform.rotation = firePoint.rotation;

    Bullet bulletScript = bullet.GetComponent<Bullet>();
    if ( bulletScript != null )
    {
      bulletScript.Launch(firePoint.forward, bulletPool);
    }
  }
}
