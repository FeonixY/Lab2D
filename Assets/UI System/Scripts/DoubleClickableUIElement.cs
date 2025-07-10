using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickableUIElement : CommonUIElement
{
    public float DoubleClickTimeThreshold = 0.05f;
    
    protected Action<GameObject, PointerEventData> OnPointerDoubleClickEvent;
    
    private float lastClickTime;
    
    public void Init(
        Action<GameObject, PointerEventData> onPointerEnter = null,
        Action<GameObject, PointerEventData> onPointerClick = null,
        Action<GameObject, PointerEventData> onPointerDoubleClick = null,
        Action<GameObject, PointerEventData> onPointerExit = null,
        Action<GameObject, PointerEventData> onBeginDrag = null,
        Action<GameObject, PointerEventData> onDrag = null,
        Action<GameObject, PointerEventData> onEndDrag = null,
        Action<GameObject> onCreated = null,
        Action<GameObject> onDestroyed = null,
        Action<GameObject> onEnabled = null,
        Action<GameObject> onDisabled = null,
        Action<GameObject, UIData> onRefreshed = null)
    {
        OnPointerEnterEvent += onPointerEnter;
        OnPointerClickEvent += onPointerClick;
        OnPointerDoubleClickEvent += onPointerDoubleClick;
        OnPointerExitEvent += onPointerExit;
        OnBeginDragEvent += onBeginDrag;
        OnDragEvent += onDrag;
        OnEndDragEvent += onEndDrag;
        OnCreatedEvent += onCreated;
        OnDestroyedEvent += onDestroyed;
        OnEnabledEvent += onEnabled;
        OnDisabledEvent += onDisabled;
        OnRefreshedEvent += onRefreshed;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        
        float time = Time.unscaledTime;
        if (time - lastClickTime < DoubleClickTimeThreshold)
        {
            OnPointerDoubleClickEvent?.Invoke(gameObject, eventData);
            lastClickTime = 0f;
        }
        else
        {
            lastClickTime = time;
        }
    }
}
