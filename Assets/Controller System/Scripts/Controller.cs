using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    private void Awake() => ControllerManager.Instance.Register(this);

    public virtual void InitController() { }
    public virtual void OnAlwaysUpdate() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnGainControl() { }
    public virtual void OnLoseControl() { }
}
