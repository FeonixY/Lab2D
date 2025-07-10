using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommonUIElement : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected Action<GameObject, PointerEventData> OnPointerEnterEvent;
    protected Action<GameObject, PointerEventData> OnPointerClickEvent;
    protected Action<GameObject, PointerEventData> OnPointerExitEvent;
    protected Action<GameObject, PointerEventData> OnBeginDragEvent;
    protected Action<GameObject, PointerEventData> OnDragEvent;
    protected Action<GameObject, PointerEventData> OnEndDragEvent;
    protected Action<GameObject> OnCreatedEvent;
    protected Action<GameObject> OnDestroyedEvent;
    protected Action<GameObject> OnEnabledEvent;
    protected Action<GameObject> OnDisabledEvent;
    protected Action<GameObject, UIData> OnRefreshedEvent;
    
    public void Init(
        Action<GameObject, PointerEventData> onPointerEnter = null,
        Action<GameObject, PointerEventData> onPointerClick = null,
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
        OnPointerEnterEvent = onPointerEnter;
        OnPointerClickEvent = onPointerClick;
        OnPointerExitEvent = onPointerExit;
        OnBeginDragEvent = onBeginDrag;
        OnDragEvent = onDrag;
        OnEndDragEvent = onEndDrag;
        OnCreatedEvent = onCreated;
        OnDestroyedEvent = onDestroyed;
        OnEnabledEvent = onEnabled;
        OnDisabledEvent = onDisabled;
        OnRefreshedEvent = onRefreshed;
    }
    
    protected virtual void Awake() => OnCreatedEvent?.Invoke(gameObject);

    protected virtual void OnDestroy()
    {
        OnDestroyedEvent?.Invoke(gameObject);
        
        OnPointerEnterEvent = null;
        OnPointerClickEvent = null;
        OnPointerExitEvent = null;
        OnBeginDragEvent = null;
        OnDragEvent = null;
        OnEndDragEvent = null;
        OnCreatedEvent = null;
        OnDestroyedEvent = null;
        OnEnabledEvent = null;
        OnDisabledEvent = null;
        OnRefreshedEvent = null;
    }

    protected virtual void OnEnable() => OnEnabledEvent?.Invoke(gameObject);

    protected virtual void OnDisable() => OnDisabledEvent?.Invoke(gameObject);

    public virtual void Refresh(UIData data) => OnRefreshedEvent?.Invoke(gameObject, data);

    public virtual void OnPointerEnter(PointerEventData eventData) => OnPointerEnterEvent?.Invoke(gameObject, eventData);

    public virtual void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent?.Invoke(gameObject, eventData);

    public virtual void OnPointerExit(PointerEventData eventData) => OnPointerExitEvent?.Invoke(gameObject, eventData);

    public virtual void OnBeginDrag(PointerEventData eventData) => OnBeginDragEvent?.Invoke(gameObject, eventData);

    public virtual void OnDrag(PointerEventData eventData) => OnDragEvent?.Invoke(gameObject, eventData);

    public virtual void OnEndDrag(PointerEventData eventData) => OnEndDragEvent?.Invoke(gameObject, eventData);
}
