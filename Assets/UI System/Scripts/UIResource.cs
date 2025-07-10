using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIResource : MonoSingleton<UIResource>
{
    [SerializeField]
    private List<UIScreenBase> _screenDic = new();

    private readonly List<UIScreenBase> screenInstance = new();
    
    public bool TryGetScreen<T>(out GameObject screenPrefab) where T : UIScreenBase
    {
        screenPrefab = _screenDic.OfType<T>().FirstOrDefault()?.gameObject;
        return screenPrefab != null;
    }
    
    public bool TryAddScreenInstance<T>(T instance) where T : UIScreenBase
    {
        if (screenInstance.FirstOrDefault(screen => screen.GetType() == instance.GetType()) == null) return false;
        
        screenInstance.Add(instance);
        return true;
    }
    
    public bool TryRemoveScreenInstance<T>(T instance) where T : UIScreenBase
    { 
        return screenInstance.RemoveAll(screen => screen.GetType() == instance.GetType()) != 0;
    }
    
    public bool TryGetScreenInstance<T>(out T instance) where T : UIScreenBase
    {
        instance = screenInstance.OfType<T>().FirstOrDefault();
        return instance != null;
    }
}
