using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UIElementExtensions
{
    public static void Register<TFactory, TData>(this GameObject gameObject, List<CommonUIElement> elements = null,
        Action<GameObject, TData, PointerEventData> onPointerEnter = null,
        Action<GameObject, TData, PointerEventData> onPointerClick = null,
        Action<GameObject, TData, PointerEventData> onPointerExit = null,
        Action<GameObject, TData, PointerEventData> onBeginDrag = null,
        Action<GameObject, TData, PointerEventData> onDrag = null,
        Action<GameObject, TData, PointerEventData> onEndDrag = null,
        Action<GameObject, TData> onCreated = null,
        Action<GameObject, TData> onDestroyed = null,
        Action<GameObject, TData> onEnabled = null,
        Action<GameObject, TData> onDisabled = null,
        Action<GameObject, TData> onRefreshed = null)
        where TFactory : CommonUIElementFactory<TData>, new()
        where TData : UIData
    {
        TFactory factory = new();
        factory.Init(
            onPointerEnter: onPointerEnter,
            onPointerClick: onPointerClick,
            onPointerExit: onPointerExit,
            onBeginDrag: onBeginDrag,
            onDrag: onDrag,
            onEndDrag: onEndDrag,
            onCreated: onCreated,
            onDestroyed: onDestroyed,
            onEnabled: onEnabled,
            onDisabled: onDisabled,
            onRefreshed: onRefreshed);
        factory.Register(gameObject, elements: elements);
        
    }
}
