using System;
using UnityEngine.EventSystems;

public class LongPressableUIElement : CommonUIElement
{
    public float LongPressTimeThreshold = 0.5f;
    
    private float pressStartTime;
    
    private Action<PointerEventData> OnPointerLongPress;
}
