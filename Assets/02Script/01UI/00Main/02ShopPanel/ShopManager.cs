using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GachaPanel gachaPanel;
    public CrystalPanel crystalPanel;
    public GoldPanel goldPanel;
    public ItemPanel itemPanel;
    public MileagePanel mileagePanel;

    public Button GachaTapButton;
    public Button CrystalTapButton;
    public Button GoldTapButton;
    public Button ItemTapButton;
    public Button MileageTapButton;

    public void Start()
    {
        GachaTapButton.onClick.AddListener(() => OnClickTapButton(gachaPanel.gameObject));
        CrystalTapButton.onClick.AddListener(() => OnClickTapButton(crystalPanel.gameObject));
        GoldTapButton.onClick.AddListener(() => OnClickTapButton(goldPanel.gameObject));
        ItemTapButton.onClick.AddListener(() => OnClickTapButton(itemPanel.gameObject));
        MileageTapButton.onClick.AddListener(() => OnClickTapButton(mileagePanel.gameObject));
    }

    private void OnClickTapButton(GameObject panel)
    {
        gachaPanel.gameObject.SetActive(false);
        crystalPanel.gameObject.SetActive(false);
        goldPanel.gameObject.SetActive(false);
        itemPanel.gameObject.SetActive(false);
        mileagePanel.gameObject.SetActive(false);

        panel.SetActive(true);    
    }
}
