using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterFeaturePanel : MonoBehaviour
{
    private AssetListTable assetListTable;
    private UpgradeTable upgradeTable;
    private StringTable stringTable;

    public Button[] characterArrangementButtons; // UpStatType = 3
    public Button[] characterHpUpgradeButtons; // UpStatType = 4
    public Button[] characterEnhancedGradeButtons; // UpStatType = 5

    public TextMeshProUGUI characterArrangementLevelText;
    public TextMeshProUGUI characterHpLevelText;
    public TextMeshProUGUI characterEnhancedGradeLevelText;

    public GameObject[] characterArragementLockImages;
    public GameObject[] characterHpLockImages;
    public GameObject[] characterEnhancedGradeLockImages;

    private void Awake()
    {
        assetListTable ??= DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        upgradeTable ??= DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    private void Start()
    {
        SetupCharacterFeatureButtons();
        characterArrangementLevelText.text = stringTable.Get(99972.ToString());
        characterHpLevelText.text = stringTable.Get(99982.ToString());
        characterEnhancedGradeLevelText.text = stringTable.Get(99992.ToString());
    }

    private void SetupCharacterFeatureButtons()
    {
        foreach (var upgradeData in upgradeTable.upgradeTable.Values)
        {
            if (upgradeData.Type == 3)
            {
                string assetName = assetListTable.Get(upgradeData.AssetNo);
                LoadSpriteAsync(assetName, upgradeData);
            }
        }

        // 현재 업그레이드 레벨에 따른 잠금 이미지 처리
        UpdateLockImages(characterArrangementButtons, characterArragementLockImages, 3);
        UpdateLockImages(characterHpUpgradeButtons, characterHpLockImages, 4);
        UpdateLockImages(characterEnhancedGradeButtons, characterEnhancedGradeLockImages, 5);
    }

    private void LoadSpriteAsync(string assetName, UpgradeData upgradeData)
    {
        Addressables.LoadAssetAsync<Sprite>($"Prefab/10UpgradeUI/{assetName}").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Sprite sprite = handle.Result;

                switch (upgradeData.UpStatType)
                {
                    case 3:
                        AssignUpgradeDataToButton(characterArrangementButtons, upgradeData, sprite, new int[] { 99651,99661,99671,99681,99691 }, new int[] { 99652, 99662, 99672, 99682, 99692 });
                        break;
                    case 4:
                        AssignUpgradeDataToButton(characterHpUpgradeButtons, upgradeData, sprite, new int[] { 99701,99711,99721,99731,99741 }, new int[] { 99702, 99712, 99722, 99732, 99742 });
                        break;
                    case 5:
                        AssignUpgradeDataToButton(characterEnhancedGradeButtons, upgradeData, sprite, new int[] { 99751,99761,99771,99781,99791 }, new int[] { 99752, 99762, 99772, 99782, 99792 });
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

    private void AssignUpgradeDataToButton(Button[] buttons, UpgradeData upgradeData, Sprite sprite, int[] headerStringIDs, int[] bodyStringIDs)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (upgradeData.Level == i + 1) // 버튼 레벨과 UpgradeData의 레벨이 일치하는지 확인
            {
                var button = buttons[i];
                button.image.sprite = sprite;
                
                string headerText = stringTable.Get(headerStringIDs[i].ToString());
                string bodyText = stringTable.Get(bodyStringIDs[i].ToString());

                button.onClick.RemoveAllListeners(); // 중복 이벤트 방지를 위해 기존 리스너 제거
                button.onClick.AddListener(() =>
                {
                    var playerUpgradeLevel = GameManager.instance.GameData.PlayerUpgradeLevel
                        .Find(x => x.playerUpgrade == upgradeData.UpStatType);

                    int currentLevel = playerUpgradeLevel.level;

                    if (upgradeData.Level == currentLevel + 1)
                    {
                        TryUpgradeFeature(upgradeData, headerText, bodyText);
                    }
                    else if (upgradeData.Level <= currentLevel)
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
        var playerUpgradeLevel = GameManager.instance.GameData.PlayerUpgradeLevel
            .Find(x => x.playerUpgrade == upStatType);

        int currentLevel = playerUpgradeLevel.level;

        for (int i = 0; i < lockImages.Length; i++)
        {
            // 현재 레벨 이하의 버튼들은 잠금 해제
            lockImages[i].SetActive(i >= currentLevel);
            // 현재 레벨을 초과하는 버튼들은 잠금 상태로 유지
        }
    }

    private void TryUpgradeFeature(UpgradeData upgradeData,string header, string body)
    {
        int playerGold = GameManager.instance.GameData.Gold;
        int stageClearNum = GameManager.instance.GameData.StageClearNum;

        // 현재 버튼에 해당하는 업그레이드 가격과 스테이지 조건을 검사
        if (playerGold >= upgradeData.UpgradePrice && stageClearNum >= upgradeData.NeedClearStage)
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader(header)
                    .SetBody(body)
                    .AddButton("확인", () =>
                    {
                        GameManager.instance.GameData.Gold -= upgradeData.UpgradePrice;
                        ApplyUpgrade(upgradeData);
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
            var stageNum = upgradeData.NeedClearStage - 13000;
            ModalWindow.Create(window =>
            {
                window.SetHeader("스테이지 클리어 필요")
                    .SetBody($"이 업그레이드를 구매하려면 스테이지 {stageNum}를 클리어해야 합니다.")
                    .AddButton("확인", () => { })
                    .Show();
            });
        }
    }

    private void ApplyUpgrade(UpgradeData upgradeData)
    {
        switch (upgradeData.UpStatType)
        {
            case 3: // CHARACTER_ARRANGEMENT
                GameManager.instance.GameData.UpdatePlayerUpgradeLevel((int)GameData.PlayerUpgrade.CHARACTER_ARRANGEMENT, upgradeData.Level);
                break;
            case 4: // PLAYER_HEALTH
                GameManager.instance.GameData.UpdatePlayerUpgradeLevel((int)GameData.PlayerUpgrade.PLAYER_HEALTH, upgradeData.Level);
                break;
            case 5: // INCREASE_DRAG
                GameManager.instance.GameData.UpdatePlayerUpgradeLevel((int)GameData.PlayerUpgrade.INCREASE_DRAG, upgradeData.Level);
                break;
            default:
                Debug.LogWarning($"알 수 없는 UpStatType: {upgradeData.UpStatType}");
                break;
        }

        // 업그레이드 후 데이터 저장
        DataManager.SaveFile(GameManager.instance.GameData);

        // UI 갱신 로직 추가 (예: 버튼 비활성화 등)
        UpdateUIAfterUpgrade(upgradeData);
    }

    private void UpdateUIAfterUpgrade(UpgradeData upgradeData)
    {
        // 업그레이드 후 UI 갱신 로직 추가
        UpdateLockImages(characterArrangementButtons, characterArragementLockImages, 3);
        UpdateLockImages(characterHpUpgradeButtons, characterHpLockImages, 4);
        UpdateLockImages(characterEnhancedGradeButtons, characterEnhancedGradeLockImages, 5);

        switch (upgradeData.UpStatType)
        {
            case 3:
                // 추가적인 UI 갱신 로직이 필요한 경우 여기에 작성
                break;
            case 4:
                // 추가적인 UI 갱신 로직이 필요한 경우 여기에 작성
                break;
            case 5:
                // 추가적인 UI 갱신 로직이 필요한 경우 여기에 작성
                break;
            default:
                Debug.LogWarning($"알 수 없는 UpStatType: {upgradeData.UpStatType}");
                break;
        }
    }
}
