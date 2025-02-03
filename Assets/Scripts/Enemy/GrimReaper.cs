using UnityEngine;

public class GrimReaper : Enemy
{
  protected override void OnEnable()
  {
    base.OnEnable();
    Debug.Log("Grim Reaper 활성화!");
  }
}
