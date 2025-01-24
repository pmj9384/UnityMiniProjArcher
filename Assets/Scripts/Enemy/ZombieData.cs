using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/ZombieData",fileName = "Zombie Data")]
public class ZombieData : ScriptableObject
{
    public float hp = 100f;
    public float damage = 20f;

    public float speed = 2f;
    public Color skinColor = Color.white;



}
