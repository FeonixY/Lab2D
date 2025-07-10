using System.Collections.Generic;

public class UIScreenNode
{
    public UIScreenBase UIScreen { get; private set; }
    private UIScreenState screenState = UIScreenState.Destroyed;
    private readonly List<UIScreenNode> relatedWidgetUIScreens = new();

    public UIScreenNode(UIScreenBase uiScreen)
    {
        UIScreen = uiScreen;
    }

    public void Open(UIData data)
    {
        switch (screenState)
        {
            case UIScreenState.Destroyed:
                UIResource.Instance.TryAddScreenInstance(UIScreen);
                UIScreen.CreateScreen(data);
                break;
            case UIScreenState.Disabled:
                if (UIResource.Instance.TryGetScreenInstance(out UIScreenBase uiScreenBase))
                {
                    UIScreen = uiScreenBase;
                    UIScreen.EnableScreen(data);
                }
                break;
            case UIScreenState.Enabled:
                UIScreen.RefreshScreen(data);
                break;
        }

        screenState = UIScreenState.Enabled;
    }

    public void CloseRecursively()
    {
        foreach (UIScreenNode relatedWidgetUIScreen in relatedWidgetUIScreens)
        {
            relatedWidgetUIScreen.CloseRecursively();
        }

        Close();
    }

    public void AddRelatedWidgetUIScreen(UIScreenNode node)
    {
        relatedWidgetUIScreens.Add(node);
    }

    private void Close()
    {
        switch (UIScreen.CacheType)
        {
            case UIScreenCacheType.Cache:
                DisableNodeRecursively();
                screenState = UIScreenState.Disabled;
                break;
            case UIScreenCacheType.NotCache:
                DestroyNodeRecursively();
                UIResource.Instance.TryRemoveScreenInstance(UIScreen);
                screenState = UIScreenState.Destroyed;
                break;
        }
    }

    private void DisableNodeRecursively()
    {
        UIScreen.DisableScreen();
        foreach (UIScreenNode relatedwidgetUIScreen in relatedWidgetUIScreens)
        {
            relatedwidgetUIScreen.DisableNodeRecursively();
        }
    }

    private void DestroyNodeRecursively()
    {
        UIScreen.DestroyScreen();
        UIScreen = null;
        foreach (UIScreenNode relatedwidgetUIScreen in relatedWidgetUIScreens)
        {
            relatedwidgetUIScreen.DestroyNodeRecursively();
        }
    }
}