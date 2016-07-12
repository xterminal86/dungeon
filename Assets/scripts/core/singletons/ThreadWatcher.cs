using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThreadWatcher : MonoSingleton<ThreadWatcher> 
{
  [HideInInspector]
  public bool StopAllThreads = false;
}
