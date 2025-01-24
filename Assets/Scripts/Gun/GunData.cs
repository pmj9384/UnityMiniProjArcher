using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Gundata", fileName = "GunData")]// atributes
public class GunData : ScriptableObject
{
 public AudioClip shotClip;
//  public AudioClip reloadClip;

public float damage = 25f;

public int startAmmoRemain = 100;
public int magCapacity = 25;
public float fireRate= 1f;

// public float reloadTime = 1f;




}
