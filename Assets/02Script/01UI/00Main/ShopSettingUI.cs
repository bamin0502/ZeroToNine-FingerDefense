using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShopSettingUI : MonoBehaviour, IResourceObserver
{
    public GameObject MileageWindow;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI ticketText;

    public TextMeshProUGUI[] diamondPayTexts;
    public TextMeshProUGUI[] goldPayTexts;
    public TextMeshProUGUI[] itemTitleTexts;
    public TextMeshProUGUI[] ticketPayTexts;
    public TextMeshProUGUI[] mileagePayTexts;
    public TextMeshProUGUI[] diamondBonusTexts;
    public TextMeshProUGUI[] goldBonusTexts;

    public TextMeshProUGUI[] tapMenuTexts;

    public Image[] diamontPayImages;
    public Image[] goldPayImages;
    public Image[] itemImages;

    private StringTable shopTable;
    private AssetListTable assetTable;
    private GameManager gameManager;

    private bool dataLoaded = false;

    private void OnEnable()
    {
        if (!dataLoaded)
        {
            LoadData();
            dataLoaded = true;
        }
    }

    private void Start()
    {
        gameManager = GameObject.FindWithTag("Manager").TryGetComponent(out GameManager manager) ? manager : null;
        if (gameManager == null)
        {
            Logger.LogError("GameManager is not initialized.");
            return;
        }
        gameManager.GameData.RegisterObserver(this);
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        goldText.text = gameManager.GameData.Gold.ToString();
        diamondText.text = gameManager.GameData.Diamond.ToString();
        ticketText.text = gameManager.GameData.Ticket.ToString();
        
        UpdateTicketOrMileageText();
    }
    
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.GameData.RemoveObserver(this);
        }
    }

    public void OnResourceUpdate(ResourceType resourceType, int newValue)
    {
        switch (resourceType)
        {
            case ResourceType.Gold:
                goldText.text = newValue.ToString();
                break;
            case ResourceType.Diamond:
                diamondText.text = newValue.ToString();
                break;
            case ResourceType.Ticket:
            case ResourceType.Mileage:
                UpdateTicketOrMileageText();
                break;
        }
    }

    private void UpdateTicketOrMileageText()
    {
        ticketText.text = MileageWindow.activeSelf ? gameManager.GameData.Mileage.ToString() : gameManager.GameData.Ticket.ToString();
    }

    private void LoadData()
    {
        shopTable = DataTableManager.Get<StringTable>(DataTableIds.String);
        assetTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);

        if (shopTable == null)
        {
            Logger.LogError("ShopTable is null");
            return;
        }

        SetTextElements();
    }

    private void SetImage(string id, Image image, string category)
    {
        string path = $"Prefab/{category}/{assetTable.Get(Convert.ToInt32(id))}";
        Addressables.LoadAssetAsync<Sprite>(path).Completed += (AsyncOperationHandle<Sprite> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                image.sprite = handle.Result;
            }
            else
            {
                Logger.LogWarning($"Image not found for ID: {id} in category: {category}");
            }
        };
    }

    private void SetTextElements()
    {
        tapMenuTexts[0].text = shopTable.Get(90331.ToString());
        tapMenuTexts[1].text = shopTable.Get(90591.ToString());
        tapMenuTexts[2].text = shopTable.Get(90601.ToString());
        tapMenuTexts[4].text = shopTable.Get(90611.ToString());

        diamondPayTexts[0].text = shopTable.Get(90051.ToString());
        diamondPayTexts[1].text = shopTable.Get(90061.ToString());
        diamondPayTexts[2].text = shopTable.Get(90081.ToString());
        diamondPayTexts[3].text = shopTable.Get(90101.ToString());
        diamondPayTexts[4].text = shopTable.Get(90121.ToString());
        diamondPayTexts[5].text = shopTable.Get(90141.ToString());

        goldPayTexts[0].text = shopTable.Get(90161.ToString());
        goldPayTexts[1].text = shopTable.Get(90171.ToString());
        goldPayTexts[2].text = shopTable.Get(90181.ToString());
        goldPayTexts[3].text = shopTable.Get(90191.ToString());
        goldPayTexts[4].text = shopTable.Get(90201.ToString());
        goldPayTexts[5].text = shopTable.Get(90211.ToString());

        itemTitleTexts[0].text = shopTable.Get(90221.ToString());
        itemTitleTexts[1].text = shopTable.Get(90231.ToString());
        itemTitleTexts[2].text = shopTable.Get(90241.ToString());
        itemTitleTexts[3].text = shopTable.Get(90251.ToString());
        itemTitleTexts[4].text = shopTable.Get(90261.ToString());
        itemTitleTexts[5].text = shopTable.Get(90271.ToString());
        itemTitleTexts[6].text = shopTable.Get(90281.ToString());
        itemTitleTexts[7].text = shopTable.Get(90291.ToString());
        itemTitleTexts[8].text = shopTable.Get(90301.ToString());
        itemTitleTexts[9].text = shopTable.Get(90311.ToString());
        itemTitleTexts[10].text = shopTable.Get(90321.ToString());

        diamondBonusTexts[1].text = shopTable.Get(90071.ToString());
        diamondBonusTexts[2].text = shopTable.Get(90091.ToString());
        diamondBonusTexts[3].text = shopTable.Get(90111.ToString());
        diamondBonusTexts[4].text = shopTable.Get(90131.ToString());
        diamondBonusTexts[5].text = shopTable.Get(90151.ToString());

        for (var i = 0; i < diamontPayImages.Length; i++)
        {
            SetImage((605 + i).ToString(), diamontPayImages[i], "06ShopIcon");
        }

        for (var i = 0; i < goldPayImages.Length; i++)
        {
            SetImage((611 + i).ToString(), goldPayImages[i], "06ShopIcon");
        }

        for (var i = 0; i < itemImages.Length; i++)
        {
            SetImage((617 + i).ToString(), itemImages[i], "07GameItem");
        }
    }
}
