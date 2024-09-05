using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiSlotButton : MonoBehaviour
{
    public Button button;
    public Image backgroundBlack;
    public Image slotImage;
    public TextMeshProUGUI text;

    public UiButtonEffect buttonEffect;

    private void Awake()
    {
        buttonEffect = GetComponentInChildren<UiButtonEffect>();
        if (buttonEffect != null)
        {
            buttonEffect.ButtonRectTransform = GetComponent<RectTransform>();
            buttonEffect.gameObject.SetActive(false);
        }
    }

    public void SetFillAmountBackground(float amount)
    {
        amount = Mathf.Clamp01(amount);
        if (!backgroundBlack)
            return;
            
        backgroundBlack.fillAmount = amount;

        if (amount == 0f)
            button.interactable = true;
        else if (amount == 1f)
            button.interactable = false;
    }

    public void ActiveButton(bool isActive)
    {
        SetFillAmountBackground(isActive ? 0f : 1f);
    }
}
