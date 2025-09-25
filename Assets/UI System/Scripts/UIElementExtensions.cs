using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class UIElementExtensions
{
    public static void Refresh<T>(GameObject prefab, Transform parent, List<T> data, Action<T, GameObject> refreshAction = null)
    {
        int dataCount = data.Count;
        int childCount = parent.childCount;

        for (int i = 0; i < Math.Min(dataCount, childCount); i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            child.SetActive(true);
            if (child.TryGetComponent(out UIElement uiElement) && data[i] is UIData uiData)
            {
                uiElement.Refresh(uiData);
            }
            else
            {
                refreshAction?.Invoke(data[i], child);
            }
        }
        
        for (int i = childCount; i < dataCount; i++)
        {
            GameObject newChild = Object.Instantiate(prefab, parent);
            if (newChild.TryGetComponent(out UIElement uiElement) && data[i] is UIData uiData)
            {
                uiElement.Refresh(uiData);
            }
            else
            {
                refreshAction?.Invoke(data[i], newChild);
            }
        }
        
        for (int i = dataCount; i < childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static void PopulateAll(GameObject prefab, Transform parent, int number)
    {
        int childCount = parent.childCount;
        
        for (int i = 0; i < Math.Min(number, childCount); i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            child.SetActive(true);
        }
        
        for (int i = childCount; i < number; i++)
        {
            Object.Instantiate(prefab, parent);
        }
        
        for (int i = number; i < childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.GetComponentInChildren<T>();
        if (component != null) return true;

        return false;
    }
}
