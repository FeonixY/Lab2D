using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class UIElementFactory<T> where T : UIData
{
    private Action<GameObject, T, PointerEventData> onPointerEnterEvent;
    private Action<GameObject, T, PointerEventData> onPointerClickEvent;
    private Action<GameObject, T, PointerEventData> onPointerExitEvent;
    private Action<GameObject, T, PointerEventData> onBeginDragEvent;
    private Action<GameObject, T, PointerEventData> onDragEvent;
    private Action<GameObject, T, PointerEventData> onEndDragEvent;
    private Action<GameObject, T, PointerEventData> onHoverEvent;
    private Action<GameObject, T> onDestroyedEvent;
    private Action<GameObject, T> onEnabledEvent;
    private Action<GameObject, T> onDisabledEvent;
    private Action<GameObject, T> onCreatedEvent;
    private Action<GameObject, T> onRefreshedEvent;
    private readonly Dictionary<GameObject, T> dataDic = new();
    private bool isFirstCreatedDone;

    public UIElementFactory(
        Action<GameObject, T, PointerEventData> onPointerEnter = null,
        Action<GameObject, T, PointerEventData> onPointerClick = null,
        Action<GameObject, T, PointerEventData> onPointerExit = null,
        Action<GameObject, T, PointerEventData> onBeginDrag = null,
        Action<GameObject, T, PointerEventData> onDrag = null,
        Action<GameObject, T, PointerEventData> onEndDrag = null,
        Action<GameObject, T, PointerEventData> onHover = null,
        Action<GameObject, T> onDestroyed = null,
        Action<GameObject, T> onEnabled = null,
        Action<GameObject, T> onDisabled = null,
        Action<GameObject, T> onCreated = null,
        Action<GameObject, T> onRefreshed = null)
    {
        onPointerEnterEvent = onPointerEnter;
        onPointerClickEvent = onPointerClick;
        onPointerExitEvent = onPointerExit;
        onBeginDragEvent = onBeginDrag;
        onDragEvent = onDrag;
        onEndDragEvent = onEndDrag;
        onHoverEvent = onHover;
        onDestroyedEvent = onDestroyed;
        onEnabledEvent = onEnabled;
        onDisabledEvent = onDisabled;
        onCreatedEvent = onCreated;
        onRefreshedEvent = onRefreshed;
    }

    public void Init(
        Action<GameObject, T, PointerEventData> onPointerEnter = null,
        Action<GameObject, T, PointerEventData> onPointerClick = null,
        Action<GameObject, T, PointerEventData> onPointerExit = null,
        Action<GameObject, T, PointerEventData> onBeginDrag = null,
        Action<GameObject, T, PointerEventData> onDrag = null,
        Action<GameObject, T, PointerEventData> onEndDrag = null,
        Action<GameObject, T, PointerEventData> onHover = null,
        Action<GameObject, T> onDestroyed = null,
        Action<GameObject, T> onEnabled = null,
        Action<GameObject, T> onDisabled = null,
        Action<GameObject, T> onCreated = null,
        Action<GameObject, T> onRefreshed = null)
    {
        onPointerEnterEvent = onPointerEnter;
        onPointerClickEvent = onPointerClick;
        onPointerExitEvent = onPointerExit;
        onBeginDragEvent = onBeginDrag;
        onDragEvent = onDrag;
        onEndDragEvent = onEndDrag;
        onHoverEvent = onHover;
        onDestroyedEvent = onDestroyed;
        onEnabledEvent = onEnabled;
        onDisabledEvent = onDisabled;
        onCreatedEvent = onCreated;
        onRefreshedEvent = onRefreshed;
    }
    
    public List<UIElement> PopulateAll(GameObject gameObject, Transform parent, List<T> dataList)
    {
        int dataCount = dataList.Count;
        int childCount = parent.childCount;
        List<UIElement> result = new(dataCount);

        if (childCount == 1)
        {
            UIElement uiElement = parent.GetComponentInChildren<UIElement>();
            if (uiElement != null)
            {
                if (!isFirstCreatedDone)
                {
                    BindUIElementEvents(uiElement);
                    uiElement.Create(dataList[0]);
                    isFirstCreatedDone = true;
                }
                else
                {
                    uiElement.Refresh(dataList[0]);
                }
            }
            result.Add(uiElement);
        }
        
        for (int i = (childCount == 1) ? 1 : 0; i < Math.Min(dataCount, childCount); i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            child.SetActive(true);
            
            if (child.TryGetComponentInChildren(out UIElement uiElement))
            {
                uiElement.Refresh(dataList[i]);
            }
            
            result.Add(uiElement);
        }

        for (int i = childCount; i < dataCount; i++)
        {
            UIElement uiElement = Instantiate(gameObject, parent);
            BindUIElementEvents(uiElement);
            if (uiElement != null) uiElement.Create(dataList[i]);
            
            result.Add(uiElement);
        }

        for (int i = dataCount; i < childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }

        return result;
    }

    private UIElement Instantiate(GameObject gameObject, Transform parent)
    {
        GameObject instance = Object.Instantiate(gameObject, parent);

        if (!instance.TryGetComponentInChildren(out UIElement uiElement))
        {
            uiElement = instance.AddComponent<UIElement>();
        }

        return uiElement;
    }
    
    private void BindUIElementEvents(UIElement uiElement)
    {
        uiElement.Init(
            onPointerEnter: (go, eventData) => OnEvent(go, eventData, onPointerEnterEvent),
            onPointerClick: (go, eventData) => OnEvent(go, eventData, onPointerClickEvent),
            onPointerExit: (go, eventData) => OnEvent(go, eventData, onPointerExitEvent),
            onBeginDrag: (go, eventData) => OnEvent(go, eventData, onBeginDragEvent),
            onDrag: (go, eventData) => OnEvent(go, eventData, onDragEvent),
            onEndDrag: (go, eventData) => OnEvent(go, eventData, onEndDragEvent),
            onHover: (go, eventData) => OnEvent(go, eventData, onHoverEvent),
            onDestroyed: (go) =>
            {
                OnEvent(go, onDestroyedEvent);
                dataDic.Remove(go);
            },
            onEnabled: (go) => OnEvent(go, onEnabledEvent),
            onDisabled: (go) => OnEvent(go, onDisabledEvent),
            onCreated: (go, uiData) =>
            {
                dataDic[go] = uiData as T;
                OnEvent(go, uiData, onCreatedEvent);
            },
            onRefreshed: (go, uiData) =>
            {
                dataDic[go] = uiData as T;
                OnEvent(go, uiData, onRefreshedEvent);
            });
    }
    
    private void OnEvent(GameObject go, PointerEventData eventData, Action<GameObject, T, PointerEventData> action)
    {
        if (dataDic.TryGetValue(go, out T data))
            action?.Invoke(go, data, eventData);
    }

    private void OnEvent(GameObject go, Action<GameObject, T> action)
    {
        if (dataDic.TryGetValue(go, out T data))
            action?.Invoke(go, data);
    }
    
    private void OnEvent(GameObject go, UIData data, Action<GameObject, T> action)
    {
        action?.Invoke(go, data as T);
    }
}
