using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ModalWindow : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI bodyText;
    public Button ButtonPrefab;
    public Transform buttonParent;

    private StringTable stringTable;

    private void OnEnable()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    public static void Create(Action<ModalWindow> onCreated)
    {
        Addressables.LoadAssetAsync<GameObject>("Prefab/08Main/ModalWindow").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var prefab = handle.Result;
                var window = Instantiate(prefab).GetComponent<ModalWindow>();
                var canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    window.transform.SetParent(canvas.transform, false);
                    window.gameObject.tag = "Modal";
                }
                else
                {
                    Logger.LogError("Canvas not found in the scene. Make sure there is a Canvas in the scene.");
                }
                onCreated?.Invoke(window);
            }
            else
            {
                Logger.LogError("Failed to load Modal Window prefab.");
            }
        };
    }

    // ID로 헤더 설정
    public ModalWindow SetHeader(int headerId)
    {
        headerText.text = stringTable.Get(headerId.ToString());
        return this;
    }

    // 직접 문자열로 헤더 설정
    public ModalWindow SetHeader(string header)
    {
        headerText.text = header;
        return this;
    }

    // ID로 본문 설정
    public ModalWindow SetBody(int bodyId)
    {
        bodyText.text = stringTable.Get(bodyId.ToString());
        return this;
    }

    // 직접 문자열로 본문 설정
    public ModalWindow SetBody(string body)
    {
        bodyText.text = body;
        return this;
    }

    // ID로 버튼 텍스트 설정
    public ModalWindow AddButton(int buttonId, Action onClick)
    {
        var button = Instantiate(ButtonPrefab, buttonParent);
        button.GetComponentInChildren<TextMeshProUGUI>().text = stringTable.Get(buttonId.ToString());
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            Destroy(gameObject);
        });
        return this;
    }

    // 직접 문자열로 버튼 텍스트 설정
    public ModalWindow AddButton(string buttonText, Action onClick)
    {
        var button = Instantiate(ButtonPrefab, buttonParent);
        button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            Destroy(gameObject);
        });
        return this;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
