using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;

    public AudioClip deathClip;
    public AudioClip hitSound;
    // public AudioClip itemPickupSound;
    private AudioSource audioSource;
    private Animator animator;
    private PlayerMovement movement;
    private PlayerShooter shooter;

    private GameManager gm;  

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        shooter = GetComponent<PlayerShooter>();
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);
 
        healthSlider.maxValue = maxHp;
        healthSlider.minValue = 0f;
        healthSlider.value = Hp;

        movement.enabled = true;
        shooter.enabled = true;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = Hp;
        if (!IsDead)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
    protected override void Die()
    {
        base.Die();

        healthSlider.gameObject.SetActive(false);
        animator.SetTrigger("Die");

        audioSource.PlayOneShot(deathClip);
        movement.enabled = false;
        shooter.enabled = false;
        if (gm != null)
        {
        gm.OnGameOver(); 
        }

    }
    public override void AddHp(float add)
    {
        base.AddHp(add);
        healthSlider.value = Hp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnDamage(20,Vector3.zero, Vector3.zero);

        }
         if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddHp(20);
            
        }

   
    }

private void OnTriggerEnter(Collider other)
{
    if (IsDead) return;

    if (other.CompareTag("Enemy"))
    {
        var enemy = other.GetComponent<Enemy>();  
        if (enemy != null)
        {
            OnDamage(enemy.damage, other.transform.position, Vector3.zero);  
            Debug.Log("Player hit by zombie!");
        }
    }
}

}
