using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    private readonly Stack<UIHUDNode> commonHUDStack = new();
    private readonly List<UIHUDNode> widgetHUDNodes = new();

    private void Update()
    {
        if (commonHUDStack.Count > 0) commonHUDStack.Peek().UIHUD?.UpdateHUD();
    }

    public bool TryOpenCommonHUD<T>(UIData data = null) where T : CommonUIHUDBase
    {
        if (!UIResourceManager.Instance.TryGetHUD<T>(out GameObject screenPrefab)) return false;

        UIHUDNode existingNode = commonHUDStack.FirstOrDefault(node => node.UIHUD is T);
        if (existingNode != null)
        {
            existingNode.Open(data);
            return true;
        }

        UIHUDBase uihudBase = Instantiate(screenPrefab, transform).GetComponent<UIHUDBase>();
        UIHUDNode uiHUDNode = new(uihudBase);
        commonHUDStack.Push(uiHUDNode);
        uiHUDNode.Open(data);

        return true;
    }

    public bool TryCloseCommonHUD<T>() where T : CommonUIHUDBase
    {
        if (commonHUDStack.Count == 0) return false;

        int index = commonHUDStack
        .Select((node, i) => new { node, i })
        .FirstOrDefault(x => x.node.UIHUD is T)?.i ?? -1;

        if (index == -1) return false;

        for (int i = 0; i <= index; i++)
        {
            UIHUDNode node = commonHUDStack.Pop();
            node.CloseRecursively();
        }

        return true;
    }

    public bool TryCloseCommonHUD(Type type)
    {
        if (commonHUDStack.Count == 0) return false;

        int index = commonHUDStack
            .Select((node, i) => new { node, i })
            .FirstOrDefault(x => x.node.UIHUD.GetType() == type)?.i ?? -1;

        if (index == -1) return false;

        for (int i = 0; i <= index; i++)
        {
            UIHUDNode node = commonHUDStack.Pop();
            node.CloseRecursively();
        }

        return true;
    }

    public bool TryRefreshCommonHUD<T>(UIData data) where T : CommonUIHUDBase
    {
        UIHUDNode node = commonHUDStack.FirstOrDefault(node => node.UIHUD is T);
        if (node == null) return false;

        node.UIHUD.RefreshHUD(data);
        return true;
    }

    public bool TryOpenWidgetHUD<TWidget, TParent>(UIData data)
    where TWidget : WidgetUIHUDBase
    where TParent : UIHUDBase
    {
        if (UIResourceManager.Instance.TryGetHUD<TWidget>(out GameObject screenPrefab)
            || widgetHUDNodes.FirstOrDefault(node => node.UIHUD is TWidget) != null) return false;

        UIHUDNode parentNode = commonHUDStack.FirstOrDefault(node => node.UIHUD is TParent)
            ?? widgetHUDNodes.FirstOrDefault(node => node.UIHUD is TParent);
        if (parentNode == null) return false;

        UIHUDBase uihudBase = Instantiate(screenPrefab, transform).GetComponent<UIHUDBase>();
        UIHUDNode uiHUDNode = new(uihudBase);
        parentNode.AddRelatedWidgetUIHUD(uiHUDNode);
        uiHUDNode.Open(data);
        widgetHUDNodes.Add(uiHUDNode);

        return true;
    }

    public bool TryCloseWidgetHUD<T>() where T : WidgetUIHUDBase
    {
        UIHUDNode widgetNode = widgetHUDNodes.FirstOrDefault(node => node.UIHUD is T);
        if (widgetNode == null) return false;

        widgetNode.CloseRecursively();
        widgetHUDNodes.Remove(widgetNode);

        return true;
    }

    public bool TryCloseWidgetHUD(Type type)
    {
        UIHUDNode widgetNode = widgetHUDNodes.FirstOrDefault(node => node.UIHUD.GetType() == type);
        if (widgetNode == null) return false;

        widgetNode.CloseRecursively();
        widgetHUDNodes.Remove(widgetNode);

        return true;
    }

    public bool TryRefreshWidgetHUD<T>(UIData data) where T : WidgetUIHUDBase
    {
        UIHUDNode widgetNode = commonHUDStack.FirstOrDefault(node => node.UIHUD is T);
        if (widgetNode == null) return false;

        widgetNode.UIHUD.RefreshHUD(data);
        return true;
    }
}
