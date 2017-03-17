using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourWallObject : BehaviourWorldObjectBase 
{
  [Space(32)]

  public Transform FrontQuad;
  public Transform BackQuad;
  public Transform LeftQuad;
  public Transform RightQuad;
  public Transform TopQuad;
  public Transform BottomQuad;
}
