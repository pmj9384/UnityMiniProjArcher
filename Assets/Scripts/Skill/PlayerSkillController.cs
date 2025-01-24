using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public GameObject bulletPrefab; // 기본 탄환 Prefab
    public Transform leftShootPoint; // 왼쪽 발사 지점
    public Transform rightShootPoint; // 오른쪽 발사 지점

    private PlayerMovement playerMovement; // 플레이어 이동 속도 조절
    private PlayerHealth playerHealth; // 플레이어 체력 관리
    public GunData gunData; // 플레이어 총 데이터 관리

    private bool hasDiagonalArrow = false;

    private void Start()
    {
        // 필요한 컴포넌트 가져오기
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
    
    }

    // 사선 화살 효과
    public void ApplyDiagonalArrow()
    {
        if (bulletPrefab != null)
        {
            hasDiagonalArrow = true;
            if (leftShootPoint != null)
            {
                // 왼쪽 사선 탄환 발사
                Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation * Quaternion.Euler(0, -45, 0));
            }

            if (rightShootPoint != null)
            {
                // 오른쪽 사선 탄환 발사
                Instantiate(bulletPrefab, rightShootPoint.position, rightShootPoint.rotation * Quaternion.Euler(0, 45, 0));
            }

            Debug.Log("Diagonal Arrow effect applied!");
        }
        else
        {
            Debug.LogError("BulletPrefab is missing!");
        }
    }

 public void ApplyFireArrow()
{
    if (bulletPrefab != null)
    {
        if (hasDiagonalArrow) 
        {
            // 사선 화살이 있을 때는 왼쪽과 오른쪽에서 화염 화살 발사
            GameObject leftBullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
            GameObject rightBullet = Instantiate(bulletPrefab, rightShootPoint.position, rightShootPoint.rotation);

            // 화염 화살에 맞은 적에게 데미지 적용
            if (leftBullet != null)
            {
                ApplyFireDamage(leftBullet);
            }

            if (rightBullet != null)
            {
                ApplyFireDamage(rightBullet);
            }

            Debug.Log("Fire Arrow effect applied with diagonal arrow!");
        }
        else
        {
            // 사선 화살이 없을 때는 한 곳에서만 발사
            GameObject bullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
            ApplyFireDamage(bullet);
            Debug.Log("Fire Arrow effect applied without diagonal arrow!");
        }
    }
    else
    {
        Debug.LogError("BulletPrefab is missing!");
    }
}


    void ApplyFireDamage(GameObject bullet)
    {
        // 화염 효과를 적용할 적이 있는지 확인
        RaycastHit hit;
        if (Physics.Raycast(bullet.transform.position, bullet.transform.forward, out hit))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 화염 효과 적용: 2초 동안 초당 5의 데미지
                enemy.ApplyFireEffect(2f, 5f); // 지속 시간 2초, 초당 5의 데미지
            }
        }
    }



public void ApplyFrostArrow()
{
    if (bulletPrefab != null)
    {
        if (hasDiagonalArrow)
        {
            // 사선 화살이 있을 때는 왼쪽과 오른쪽에서 얼음 화살 발사
            GameObject leftBullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
            GameObject rightBullet = Instantiate(bulletPrefab, rightShootPoint.position, rightShootPoint.rotation);

            // 얼음 화살에 맞은 적에게 얼음 효과 적용
            if (leftBullet != null)
            {
                ApplyFrostEffect(leftBullet);
            }

            if (rightBullet != null)
            {
                ApplyFrostEffect(rightBullet);
            }

            Debug.Log("Frost Arrow effect applied with diagonal arrow!");
        }
        else
        {
            // 사선 화살이 없을 때는 한 곳에서만 발사
            GameObject bullet = Instantiate(bulletPrefab, leftShootPoint.position, leftShootPoint.rotation);
            ApplyFrostEffect(bullet);
            Debug.Log("Frost Arrow effect applied without diagonal arrow!");
        }
    }
    else
    {
        Debug.LogError("BulletPrefab is missing!");
    }
}


    void ApplyFrostEffect(GameObject bullet)
    {
        // 얼음 효과를 적용할 적이 있는지 확인
        RaycastHit hit;
        if (Physics.Raycast(bullet.transform.position, bullet.transform.forward, out hit))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 얼음 효과 적용: 2초 동안 속도 감소
                enemy.ApplyFrostEffect(2f, 1f); // 지속 시간 2초, 속도 감소량 1f
            }
        }
    }

    // 공격력 증가
    public void IncreaseAttackPower()
    {
        if (gunData != null)
        {
            gunData.damage += 10f; // 총 데미지 10 증가
            Debug.Log("Attack power increased!");
        }
        else
        {
            Debug.LogError("GunData is missing!");
        }
    }

    // 이동속도 증가
    public void IncreaseMovementSpeed()
    {
        if (playerMovement != null)
        {
            playerMovement.speed += 1f; // 이동 속도 1 증가
            Debug.Log("Movement speed increased!");
        }
        else
        {
            Debug.LogError("PlayerMovement is missing!");
        }
    }

    // 체력 회복
    public void RecoverHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.AddHp(30f); // 체력 30 회복
            Debug.Log("Health recovered!");
        }
        else
        {
            Debug.LogError("PlayerHealth is missing!");
        }
    }

    // 최대 체력 증가
    public void IncreaseMaxHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.maxHp += 10f; // 최대 체력 10 증가
            playerHealth.AddHp(10f); // 현재 체력도 증가
            playerHealth.healthSlider.maxValue = playerHealth.maxHp; // 슬라이더 업데이트
            Debug.Log("Max health increased!");
        }
        else
        {
            Debug.LogError("PlayerHealth is missing!");
        }
    }

    // 공격속도 증가
    public void IncreaseAttackSpeed()
    {
        if (gunData != null)
        {
            gunData.fireRate = Mathf.Max(0.1f, gunData.fireRate - 0.1f); // 발사 속도 0.1 감소 (최소값 0.1 유지)
            Debug.Log("Attack speed increased!");
        }
        else
        {
            Debug.LogError("GunData is missing!");
        }
    }
}
