using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Door : MonoBehaviour
{
    public enum DoorDirection { Left, Right } // 열리는 방향 Enum
    public DoorDirection direction; // 방향 변수
}
