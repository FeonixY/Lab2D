using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public bool IsDontDestroyOnLoad = true;
    
    public static T Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (IsDontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void OnApplicationQuit()
    {
        Instance = null;
    }
}