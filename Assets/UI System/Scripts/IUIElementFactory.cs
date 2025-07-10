using System.Collections.Generic;
using UnityEngine;

public interface IUIElementFactory<in T> where T : UIData
{
    public CommonUIElement Register(GameObject gameObject, T data, List<CommonUIElement> elements = null);
    
    public CommonUIElement Instantiate(GameObject gameObject ,Transform parent, T data, List<CommonUIElement> elements = null);
}
