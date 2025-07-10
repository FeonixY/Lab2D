using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public Sprite Preview;

    public ItemData(string name, string description, Sprite icon, Sprite preview)
    {
        Name = name;
        Description = description;
        Icon = icon;
        Preview = preview;
    }
}
