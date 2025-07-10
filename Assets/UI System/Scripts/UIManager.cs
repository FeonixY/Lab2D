using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public Canvas RootCanvas;
    
    private readonly Stack<UIScreenNode> screenStack = new();
    private readonly List<UIScreenNode> widgetScreenNodes = new();

    public bool TryOpenScreen<T>(UIData data = null) where T : CommonUIScreenBase
    {
        if (!UIResource.Instance.TryGetScreen<T>(out GameObject screenPrefab)
            || screenStack.FirstOrDefault(node => node.UIScreen is T) != null) return false;

        UIScreenBase uiScreenBase = Instantiate(screenPrefab, RootCanvas.transform).GetComponent<UIScreenBase>();
        UIScreenNode uiScreenNode = new(uiScreenBase);
        screenStack.Push(uiScreenNode);
        uiScreenNode.Open(data);

        return true;
    }
    
    public bool TryCloseScreen<T>() where T : CommonUIScreenBase
    {
        if (screenStack.Count == 0) return false;

        int index = screenStack
        .Select((node, i) => new { node, i })
        .FirstOrDefault(x => x.node.UIScreen is T)?.i ?? -1;

        if (index == -1) return false;

        for (int i = 0; i <= index; i++)
        {
            UIScreenNode node = screenStack.Pop();
            node.CloseRecursively();
        }

        return true;
    }

    public bool TryOpenWidgetScreen<TWidget, TParent>(UIData data)
    where TWidget : WidgetUIScreenBase
    where TParent : CommonUIScreenBase
    {
        if (!UIResource.Instance.TryGetScreen<TWidget>(out GameObject screenPrefab)
            || widgetScreenNodes.FirstOrDefault(node => node.UIScreen is TWidget) != null) return false;

        UIScreenNode parentNode = screenStack.FirstOrDefault(node => node.UIScreen is TParent);
        if (parentNode == null) return false;

        UIScreenBase uiScreenBase = Instantiate(screenPrefab, RootCanvas.transform).GetComponent<UIScreenBase>();
        UIScreenNode uiScreenNode = new(uiScreenBase);
        parentNode.AddRelatedWidgetUIScreen(uiScreenNode);
        uiScreenNode.Open(data);
        widgetScreenNodes.Add(uiScreenNode);

        return true;
    }

    public bool TryCloseWidgetScreen<T>() where T : WidgetUIScreenBase
    {
        UIScreenNode popUpNode = widgetScreenNodes.FirstOrDefault(node => node.UIScreen is T);
        if (popUpNode == null) return false;

        popUpNode.CloseRecursively();
        widgetScreenNodes.Remove(popUpNode);

        return true;
    }
}
