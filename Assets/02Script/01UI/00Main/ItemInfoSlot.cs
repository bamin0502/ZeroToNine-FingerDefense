using TMPro;
using UnityEngine;

public class ItemInfoSlot : MonoBehaviour
{
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI ItemTypeText;
    public TextMeshProUGUI ItemDescriptionText;
    
    private StringTable stringTable;
    private ItemTable itemTable;
    
    private void Awake()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
        itemTable ??= DataTableManager.Get<ItemTable>(DataTableIds.Item);
    }
    
    private void OnEnable()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
        itemTable ??= DataTableManager.Get<ItemTable>(DataTableIds.Item);
        
    }
    public void SetItemInfoSlot(ItemData Data)
    {
        if (Data == null || itemTable == null || stringTable == null)
        {
            return;
        }
        
        var itemData = itemTable.Get(Data.Id);
        if (itemData == null)
        {
            Logger.LogError($"ItemData with ID {Data.Id} not found in ItemTable.");
            return;
        }
        
        // 아이템 이름 설정
        ItemNameText.text = stringTable.Get(itemData.NameId.ToString());

        // 아이템 타입에 따른 문자열 설정
        string itemTypeString = itemData.ItemType switch
        {
            1 => stringTable.Get("92011"),
            2 => stringTable.Get("92021"),
            _ => stringTable.Get(itemData.ItemType.ToString())
        };

        ItemTypeText.text = itemTypeString;

        // 아이템 설명 설정
        ItemDescriptionText.text = stringTable.Get(itemData.DescId.ToString());
    }
}
