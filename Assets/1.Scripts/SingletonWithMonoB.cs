using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonWithMonoB<T> : MonoBehaviour where T : Component 
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                {
                    // Test Code
                    Debug.Log("Found an instance about the singleton <" + typeof(T).ToString() + ">.");

                    _instance = objs[0];
                }
                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }
                if (_instance == null)
                {
                    // Test Code
                    Debug.Log("There is no instance in the scene about the singleton <" + typeof(T).ToString() + ">. Create new one");

                    GameObject obj = new GameObject();
                    //obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}


public class SingletonWithMonoBPersistent<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
