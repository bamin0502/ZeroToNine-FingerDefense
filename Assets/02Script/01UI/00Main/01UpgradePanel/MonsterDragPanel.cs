using UnityEngine;
using UnityEngine.UI;

public class MonsterDragPanel : MonoBehaviour
{
    private UpgradeTable upgradeTable;
    private AssetListTable assetListTable;
    private StringTable stringTable;

    public DragInfoSlot dragInfoSlotPrefab;
    public RectTransform[] dragInfoParent;
    public Button[] stageButton;

    private void Awake()
    {
        upgradeTable ??= DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);
        assetListTable ??= DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    private void Start()
    {
        CreateSlots();
        AssignButtonEvents();
    }

    private void AssignButtonEvents()
    {
        for (var i = 0; i < stageButton.Length; i++)
        {
            var index = i;
            stageButton[index].onClick.AddListener(() => ShowDragInfoParent(index));
        }
    }

    private void ShowDragInfoParent(int index)
    {
        foreach (var parent in dragInfoParent)
        {
            parent.gameObject.SetActive(false);
        }

        if (index >= 0 && index < dragInfoParent.Length)
        {
            dragInfoParent[index].gameObject.SetActive(true);
        }
    }

    private void CreateSlots()
    {
        var parentIndex = 0;
        var slotCount = 0;

        foreach (var upgradeData in upgradeTable.upgradeTable.Values)
        {
            if (upgradeData.Type == 0)
            {
                var slot = Instantiate(dragInfoSlotPrefab, dragInfoParent[parentIndex]);

                // 현재 UpgradeResultId에 대한 MonsterDragLevel을 가져옵니다.
                var dragLevel = GameManager.instance.GameData.MonsterDragLevel[upgradeData.UpgradeResultId];

                slot.SetupSlot(upgradeData.UpgradePrice, dragLevel, upgradeData.UpgradeResultId);

                string assetName = assetListTable.Get(upgradeData.AssetNo);
                if (!string.IsNullOrEmpty(assetName))
                {
                    string assetPath = $"Prefab/10UpgradeUI/{assetName}";
                    GameObject assetObject = Resources.Load<GameObject>(assetPath);

                    if (assetObject != null)
                    {
                        var monster=Instantiate(assetObject, slot.transform);
                        monster.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        Debug.LogWarning($"AssetName {assetName}에 해당하는 오브젝트를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning($"AssetNo {upgradeData.AssetNo}에 해당하는 AssetName을 찾을 수 없습니다.");
                }

                slot.UpdateLockState(dragLevel == (int)GameData.MonsterDrag.LOCK);
                slot.UpdateCostColor();
                slot.transform.SetParent(dragInfoParent[parentIndex], false);

                slotCount++;
                if (slotCount % 2 == 0)
                {
                    parentIndex++;
                }

                if (parentIndex >= dragInfoParent.Length)
                {
                    break;
                }
            }
        }
    }


}