using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class CommonUIElementFactory<T> : IUIElementFactory<T> where T : UIData
{
    private Action<GameObject, T, PointerEventData> onPointerEnterEvent;
    private Action<GameObject, T, PointerEventData> onPointerClickEvent;
    private Action<GameObject, T, PointerEventData> onPointerExitEvent;
    private Action<GameObject, T, PointerEventData> onBeginDragEvent;
    private Action<GameObject, T, PointerEventData> onDragEvent;
    private Action<GameObject, T, PointerEventData> onEndDragEvent;
    private Action<GameObject, T> onCreatedEvent;
    private Action<GameObject, T> onDestroyedEvent;
    private Action<GameObject, T> onEnabledEvent;
    private Action<GameObject, T> onDisabledEvent;
    private Action<GameObject, T> onRefreshedEvent;
    private readonly Dictionary<GameObject, T> dataDic = new();

    public CommonUIElementFactory(
        Action<GameObject, T, PointerEventData> onPointerEnter = null,
        Action<GameObject, T, PointerEventData> onPointerClick = null,
        Action<GameObject, T, PointerEventData> onPointerExit = null,
        Action<GameObject, T, PointerEventData> onBeginDrag = null,
        Action<GameObject, T, PointerEventData> onDrag = null,
        Action<GameObject, T, PointerEventData> onEndDrag = null,
        Action<GameObject, T> onCreated = null,
        Action<GameObject, T> onDestroyed = null,
        Action<GameObject, T> onEnabled = null,
        Action<GameObject, T> onDisabled = null,
        Action<GameObject, T> onRefreshed = null)
    {
        onPointerEnterEvent = onPointerEnter;
        onPointerClickEvent = onPointerClick;
        onPointerExitEvent = onPointerExit;
        onBeginDragEvent = onBeginDrag;
        onDragEvent = onDrag;
        onEndDragEvent = onEndDrag;
        onCreatedEvent = onCreated;
        onDestroyedEvent = onDestroyed;
        onEnabledEvent = onEnabled;
        onDisabledEvent = onDisabled;
        onRefreshedEvent = onRefreshed;
    }

    public CommonUIElementFactory() { }

    public void Init(
        Action<GameObject, T, PointerEventData> onPointerEnter = null,
        Action<GameObject, T, PointerEventData> onPointerClick = null,
        Action<GameObject, T, PointerEventData> onPointerExit = null,
        Action<GameObject, T, PointerEventData> onBeginDrag = null,
        Action<GameObject, T, PointerEventData> onDrag = null,
        Action<GameObject, T, PointerEventData> onEndDrag = null,
        Action<GameObject, T> onCreated = null,
        Action<GameObject, T> onDestroyed = null,
        Action<GameObject, T> onEnabled = null,
        Action<GameObject, T> onDisabled = null,
        Action<GameObject, T> onRefreshed = null)
    {
        onPointerEnterEvent = onPointerEnter;
        onPointerClickEvent = onPointerClick;
        onPointerExitEvent = onPointerExit;
        onBeginDragEvent = onBeginDrag;
        onDragEvent = onDrag;
        onEndDragEvent = onEndDrag;
        onCreatedEvent = onCreated;
        onDestroyedEvent = onDestroyed;
        onEnabledEvent = onEnabled;
        onDisabledEvent = onDisabled;
        onRefreshedEvent = onRefreshed;
    }

    public CommonUIElement Register(GameObject gameObject, T data = null, List<CommonUIElement> elements = null)
    {
        CommonUIElement uiElement = gameObject.AddComponent<CommonUIElement>();
        if (data != null) dataDic[gameObject] = data;

        uiElement.Init(
            onPointerEnter: (go, eventData) => OnEvent(go, eventData, onPointerEnterEvent),
            onPointerClick: (go, eventData) => OnEvent(go, eventData, onPointerClickEvent),
            onPointerExit: (go, eventData) => OnEvent(go, eventData, onPointerExitEvent),
            onBeginDrag: (go, eventData) => OnEvent(go, eventData, onBeginDragEvent),
            onDrag: (go, eventData) => OnEvent(go, eventData, onDragEvent),
            onEndDrag: (go, eventData) => OnEvent(go, eventData, onEndDragEvent),
            onCreated: (go) => OnEvent(go, onCreatedEvent),
            onDestroyed: (go) =>
            {
                OnEvent(go, onDestroyedEvent);
                elements?.Remove(uiElement);
            },
            onEnabled: (go) => OnEvent(go, onEnabledEvent),
            onDisabled: (go) => OnEvent(go, onDisabledEvent),
            onRefreshed: (go, uiData) => OnRefreshEvent(go, uiData, onRefreshedEvent));
        elements?.Add(uiElement);

        return uiElement;
    }

    public CommonUIElement Instantiate(GameObject gameObject, Transform parent, T data, List<CommonUIElement> elements = null)
    {
        GameObject instance = Object.Instantiate(gameObject, parent);
        
        CommonUIElement uiElement = instance.AddComponent<CommonUIElement>();

        if (data != null) dataDic[instance] = data;

        uiElement.Init(
            onPointerEnter: (go, eventData) => OnEvent(go, eventData, onPointerEnterEvent),
            onPointerClick: (go, eventData) => OnEvent(go, eventData, onPointerClickEvent),
            onPointerExit: (go, eventData) => OnEvent(go, eventData, onPointerExitEvent),
            onBeginDrag: (go, eventData) => OnEvent(go, eventData, onBeginDragEvent),
            onDrag: (go, eventData) => OnEvent(go, eventData, onDragEvent),
            onEndDrag: (go, eventData) => OnEvent(go, eventData, onEndDragEvent),
            onCreated: (go) => OnEvent(go, onCreatedEvent),
            onDestroyed: (go) =>
            {
                OnEvent(go, onDestroyedEvent);
                elements?.Remove(uiElement);
            },
            onEnabled: (go) => OnEvent(go, onEnabledEvent),
            onDisabled: (go) => OnEvent(go, onDisabledEvent),
            onRefreshed: (go, uiData) => OnRefreshEvent(go, uiData, onRefreshedEvent));
        elements?.Add(uiElement);

        return uiElement;
    }
    
    private void OnEvent(GameObject go, PointerEventData eventData, Action<GameObject, T, PointerEventData> action)
    {
        if (dataDic.TryGetValue(go, out T data))
        {
            action?.Invoke(go, data, eventData);
        }
    }

    private void OnEvent(GameObject go, Action<GameObject, T> action)
    {
        if (dataDic.TryGetValue(go, out T data))
            action?.Invoke(go, data);
    }
    
    private void OnRefreshEvent(GameObject go, UIData data, Action<GameObject, T> action)
    {
        action?.Invoke(go, data as T);
    }
}
