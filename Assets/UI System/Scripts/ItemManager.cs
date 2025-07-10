using System.Collections.Generic;

public class ItemManager : MonoSingleton<ItemManager>
{
    public List<ItemData> Items = new();
}
