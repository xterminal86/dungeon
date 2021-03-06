﻿using UnityEngine;
using System.Collections;

public class BehaviourWorldObject : BehaviourWorldObjectBase
{
  [Header("Sound that is played when object starts the \"on\" state (e.g. door opening)")]
  public AudioSource OnStateBeginSound;
  [Header("Sound that is played when \"on\" state completes")]
  public AudioSource OnStateFinishedSound;

  [Header("Sound that is played when object starts the \"off\" state (e.g. door closing)")]
  public AudioSource OffStateBeginSound;
  [Header("Sound that is played when \"off\" state completes")]
  public AudioSource OffStateFinishedSound;

  [Space(32)]
  [Header("For walls")]

  public Transform FrontQuad;
  public Transform BackQuad;
  public Transform LeftQuad;
  public Transform RightQuad;
  public Transform TopQuad;
  public Transform BottomQuad;

  public Transform WallColumnLeft;
  public Transform WallColumnRight;
}
