using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IFlyMoveBehavior
{
  void Move(Rigidbody rb, Transform target);
}