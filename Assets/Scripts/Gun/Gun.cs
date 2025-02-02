using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
  public enum State { Ready, Empty, Reloading }
  public State GunState { get; private set; }
  public GunData gundata;

  public GameObject bulletPrefab;
  public Transform firePoint;
  public Transform leftFirePoint;
  public Transform rightFirePoint;

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
        },
       // actionOnDestroy: bullet => Destroy(bullet),
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
    }
  }

  public void ShootBullet()
  {
    ShootBullet(firePoint);

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
