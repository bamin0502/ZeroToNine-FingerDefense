using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class NickSettingUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button confirmButton;
    public GameObject nickCheckUI;
    public TextMeshProUGUI nickNameText;
    public Button cancelButton;
    public Button confirmNickButton;

    [HideInInspector]
    public bool isComplete = false;
    public MainUI mainUI;
    private GameManager gameManager;
    private ResourceManager resourceManager;
    private readonly Regex koreanFullSyllablesRegex = new("^[가-힣a-zA-Z0-9]*$");
    private string userId;

    public TextMeshProUGUI noticeText;
    private Vector3 noticeTextInitialPosition;

    private void Start()
    {
        gameManager = GameManager.instance;
        resourceManager=GameObject.FindWithTag("Resources").TryGetComponent(out ResourceManager manager) ? manager : null;
        
        noticeTextInitialPosition = noticeText.transform.localPosition;
    }

    private void OnEnable()
    {
        confirmNickButton.onClick.AddListener(OnClickConfirmNick);
        confirmButton.onClick.AddListener(OnClickConfirm);
        cancelButton.onClick.AddListener(OnClickCancel);
        inputField.onSelect.AddListener(OnInputFieldSelected);
    }

    private void OnInputFieldSelected(string text)
    {
        inputField.ActivateInputField();
    }
    
    private void OnClickConfirm()
    {
        userId = inputField.text;
        if (userId.Length is < 2 or > 8)
        {
            ShowNotice("닉네임은 2자 이상 8자 이하로 입력해주세요.");
            Logger.Log("닉네임은 2자 이상 8자 이하로 입력해주세요.");
            return;
        }
        if (!koreanFullSyllablesRegex.IsMatch(userId))
        {
            ShowNotice("닉네임은 한글 음절, 영어, 숫자만 입력 가능합니다.");
            Logger.Log("닉네임은 한글 음절, 영어, 숫자만 입력 가능합니다.");
            return;
        }
        
        gameManager.ResourceManager.PlayerName = inputField.text;
        
        nickCheckUI.SetActive(true);
        nickNameText.text = $"정말 <color=#FF0000>{gameManager.ResourceManager.PlayerName}</color>으로 설정하시겠습니까?";
        
        Logger.Log($"{gameManager.ResourceManager.PlayerName}으로 설정되었습니다.");
    }

    private void OnClickCancel()
    {
        if (gameManager == null || gameManager.ResourceManager == null)
        {
            Debug.LogError("GameManager or ResourceManager is not initialized.");
            return;
        }

        nickCheckUI.SetActive(false);
        gameManager.ResourceManager.PlayerName = "";
        Logger.Log($"{gameManager.ResourceManager.PlayerName}으로 설정되었습니다.");
    }

    private void OnClickConfirmNick()
    {
        gameObject.SetActive(false);
        isComplete = true;
        
        Logger.Log("닉네임 설정이 완료되었습니다.");
    }
    
    private void ShowNotice(string message)
    {
        noticeText.text = message;
        noticeText.color = new Color(noticeText.color.r, noticeText.color.g, noticeText.color.b, 1);
        noticeText.transform.localPosition = noticeTextInitialPosition;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1.5f);
        sequence.Append(noticeText.DOFade(0, 1f));
        sequence.Join(noticeText.transform.DOLocalMoveY(noticeTextInitialPosition.y + 50, 1f).SetEase(Ease.InQuad));
        sequence.Play();
    }
}
