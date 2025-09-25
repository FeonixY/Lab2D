using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIResourceManager : SingletonMonoBehaviour<UIResourceManager>
{
    private readonly Dictionary<Type, GameObject> prefabCache = new();
    private readonly List<UIHUDBase> hudInstance = new();
    
    public bool TryGetHUD<T>(out GameObject hudPrefab) where T : UIHUDBase
    {
        if (prefabCache.TryGetValue(typeof(T), out hudPrefab) && hudPrefab != null) return true;

        hudPrefab = Resources.Load($"UI/{typeof(T).Name}") as GameObject;

        if (hudPrefab == null) return false;

        prefabCache[typeof(T)] = hudPrefab;
        return true;
    }
    
    public bool TryAddHUDInstance<T>(T instance) where T : UIHUDBase
    {
        if (instance == null) return false;
        
        if (hudInstance.FirstOrDefault(hud => hud != null && hud.GetType() == instance.GetType()) != null) 
        {
            Debug.LogWarning($"Screen instance of type {typeof(T).Name} already exists");
            return false;
        }
        
        hudInstance.Add(instance);
        return true;
    }
    
    public bool TryRemoveHUDInstance<T>(T instance) where T : UIHUDBase
    { 
        if (instance == null) return false;
        
        // Remove by instance reference first, then by type as fallback
        int removedCount = hudInstance.RemoveAll(hud => hud == instance);
        
        if (removedCount == 0)
        {
            // Fallback: remove by type (with null check)
            removedCount = hudInstance.RemoveAll(hud => hud != null && hud.GetType() == instance.GetType());
        }
        
        return removedCount > 0;
    }
    
    public bool TryGetHUDInstance<T>(out T instance) where T : UIHUDBase
    {
        instance = hudInstance.Where(hud => hud != null).OfType<T>().FirstOrDefault();
        return instance != null;
    }
}