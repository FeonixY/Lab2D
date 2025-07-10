using System.Collections.Generic;
using UnityEngine;

public class UIData { }

public abstract class UIScreenBase : MonoBehaviour
{
    public UIScreenCacheType CacheType;

    protected readonly List<CommonUIElement> DefaultUIElements = new();

    public void CreateScreen(UIData data)
    {
        OnCreated(data);
    }
    
    public void DestroyScreen()
    {
        OnDestroyed();
        
        Destroy(gameObject);
    }
    
    public void EnableScreen(UIData data = null)
    {
        OnEnabled(data);
        
        gameObject.SetActive(true);
    }

    public void DisableScreen()
    {
        OnDisabled();
        
        gameObject.SetActive(false);
    }
    
    public void RefreshScreen(UIData data, List<CommonUIElement> elements = null)
    {
        OnRefreshed(data);

        foreach (CommonUIElement element in elements ?? DefaultUIElements)
        {
            element.Refresh(data);
        }
    }
    
    protected virtual void OnCreated(UIData data) { }
    protected virtual void OnDestroyed() { }
    
    protected virtual void OnEnabled(UIData data = null) { }
    
    protected virtual void OnDisabled() { }

    protected virtual void OnRefreshed(UIData data) { }
}

public abstract class CommonUIScreenBase : UIScreenBase { }
public abstract class WidgetUIScreenBase : UIScreenBase { }

public enum UIScreenState
{
    Enabled,
    Disabled,
    Destroyed
}

public enum UIScreenCacheType
{
    Cache,
    NotCache
}
