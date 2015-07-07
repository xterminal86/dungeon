using UnityEngine;
using System.Collections;

public class JobManager : MonoSingleton<JobManager>
{
  Job _job;
  public void CreateJob(IEnumerator coroutineMethod)
  {
    _job = new Job(coroutineMethod);
  }
}

public class Job
{
  IEnumerator _coroutineMethod;
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
