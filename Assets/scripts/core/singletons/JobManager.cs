using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Class for centralized managing of coroutines.
/// Start all coroutines here and stop using JobManager.Instance.StopAllCoroutines()
/// </summary>
public class JobManager : MonoSingleton<JobManager>
{  
  public Job CreateCoroutine(IEnumerator coroutineMethod)
  {
    return new Job(coroutineMethod);
  }

  /// <summary>
  /// Always check for ThreadWatcher.Instance.StopAllThreads in a thread method loop
  /// to avoid possible hanging threads in Unity Editor after stopping the game.
  /// </summary>
  /// <param name="threadMethod">Reference to a thread function</param>
  public void CreateThread(Callback threadMethod)
  {
    if (threadMethod == null) return;

    Thread t = new Thread(new ThreadStart(threadMethod));
    t.IsBackground = true;
    t.Start();    
  }

  public void CreateThreadB(CallbackO threadMethod, bool arg)
  {
    if (threadMethod == null) return;

    Thread t = new Thread(new ParameterizedThreadStart(threadMethod));
    t.IsBackground = true;
    t.Start(arg);    
  }

  void OnDestroy()
  {
    base.OnDestroy();

    if (ThreadWatcher.isInstantinated)
    {
      ThreadWatcher.Instance.StopAllThreads = true;
    }
  }
}

public class Job
{
  IEnumerator _coroutineMethod;
  public IEnumerator CoroutineMethod
  {
    get { return _coroutineMethod; }
  }

  int _hash = -1;
  public Job (IEnumerator coroutineMethod)
  {
    _coroutineMethod = coroutineMethod;
    _hash = coroutineMethod.GetHashCode();
    JobManager.Instance.StartCoroutine(_coroutineMethod);
  }

  public void KillJob()
  {    
    JobManager.Instance.StopCoroutine(_coroutineMethod);
  }

  public void RestartJob()
  {
    JobManager.Instance.StartCoroutine(_coroutineMethod);
  }
}
