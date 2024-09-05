using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterUpgradeSlotUI : MonoBehaviour
{
    public Sprite gradeImage;
    
    public Sprite[] elementImages;
    
    public RectTransform elementParent;
    public RectTransform gradeParent;
    public RectTransform classParent;
    
    public GameObject ChoicePanel;
    public Button ChoiceButton;
    public Image SkillIcon;
    
    public delegate void SlotClickDelegate(CharacterUpgradeSlotUI slot);
    public SlotClickDelegate OnSlotClick;
    
    public PlayerCharacterData characterData { get; private set; }
    
    private AssetListTable assetListTable;
    private SkillTable skillTable;
    public TextMeshProUGUI upgradeLevelText;

    public TextMeshProUGUI powerText;
    
    private void Awake()
    {
        assetListTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        skillTable = DataTableManager.Get<SkillTable>(DataTableIds.Skill);
    }
    
    public void SetCharacterSlot(PlayerCharacterData characterData)
    {
        this.characterData = characterData;
        
        if(assetListTable == null)
        {
            return;
        }
        
        var assetName = assetListTable.Get(characterData.AssetNo);
        if (!string.IsNullOrEmpty(assetName))
        {
            Addressables.LoadAssetAsync<GameObject>($"Prefab/00CharacterUI/{assetName}").Completed += OnCharacterPrefabLoaded;
        }

        for (var i = 0; i < characterData.Grade+1; i++)
        {
            var gradeInstance = new GameObject("GradeImage").AddComponent<Image>();
            gradeInstance.sprite = gradeImage;
            gradeInstance.transform.SetParent(gradeParent, false);
        }

        var elementImage = elementParent.GetComponent<Image>();
        if (elementImage != null && characterData.Element >= 0 && characterData.Element < elementImages.Length)
        {
            elementImage.sprite = elementImages[characterData.Element];
            elementImage.gameObject.SetActive(true);
            
        }
        upgradeLevelText.text = $"+ {characterData.Plus}";
        
        var skillId = assetListTable.Get(characterData.SkillIcon);
        if (!string.IsNullOrEmpty(skillId))
        {
            Addressables.LoadAssetAsync<Sprite>($"Prefab/09SkillIcon/{skillId}").Completed += OnSkillIconLoaded;
        }

        powerText.text = $"{characterData.Power}";
        powerText.gameObject.transform.SetAsLastSibling();

        ChoicePanel.transform.SetAsLastSibling();
        ChoiceButton.onClick.AddListener(OnClick);
    }

    private void OnCharacterPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            var spineInstance = Instantiate(obj.Result, classParent);
            spineInstance.transform.localPosition = Vector3.zero;
            spineInstance.transform.SetAsFirstSibling();
        }
        else
        {
            Debug.LogWarning($"Prefab/00CharacterUI/{assetListTable.Get(characterData.AssetNo)} could not be loaded.");
        }
    }

    private void OnSkillIconLoaded(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            SkillIcon.sprite = obj.Result;
            SkillIcon.gameObject.SetActive(true);
            
        }
        else
        {
            Debug.LogWarning($"Prefab/09SkillIcon/{assetListTable.Get(characterData.SkillIcon)} could not be loaded.");
        }
    }

    private void OnClick()
    {
        OnSlotClick?.Invoke(this);
    }
}
