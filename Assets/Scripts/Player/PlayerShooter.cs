using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Transform gunPivot;

    public Gun gun;
    [SerializeField]
    private PlayerInput input;
    [SerializeField]
    private Animator animator;

    private void Awake()
        {
            input = GetComponent<PlayerInput>();
            animator = GetComponent<Animator>();
        }

    private void Update()
    {   
        if (input.Fire)    
        {
             gun.Fire();
        }
        // if (input.Reload)
        // {
                
        //     if (gun.GunState != Gun.State.Reloading) 
        //     {
        //         animator.SetTrigger("Reload"); 
        //         gun.Reload(); 
        //     }

        // }

    }
}
