using UnityEngine;
using System.Collections;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
  static T _instance = null;

  static string _resourcePath = "";

	static bool applicationIsQuitting = false;
  static bool _instantiated = false;

	public static T Instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit." + " Won't create again - returning null.");
				return null;
			}

      if (_instantiated) return _instance;

			if (_instance == null)
			{
        bool error = false;

				_instance = GameObject.FindObjectOfType(typeof(T)) as T;

        if (_instance == null)
        {
          var res_path = _resourcePath + typeof(T).ToString();
          
          T resource = Resources.Load(res_path, typeof(T)) as T;

          if (resource != null)
          {
            _instance = Instantiate(resource) as T;
            _instance.name = typeof(T).ToString() + ".Singleton";
          }

          if (_instance == null)
          {
            _instance = new GameObject(typeof(T).ToString() + ".Singleton", typeof(T)).GetComponent<T>();

            if (_instance == null)
            {
              Debug.LogError("Problem during the creation of " + typeof(T).ToString());
              error = true;
            }
          }
        }

        if (!error)
        {
          _instantiated = true;

          DontDestroyOnLoad(_instance.gameObject);          
        }
			}

      return _instance;
		}
	}

  void Awake()
  {
    Instance.Init();
  }

  protected virtual void Init()
  {
  }
  	
	protected void OnDestroy()
	{
    _instantiated = false;
		applicationIsQuitting = true;
	}

  public static bool isInstantinated { get { return _instantiated; } }
}