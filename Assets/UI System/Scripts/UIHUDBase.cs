using System.Collections.Generic;
using UnityEngine;

public abstract class UIData { }

public abstract class UIHUDBase : MonoBehaviour
{
    public UIHUDCacheType CacheType = UIHUDCacheType.NotCache;

    public void CreateHUD(UIData data)
    {
        OnCreated(data);
    }
    
    public void DestroyHUD()
    {
        OnDestroyed();
        
        Destroy(gameObject);
    }
    
    public void EnableHUD(UIData data = null)
    {
        OnEnabled(data);
        
        gameObject.SetActive(true);
    }

    public void DisableHUD()
    {
        OnDisabled();
        
        gameObject.SetActive(false);
    }

    public void UpdateHUD()
    {
        OnUpdated();
    }
    
    public void RefreshHUD(UIData data, List<UIElement> elements = null)
    {
        OnRefreshed(data);
    }
    
    protected virtual void CloseHUD() { }
    protected virtual void OnCreated(UIData data) { }
    protected virtual void OnDestroyed() { }
    protected virtual void OnEnabled(UIData data = null) { }
    protected virtual void OnDisabled() { }
    protected virtual void OnUpdated() { }
    protected virtual void OnRefreshed(UIData data) { }
}

public abstract class CommonUIHUDBase : UIHUDBase
{ 
    protected override void CloseHUD() => UIManager.Instance.TryCloseCommonHUD(GetType());
}

public abstract class WidgetUIHUDBase : UIHUDBase
{ 
    protected override void CloseHUD() => UIManager.Instance.TryCloseWidgetHUD(GetType());
}

public enum UIHUDState
{
    NotInitializedOrDestroyed,
    Enabled,
    Disabled
}

public enum UIHUDCacheType
{
    Cache,
    NotCache
}