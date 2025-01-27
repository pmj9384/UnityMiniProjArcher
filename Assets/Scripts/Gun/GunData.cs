using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Gundata", fileName = "GunData")]// atributes
public class GunData : ScriptableObject
{
  public AudioClip shotClip;
  //  public AudioClip reloadClip;

  public float damage;

  public int startAmmoRemain;
  public int magCapacity;
  public float fireRate;

  // public float reloadTime = 1f;




}
