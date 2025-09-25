using System.Collections.Generic;

public class UIHUDNode
{
    public UIHUDBase UIHUD { get; private set; }
    private UIHUDState hudState = UIHUDState.NotInitializedOrDestroyed;
    private readonly List<UIHUDNode> relatedWidgetUIHUDs = new();

    public UIHUDNode(UIHUDBase uiHUD)
    {
        UIHUD = uiHUD;
    }

    public void Open(UIData data)
    {
        switch (hudState)
        {
            case UIHUDState.NotInitializedOrDestroyed:
                UIResourceManager.Instance.TryAddHUDInstance(UIHUD);
                UIHUD.CreateHUD(data);
                break;
            case UIHUDState.Disabled:
                if (UIResourceManager.Instance.TryGetHUDInstance(out UIHUDBase uiScreenBase))
                {
                    UIHUD = uiScreenBase;
                    UIHUD.EnableHUD(data);
                }
                break;
            case UIHUDState.Enabled:
                UIHUD.RefreshHUD(data);
                break;
        }

        hudState = UIHUDState.Enabled;
    }

    public void CloseRecursively()
    {
        foreach (UIHUDNode relatedWidgetUIScreen in relatedWidgetUIHUDs)
        {
            relatedWidgetUIScreen.CloseRecursively();
        }

        Close();
    }

    public void AddRelatedWidgetUIHUD(UIHUDNode node)
    {
        relatedWidgetUIHUDs.Add(node);
    }

    private void Close()
    {
        switch (UIHUD.CacheType)
        {
            case UIHUDCacheType.Cache:
                DisableNodeRecursively();
                hudState = UIHUDState.Disabled;
                break;
            case UIHUDCacheType.NotCache:
                DestroyNodeRecursively();
                UIResourceManager.Instance.TryRemoveHUDInstance(UIHUD);
                hudState = UIHUDState.NotInitializedOrDestroyed;
                break;
        }
    }

    private void DisableNodeRecursively()
    {
        UIHUD.DisableHUD();
        foreach (UIHUDNode relatedWidgetUIScreen in relatedWidgetUIHUDs)
        {
            relatedWidgetUIScreen.DisableNodeRecursively();
        }
    }

    private void DestroyNodeRecursively()
    {
        // Remove from UIResourceManager before destroying
        if (UIHUD != null)
        {
            UIResourceManager.Instance.TryRemoveHUDInstance(UIHUD);
        }
        
        UIHUD.DestroyHUD();
        UIHUD = null;
        
        foreach (UIHUDNode relatedWidgetUIScreen in relatedWidgetUIHUDs)
        {
            relatedWidgetUIScreen.DestroyNodeRecursively();
        }
    }
}