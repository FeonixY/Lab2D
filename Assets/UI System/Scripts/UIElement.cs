using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElement : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Action<GameObject, PointerEventData> onPointerEnterEvent;
    private Action<GameObject, PointerEventData> onPointerClickEvent;
    private Action<GameObject, PointerEventData> onPointerExitEvent;
    private Action<GameObject, PointerEventData> onBeginDragEvent;
    private Action<GameObject, PointerEventData> onDragEvent;
    private Action<GameObject, PointerEventData> onEndDragEvent;
    private Action<GameObject, PointerEventData> onHoverEvent;
    private Action<GameObject> onDestroyedEvent;
    private Action<GameObject> onEnabledEvent;
    private Action<GameObject> onDisabledEvent;
    private Action<GameObject, UIData> onCreatedEvent;
    private Action<GameObject, UIData> onRefreshedEvent;

    private Coroutine hoverCoroutine;
    
    public void Init(
        Action<GameObject, PointerEventData> onPointerEnter = null,
        Action<GameObject, PointerEventData> onPointerClick = null,
        Action<GameObject, PointerEventData> onPointerExit = null,
        Action<GameObject, PointerEventData> onBeginDrag = null,
        Action<GameObject, PointerEventData> onDrag = null,
        Action<GameObject, PointerEventData> onEndDrag = null,
        Action<GameObject, PointerEventData> onHover = null,
        Action<GameObject> onDestroyed = null,
        Action<GameObject> onEnabled = null,
        Action<GameObject> onDisabled = null,
        Action<GameObject, UIData> onCreated = null,
        Action<GameObject, UIData> onRefreshed = null)
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
    
    protected virtual void OnDestroy()
    {
        onDestroyedEvent?.Invoke(gameObject);
        
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
        
        onPointerEnterEvent = null;
        onPointerClickEvent = null;
        onPointerExitEvent = null;
        onBeginDragEvent = null;
        onDragEvent = null;
        onEndDragEvent = null;
        onHoverEvent = null;
        onDestroyedEvent = null;
        onEnabledEvent = null;
        onDisabledEvent = null;
        onCreatedEvent = null;
        onRefreshedEvent = null;
    }

    protected virtual void OnEnable() => onEnabledEvent?.Invoke(gameObject);

    protected virtual void OnDisable()
    {
        onDisabledEvent?.Invoke(gameObject);
        
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
    }

    public virtual void Refresh(UIData data) => onRefreshedEvent?.Invoke(gameObject, data);

    public virtual void Create(UIData data) => onCreatedEvent?.Invoke(gameObject, data);

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        hoverCoroutine ??= StartCoroutine(OnHover(eventData));
        onPointerEnterEvent?.Invoke(gameObject, eventData);
    }

    public virtual void OnPointerClick(PointerEventData eventData) => onPointerClickEvent?.Invoke(gameObject, eventData);

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
        onPointerExitEvent?.Invoke(gameObject, eventData);
    }
    
    protected virtual IEnumerator OnHover(PointerEventData eventData)
    {
        while (true)
        {
            onHoverEvent?.Invoke(gameObject, eventData);
            yield return null;
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData) => onBeginDragEvent?.Invoke(gameObject, eventData);

    public virtual void OnDrag(PointerEventData eventData) => onDragEvent?.Invoke(gameObject, eventData);

    public virtual void OnEndDrag(PointerEventData eventData) => onEndDragEvent?.Invoke(gameObject, eventData);
}
