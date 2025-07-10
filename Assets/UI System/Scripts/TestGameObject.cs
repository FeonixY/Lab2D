using UnityEngine;

public class TestGameObject : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.TryOpenScreen<Demo1HUD>(new Demo1UIData
        {
            Items = ItemManager.Instance.Items,
            SelectedItem = ItemManager.Instance.Items[0]
        });
    }
}
