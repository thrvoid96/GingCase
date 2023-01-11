using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            // Initialize instance
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                // If can't find type, create one
                if (_instance == null)
                {
                    Debug.LogError($"An instance of {typeof(T)} is missing.");
                }
                else
                {
                    Singleton<T> instance = _instance as Singleton<T>;
                    instance.Initialize();
                }

            }

            return _instance;
        }
    }

    protected virtual void Initialize()
    {
        
    }

}