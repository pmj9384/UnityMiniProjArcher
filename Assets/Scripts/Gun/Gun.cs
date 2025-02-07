using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

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
  public Transform doubleFirePoint;
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
    if ( skillController == null ) return;

    // 🔥 기본 발사 위치 리스트
    List<Transform> bulletPoints = new List<Transform> { firePoint };

    // 🔥 더블샷이면 추가 발사 위치 적용
    if ( skillController.HasDoubleShot )
    {
      bulletPoints.Add(doubleFirePoint);
    }

    // 🔥 사선 화살이면 왼쪽, 오른쪽 발사 추가
    if ( skillController.HasDiagonalArrow )
    {
      bulletPoints.Add(leftFirePoint);
      bulletPoints.Add(rightFirePoint);
    }

    // 🔥 멀티샷이면 기존 발사 리스트를 한 번 더 반복 (두 배 발사)
    int repeatCount = skillController.HasMultiShot ? 2 : 1;

    for ( int i = 0; i < repeatCount; i++ )
    {
      foreach ( Transform shootPoint in bulletPoints )
      {
        if ( shootPoint != null )
        {
          FireSingleBullet(shootPoint);
        }
      }
    }
  }

  private void FireSingleBullet(Transform shootPoint)
  {
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
