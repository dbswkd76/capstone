using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Data", menuName = "Scriptable Object/NPC Data")]
public class NpcData : ScriptableObject
{
  public int attackPower;   //공격력
  public float runSpeed;    //추적 속도
  public float patrolSpeed; //기본 속도
  public float navFloor;    //층 제한
}
