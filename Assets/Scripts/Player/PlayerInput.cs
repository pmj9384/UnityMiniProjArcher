using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string moveAxisName = "Vertical";
    public static readonly string rotateAxisName = "Horizontal";
    public static readonly string  fireAxisName = "Fire1";
    // public static readonly string  reloadAxisName = "Reload";

    public float Move {get; private set; }
    public float Rotate {get; private set; }
    public bool Fire {get; private set; }

    // public bool Reload {get; private set; }


    // Update is called once per frame
    void Update()
    {
        Move = Input.GetAxis(moveAxisName);
        Rotate = Input.GetAxis(rotateAxisName);
        Fire = Input.GetButton(fireAxisName);
        // Reload = Input.GetButtonDown(reloadAxisName);
    }
}
