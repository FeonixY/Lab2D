using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Demo1UIData : UIData
{
    public List<ItemData> Items;
    public ItemData SelectedItem;

    public class ItemButtonData : UIData
    {
        public ItemData SelectedItem;
    }
}

public class Demo1HUD : CommonUIScreenBase
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject _itemButtonPrefab;

    [Header("UI Elements")]
    [SerializeField]
    private Image _itemPreviewImage;
    [SerializeField]
    private TMP_Text _itemDescription;
    [SerializeField]
    private Transform _itemButtonContainer;

    private Demo1UIData demo1UIData;
    private CommonUIElementFactory<Demo1UIData.ItemButtonData> itemButtonFactory;

    protected override void OnCreated(UIData data)
    {
        demo1UIData = data as Demo1UIData;
        if (demo1UIData == null) return;

        itemButtonFactory = new CommonUIElementFactory<Demo1UIData.ItemButtonData>(
            onPointerClick: (_, uiData, _) =>
            {
                _itemPreviewImage.sprite = uiData.SelectedItem.Preview;
                _itemDescription.SetText(uiData.SelectedItem.Description);
            });
        foreach (ItemData item in demo1UIData.Items)
        {
            Demo1UIData.ItemButtonData buttonData = new() { SelectedItem = item };
            itemButtonFactory.Instantiate(_itemButtonPrefab, _itemButtonContainer, buttonData, DefaultUIElements);
        }

        /*
        _itemPreviewImage.gameObject.Register<CommonUIElementFactory<Demo1UIData>, Demo1UIData>(onRefreshed: (_, uiData) =>
        {
            _itemPreviewImage.sprite = uiData.SelectedItem.Preview;
        }, elements: DefaultUIElements);
        _itemDescription.gameObject.Register<CommonUIElementFactory<Demo1UIData>, Demo1UIData>(onRefreshed: (_, uiData) =>
        {
            _itemDescription.SetText(uiData.SelectedItem.Description);
        }, elements: DefaultUIElements);
        */
    }
}

