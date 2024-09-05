using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSlotController : MonoBehaviour,IResourceObserver
{
    public RectTransform itemSlotParent; // 빈 슬롯이 위치할 부모 객체
    public RectTransform itemSelectParent; // 아이템 슬롯이 위치할 부모 객체
    public ItemSlotUI itemSlotPrefab;

    private ItemTable itemTable;
    private AssetListTable assetListTable;

    public List<ItemSlotUI> itemSlots = new List<ItemSlotUI>(); // 아이템 슬롯 리스트
    private List<ItemSlotUI> emptySlots = new List<ItemSlotUI>(); // 빈 슬롯 리스트
    private HashSet<int> addedItems = new HashSet<int>(); // 추가된 아이템 ID를 관리

    public ItemInfoSlot itemInfoSlot;
    
    private void Awake()
    {
        itemTable = DataTableManager.Get<ItemTable>(DataTableIds.Item);
        assetListTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
    }

    private void OnEnable()
    {
        GameManager.instance.GameData.RegisterObserver(this);                          
    }
    
    private void OnDisable()
    {
        // 옵저버 해제
        GameManager.instance.GameData.RemoveObserver(this);
    }
    
    private void Start()
    {
        CheckAndProvideItemsForTutorial();
        RefreshItemSlots();   
    }

    private void CheckAndProvideItemsForTutorial()
    {
        if (!GameManager.instance.GameData.Game2TutorialCheck)
        {
            // 지급할 아이템 ID와 개수를 설정
            int itemIdToCheck = 8007; 
            int itemCountToProvide = 1;

            // 아이템이 이미 있는지 확인
            var gameItem = GameManager.instance.GameData.Items.FirstOrDefault(item => item.itemId == itemIdToCheck);
            if (gameItem.Equals(default((int itemId, int itemCount))))
            {
                // 아이템이 없으면 지급
                GameManager.instance.GameData.AddItem(itemIdToCheck, itemCountToProvide);
            }
        }
    }


    public void RefreshItemSlots()
    {
        foreach (var slot in emptySlots)
        {
            Destroy(slot.gameObject);
        }

        emptySlots.Clear();
        itemSlots.Clear();
        addedItems.Clear();
        
        var activeItemSlots = itemSelectParent.GetComponentsInChildren<ItemSlotUI>();
        foreach (var slot in activeItemSlots)
        {
            if (slot.ItemId != 0)
            {
                addedItems.Add(slot.ItemId);
                itemSlots.Add(slot);
            }
        }

        CreateEmptySlots();
        CreateItemSlots();
    }


    private void CreateEmptySlots()
    {
        for (var i = 0; i < 2; i++)
        {
            var emptySlot = Instantiate(itemSlotPrefab, itemSlotParent);
            _ = emptySlot.Setup(null, null, 0);
            emptySlot.onClickItemSlot = HandleEmptySlotClick;
            emptySlots.Add(emptySlot);
        }
    }


    private void CreateItemSlots()
    {
        if (itemTable == null || itemTable.table == null)
        {
            return;
        }

        if (assetListTable == null || assetListTable.table == null)
        {
            return;
        }

        var purchasedItems = GameManager.instance.GameData.Items;

        foreach (var purchasedItem in purchasedItems)
        {
            
            if (itemTable.table.TryGetValue(purchasedItem.itemId, out var itemData))
            {
                var itemSlot = itemSlots.FirstOrDefault(slot => slot.ItemId == purchasedItem.itemId);

                if (itemSlot != null)
                {
                    itemSlot.UpdateItemCount(purchasedItem.itemCount);
                }
                else if (assetListTable.table.TryGetValue(itemData.IconNo, out var assetPath))
                {
                    itemSlot = Instantiate(itemSlotPrefab, itemSelectParent);
                    _ = itemSlot.Setup(itemData, assetPath, purchasedItem.itemCount);
                    itemSlot.onClickItemSlot = HandleItemSlotClick;
                    itemSlot.OnLongPress = slot =>
                    {
                        itemInfoSlot.SetItemInfoSlot(itemData);
                        itemInfoSlot.gameObject.SetActive(true);
                    };

                    itemSlot.OnLongPressRelease = () =>
                    {
                        itemInfoSlot?.gameObject.SetActive(false);
                    };

                    itemSlots.Add(itemSlot);
                    addedItems.Add(purchasedItem.itemId);
                }
                else
                {
                    Logger.LogError($"Item ID {purchasedItem.itemId}에 대한 아이콘 경로를 찾을 수 없습니다.");
                }
            }
            else
            {
                Logger.LogError($"Item ID {purchasedItem.itemId}에 대한 데이터를 itemTable에서 찾을 수 없습니다.");
            }
        }
    }

    private void HandleItemSlotClick(ItemSlotUI clickedSlot)
    {
        if (clickedSlot == null || clickedSlot.ItemId == 0) return;

        var existingEmptySlot = emptySlots.FirstOrDefault(slot => slot.ItemId == clickedSlot.ItemId);
        if (existingEmptySlot != null)
        {
            return;
        }

        var emptySlot = emptySlots.FirstOrDefault(slot => slot.ItemId == 0);
        if (emptySlot == null)
        {
            return; 
        }

        int limit = clickedSlot.GetOriginalLimit();
        int currentCount = clickedSlot.GetItemCount();

        int addCount = Mathf.Min(currentCount, limit);

        emptySlot.SetItemSlot(clickedSlot.ItemId, clickedSlot.ItemSprite, addCount);

        currentCount -= addCount;
        clickedSlot.UpdateItemCount(currentCount);

        if (currentCount <= 0)
        {
            itemSlots.Remove(clickedSlot);
            addedItems.Remove(clickedSlot.ItemId);
            Destroy(clickedSlot.gameObject);
        }

        SaveItemToLoadTable(clickedSlot.ItemId, addCount);
    }


    private void HandleEmptySlotClick(ItemSlotUI clickedSlot)
    {
        if (clickedSlot == null || clickedSlot.ItemId == 0) return;

        var removeCount = clickedSlot.GetItemCount();

        var originalSlot = itemSlots.FirstOrDefault(slot => slot.ItemId == clickedSlot.ItemId);
        if (originalSlot != null)
        {
            var restoredCount = originalSlot.GetItemCount() + removeCount;
            originalSlot.UpdateItemCount(restoredCount);
        }
        else
        {
            if (itemTable.table.TryGetValue(clickedSlot.ItemId, out var itemData))
            {
                var newSlot = Instantiate(itemSlotPrefab, itemSelectParent);
                _ = newSlot.Setup(itemData, clickedSlot.ItemSprite.name, removeCount);
                newSlot.onClickItemSlot = HandleItemSlotClick;
                newSlot.OnLongPress = slot => itemInfoSlot.SetItemInfoSlot(itemData);
                newSlot.OnLongPressRelease = () => itemInfoSlot.gameObject.SetActive(false);
                itemSlots.Add(newSlot);
                addedItems.Add(clickedSlot.ItemId);
            }
            else
            {
                Logger.LogError($"ItemData for ItemId {clickedSlot.ItemId} not found.");
            }
        }
        
        RemoveItemFromLoadTable(clickedSlot.ItemId);
        clickedSlot.ClearSlot();
    }

    private void SaveItemToLoadTable(int itemId, int addCount)
    {
        var existingItem = Variables.LoadTable.ItemId.FirstOrDefault(item => item.itemId == itemId);
        if (existingItem != default)
        {
            Variables.LoadTable.ItemId.Remove(existingItem);
            addCount += existingItem.itemCount;
        }
        Variables.LoadTable.ItemId.Add((itemId, addCount));
    }

    private void RemoveItemFromLoadTable(int itemId)
    {
        var itemToRemove = Variables.LoadTable.ItemId.FirstOrDefault(item => item.itemId == itemId);
        if (itemToRemove != default)
        {
            Variables.LoadTable.ItemId.Remove(itemToRemove);
        }
    }
    
    public void OnStartButtonClick()
    {
        // 빈 슬롯에서 아이템 사용 처리
        foreach (var emptySlot in emptySlots)
        {
            if (emptySlot.ItemId == 0) continue;

            int usedCount = emptySlot.GetItemCount(); // 실제 사용한 아이템 개수
            if (usedCount > 0)
            {
                // 튜토리얼 완료 여부와 관계없이 아이템을 사용합니다.
                ApplyItemUsage(emptySlot.ItemId, usedCount);

                // 사용한 아이템을 LoadTable에서 업데이트
                UpdateItemInLoadTable(emptySlot.ItemId, usedCount);
                DataManager.SaveFile(GameManager.instance.GameData);

                // UI에서 아이템 개수를 업데이트 및 슬롯 초기화
                emptySlot.UpdateItemCount(0);
                emptySlot.ClearSlot(); // 사용 후 슬롯 초기화
            }
        }

        // 사용 후 아이템 슬롯을 새로고침
        RefreshItemSlots();
    }


    private void ApplyItemUsage(int itemId, int usedCount)
    {
        // 실제 게임 데이터에서 아이템 수량 감소 처리
        var gameItem = GameManager.instance.GameData.Items.FirstOrDefault(item => item.itemId == itemId);

        // 기본값과 비교하여 유효한 값인지 확인합니다.
        if (!gameItem.Equals(default((int itemId, int itemCount))))
        {
            var updatedItemCount = gameItem.itemCount - usedCount;

            if (updatedItemCount <= 0)
            {
                GameManager.instance.GameData.Items.Remove(gameItem);
            }
            else
            {
                // 아이템 수량을 업데이트합니다.
                var updatedItem = (gameItem.itemId, updatedItemCount);
                GameManager.instance.GameData.Items.Remove(gameItem);
                GameManager.instance.GameData.Items.Add(updatedItem);
            }
        }
    }

    private void UpdateItemInLoadTable(int itemId, int usedCount)
    {
        // LoadTable에서 사용된 아이템의 상태 업데이트
        var itemInLoadTable = Variables.LoadTable.ItemId.FirstOrDefault(item => item.itemId == itemId);
        if (itemInLoadTable != default)
        {
            Variables.LoadTable.ItemId.Remove(itemInLoadTable);
            Variables.LoadTable.ItemId.Add((itemId, usedCount));
            Logger.Log($"Updated item {itemId} in LoadTable with used count {usedCount}");
        }
        else
        {
            Variables.LoadTable.ItemId.Add((itemId, usedCount));
            Logger.Log($"Added new item {itemId} to LoadTable with count {usedCount}");
        }
    }

    public void UpdateSlotCount(int itemId, int newCount)
    {
        var itemSlot = itemSlots.FirstOrDefault(slot => slot.ItemId == itemId);

        if (itemSlot != null)
        {
            itemSlot.UpdateItemCount(newCount);
            Logger.Log($"Item ID {itemId} 슬롯의 카운트가 {newCount}로 업데이트되었습니다.");
        }
        else
        {
            Logger.LogWarning($"아이템 ID {itemId}에 해당하는 슬롯을 찾을 수 없습니다.");
        }
    }


    public void OnResourceUpdate(ResourceType resourceType, int newValue)
    {
        if (resourceType == ResourceType.ItemCount)
        {
            // ItemCount가 업데이트된 경우 해당 아이템의 ID와 수량을 전달받아 슬롯을 갱신
            int itemId = GameManager.instance.GameData.Items.FirstOrDefault(item => item.itemCount == newValue).itemId;
            UpdateSlotCount(itemId, newValue);
        }
    }
}