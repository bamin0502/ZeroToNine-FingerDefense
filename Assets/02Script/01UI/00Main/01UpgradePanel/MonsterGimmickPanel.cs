using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MonsterGimmickPanel : MonoBehaviour
{
    private UpgradeTable upgradeTable;
    private StringTable stringTable;
    private AssetListTable assetListTable;

    public Button[] GimmickRangeUpgradeButtons; // UpStatType = 0
    public Button[] GimmickDamageUpgradeButtons; // UpStatType = 1
    public Button[] GimmickDurationUpgradeButtons; // UpStatType = 2
    
    public TextMeshProUGUI GimmickRangeLevelText;
    public TextMeshProUGUI GimmickDamageLevelText;
    public TextMeshProUGUI GimmickDurationLevelText;
    
    public GameObject[] GimmickRangeLockImages;
    public GameObject[] GimmickDamageLockImages;
    public GameObject[] GimmickDurationLockImages;
    
    private void Awake()
    {
        upgradeTable ??= DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
        assetListTable ??= DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
    }

    private void Start()
    {
        SetupGimmickUpgradeButtons();
        GimmickRangeLevelText.text = stringTable.Get(99942.ToString());
        GimmickDamageLevelText.text = stringTable.Get(99952.ToString());
        GimmickDurationLevelText.text = stringTable.Get(99962.ToString());
    }

    private void SetupGimmickUpgradeButtons()
    {
        foreach (var upgradeData in upgradeTable.upgradeTable.Values)
        {
            if (upgradeData.Type == 1)
            {
                string assetName = assetListTable.Get(upgradeData.AssetNo);

                if (!string.IsNullOrEmpty(assetName))
                {
                    string assetPath = $"Prefab/10UpgradeUI/{assetName}";
                    Addressables.LoadAssetAsync<Sprite>(assetPath).Completed += (AsyncOperationHandle<Sprite> handle) =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            Sprite sprite = handle.Result;

                            switch (upgradeData.UpStatType)
                            {
                                case 0:
                                    SetupButtonGroup(GimmickRangeUpgradeButtons, upgradeData, sprite, new int[] { 99501, 99511, 99521, 99531, 99541 }, new int[] { 99502, 99512, 99522, 99532, 99542 });
                                    break;
                                case 1:
                                    SetupButtonGroup(GimmickDamageUpgradeButtons, upgradeData, sprite, new int[] { 99551,99561,99571,99581,99591 }, new int[] { 99552,99562,99572,99582,99592 });
                                    break;
                                case 2:
                                    SetupButtonGroup(GimmickDurationUpgradeButtons, upgradeData, sprite, new int[] { 99601,99611,99621,99631,99641 }, new int[] { 99602,99612,99622,99632,99642 });
                                    break;
                                default:
                                    Debug.LogWarning($"알 수 없는 UpStatType: {upgradeData.UpStatType}");
                                    break;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"AssetNo {upgradeData.AssetNo}에 해당하는 이미지를 Addressables에서 찾을 수 없습니다.");
                        }
                    };
                }
                else
                {
                    Debug.LogWarning($"AssetNo {upgradeData.AssetNo}에 해당하는 AssetName을 찾을 수 없습니다.");
                }
            }
        }

        // 현재 업그레이드 레벨에 따른 잠금 이미지 처리
        UpdateLockImages(GimmickRangeUpgradeButtons, GimmickRangeLockImages, 0);
        UpdateLockImages(GimmickDamageUpgradeButtons, GimmickDamageLockImages, 1);
        UpdateLockImages(GimmickDurationUpgradeButtons, GimmickDurationLockImages, 2);
    }

    private void SetupButtonGroup(Button[] buttons, UpgradeData upgradeData, Sprite sprite, int[] headerStringIDs, int[] bodyStringIDs)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            button.image.sprite = sprite;
            
            string headerText = stringTable.Get(headerStringIDs[i].ToString());
            string bodyText = stringTable.Get(bodyStringIDs[i].ToString());
            
            // 각 버튼에 대해 해당 레벨에 맞는 UpgradeData를 찾아 설정
            int targetLevel = i + 1;
            UpgradeData targetUpgradeData = FindUpgradeDataByLevel(upgradeData.UpStatType, targetLevel);

            if (targetUpgradeData != null)
            {
                button.onClick.RemoveAllListeners(); // 중복 이벤트 방지를 위해 기존 리스너 제거
                button.onClick.AddListener(() =>
                {
                    int currentLevel = GameManager.instance.GameData.MonsterGimmickLevel
                        .Find(x => x.monsterGimmick == upgradeData.UpStatType).level;

                    if (targetLevel == currentLevel + 1)
                    {
                        TryUpgradeGimmick(targetUpgradeData, headerText, bodyText);
                    }
                    else if (targetLevel <= currentLevel)
                    {
                        ModalWindow.Create(window =>
                        {
                            window.SetHeader("이미 업그레이드 완료")
                                .SetBody("이 업그레이드는 이미 완료되었습니다.")
                                .AddButton("확인", () => { })
                                .Show();
                        });
                    }
                    else
                    {
                        ModalWindow.Create(window =>
                        {
                            window.SetHeader("업그레이드 필요")
                                .SetBody("이 업그레이드를 진행하려면 이전 업그레이드를 먼저 완료해 주세요.")
                                .AddButton("확인", () => { })
                                .Show();
                        });
                    }
                });
            }
        }
    }

    private void UpdateLockImages(Button[] buttons, GameObject[] lockImages, int upStatType)
    {
        var playerUpgradeLevel = GameManager.instance.GameData.MonsterGimmickLevel
            .Find(x => x.monsterGimmick == upStatType);

        int currentLevel = playerUpgradeLevel.level;

        for (int i = 0; i < lockImages.Length; i++)
        {
            // 현재 레벨 이하의 버튼들은 잠금 해제
            if (i < currentLevel)
            {
                lockImages[i].SetActive(false);
            }
            else
            {
                lockImages[i].SetActive(true);
            }
        }
    }

    private UpgradeData FindUpgradeDataByLevel(int upStatType, int targetLevel)
    {
        foreach (var upgradeData in upgradeTable.upgradeTable.Values)
        {
            if (upgradeData.UpStatType == upStatType && upgradeData.Level == targetLevel)
            {
                return upgradeData;
            }
        }
        return null;
    }

    private void TryUpgradeGimmick(UpgradeData upgradeData, string headerText, string bodyText)
    {
        int playerGold = GameManager.instance.GameData.Gold;
        int stageClearNum = GameManager.instance.GameData.StageClearNum;

        // 현재 버튼에 해당하는 업그레이드 가격과 스테이지 조건을 검사
        if (playerGold >= upgradeData.UpgradePrice && stageClearNum >= upgradeData.NeedClearStage)
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader(headerText)
                    .SetBody(bodyText)
                    .AddButton("확인", () =>
                    {
                        GameManager.instance.GameData.Gold -= upgradeData.UpgradePrice;
                        ApplyGimmickUpgrade(upgradeData);
                    })
                    .AddButton("취소", () => { })
                    .Show();
            });
        }
        else if (playerGold < upgradeData.UpgradePrice)
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("구매 실패")
                    .SetBody("골드가 부족합니다.")
                    .AddButton("확인", () => { })
                    .Show();
            });
        }
        else if (stageClearNum < upgradeData.NeedClearStage)
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("스테이지 클리어 필요")
                    .SetBody($"이 업그레이드를 구매하려면 스테이지 {upgradeData.NeedClearStage-13000}를 클리어해야 합니다.")
                    .AddButton("확인", () => { })
                    .Show();
            });
        }
    }

    private void ApplyGimmickUpgrade(UpgradeData upgradeData)
    {
        switch (upgradeData.UpStatType)
        {
            case 0: // ATTACK RANGE
                GameManager.instance.GameData.UpdateMonsterGimmickLevel((int)GameData.MonsterGimmick.ATTACKRANGE, upgradeData.Level);
                break;
            case 1: // ATTACK DAMAGE
                GameManager.instance.GameData.UpdateMonsterGimmickLevel((int)GameData.MonsterGimmick.ATTACKDAMAGE, upgradeData.Level);
                break;
            case 2: // ATTACK DURATION
                GameManager.instance.GameData.UpdateMonsterGimmickLevel((int)GameData.MonsterGimmick.ATTACKDURATION, upgradeData.Level);
                break;
            default:
                Debug.LogWarning($"알 수 없는 UpStatType: {upgradeData.UpStatType}");
                break;
        }

        // 업그레이드 후 데이터 저장
        DataManager.SaveFile(GameManager.instance.GameData);

        // UI 갱신 로직 추가
        UpdateUIAfterGimmickUpgrade(upgradeData);
    }

    private void UpdateUIAfterGimmickUpgrade(UpgradeData upgradeData)
    {
        // 업그레이드 후 UI 갱신
        UpdateLockImages(GimmickRangeUpgradeButtons, GimmickRangeLockImages, 0);
        UpdateLockImages(GimmickDamageUpgradeButtons, GimmickDamageLockImages, 1);
        UpdateLockImages(GimmickDurationUpgradeButtons, GimmickDurationLockImages, 2);

        switch (upgradeData.UpStatType)
        {
            case 0:
                // 추가적인 UI 갱신 로직이 필요한 경우 여기에 작성
                break;
            case 1:
                // 추가적인 UI 갱신 로직이 필요한 경우 여기에 작성
                break;
            case 2:
                // 추가적인 UI 갱신 로직이 필요한 경우 여기에 작성
                break;
            default:
                Debug.LogWarning($"알 수 없는 UpStatType: {upgradeData.UpStatType}");
                break;
        }
    }
}
