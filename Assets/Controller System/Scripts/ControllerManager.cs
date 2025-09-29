using System;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : SingletonMonoBehaviour<ControllerManager>
{
    private readonly Dictionary<Type, Controller> controllers = new();
    private Controller currentController;

    private void Start()
    {
        foreach (Controller controller in controllers.Values)
        {
            controller.InitController();
        }
    }

    private void Update()
    {
        foreach (Controller controller in controllers.Values)
        {
            controller.OnAlwaysUpdate();
        }

        currentController?.OnUpdate();
    }

    private void LateUpdate() => currentController?.OnLateUpdate();

    public void Register(Controller controller) => controllers[controller.GetType()] = controller;

    public bool IsCurrentController<T>() where T : Controller
    {
        if (currentController == null)
            return false;

        return currentController.GetType() == typeof(T);
    }

    public void SwitchControllerTo<T>() where T : Controller
    {
        if (!controllers.TryGetValue(typeof(T), out Controller _))
        {
            Debug.LogWarning($"Controller not registered: {typeof(T)}");
            return;
        }

        if (currentController == null)
        {
            currentController = controllers[typeof(T)];
            currentController.OnGainControl();
            return;
        }
        
        currentController.OnLoseControl();
        currentController = controllers[typeof(T)];
        currentController.OnGainControl();
    }

    public void PopCurrentController()
    {
        currentController.OnLoseControl();
        currentController = null;
    }
}