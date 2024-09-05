using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DownLoadManager : MonoBehaviour
{
    [Header("UI")] public GameObject waitMessage;
    public GameObject downloadMessage;
    public GameObject connectFailMessage;
    public Slider downSlider;
    public TextMeshProUGUI sizeInfoText;
    public TextMeshProUGUI downValText;
    public Button LoadingButton;
    public Button QuitButton;
    public Button DownLoadButton;

    [Header("Label")] public AssetLabelReference defaultLabel;

    [Header("Assets")] [SerializeField] private AssetReference[] assetReferences;

    private List<GameObject> gameObjects = new List<GameObject>();
    private long patchSize;
    private long totalDownloadedSize;
    private Dictionary<string, int> patchMap = new Dictionary<string, int>();
    private const int maxRetries = 3; // 최대 재시도 횟수

    private void Start()
    {
        connectFailMessage.SetActive(false);
        downloadMessage.SetActive(false);
        waitMessage.SetActive(true);
        downSlider.gameObject.SetActive(false);
        LoadingButton.interactable = false;
        LoadingButton.onClick.AddListener(LoadGameScene);
        DownLoadButton.onClick.AddListener(ButtonDownloadWrapper);
        QuitButton.onClick.AddListener(Application.Quit);
        InitAddressable().Forget();
    }

    private async UniTask InitAddressable()
    {
        try
        {
            await RetryPolicy(async () =>
            {
                if (!await IsInternetAvailable())
                {
                    throw new Exception("인터넷 연결 끊김");
                }

                var init = Addressables.InitializeAsync();
                await init;
            });
            await CheckUpdateFiles();
        }
        catch
        {
            ShowConnectionFailedMessage();
        }
    }

    #region UpdateCheck

    private async UniTask CheckUpdateFiles()
    {
        var labels = new List<string>() { defaultLabel.labelString };
        patchSize = 0;

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            await handle;
            patchSize += handle.Result;
        }

        if (patchSize > 0)
        {
            await UniTask.Delay(2000);
            waitMessage.SetActive(false);
            downloadMessage.SetActive(true);
            downValText.text = GetFileSize(patchSize);
            downSlider.gameObject.SetActive(false);
            sizeInfoText.text = "업데이트 용량을 확인하고 있습니다.";
        }
        else
        {
            downSlider.gameObject.SetActive(false);
            waitMessage.SetActive(true);
            sizeInfoText.text = "화면을 터치해 게임을 시작해주세요";
            LoadingButton.interactable = true;
        }
    }

    private string GetFileSize(long byteCnt)
    {
        var size = "0 bytes";
        if (byteCnt >= 1073741824.0)
            size = $"{byteCnt / 1073741824.0:##.##} GB";
        else if (byteCnt >= 1048576.0)
            size = $"{byteCnt / 1048576.0:##.##} MB";
        else if (byteCnt >= 1024.0)
            size = $"{byteCnt / 1024.0:##.##} KB";
        else if (byteCnt > 0 && byteCnt < 1024.0)
            size = byteCnt.ToString() + " bytes";

        return size;
    }

    #endregion

    #region DownLoad

    private void ButtonDownloadWrapper()
    {
        ButtonDownload().Forget();
    }

    private async UniTaskVoid ButtonDownload()
    {
        try
        {
            await RetryPolicy(async () =>
            {
                if (!await IsInternetAvailable())
                {
                    throw new Exception("인터넷 연결 끊김");
                }
            });

            waitMessage.SetActive(true);
            downloadMessage.SetActive(false);
            sizeInfoText.text = "패치 파일을 다운로드 중입니다...";
            downSlider.gameObject.SetActive(true);

            await RecheckPatchFiles();
        }
        catch
        {
            ShowConnectionFailedMessage();
        }
    }

    private async UniTask RecheckPatchFiles()
    {
        var labels = new List<string>() { defaultLabel.labelString };
        patchSize = 0;
        totalDownloadedSize = 0;
        patchMap.Clear();

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            await handle;
            patchSize += handle.Result;
        }

        if (patchSize > 0)
        {
            await PatchFiles();
        }
        else
        {
            downloadMessage.SetActive(false);
            downSlider.gameObject.SetActive(false);
            waitMessage.SetActive(true);
            sizeInfoText.text = "화면을 터치해 게임을 시작해주세요.";
            LoadingButton.interactable = true;
        }
    }

    private async UniTask PatchFiles()
    {
        var labels = new List<string>() { defaultLabel.labelString };

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            await handle;

            if (handle.Result != 0)
            {
                Download(label).Forget();
            }
        }

        await CheckDownload();
    }

    private async UniTask CheckDownload()
    {
        sizeInfoText.text = "0%";

        while (totalDownloadedSize < patchSize)
        {
            try
            {
                await RetryPolicy(async () =>
                {
                    if (!await IsInternetAvailable())
                    {
                        throw new Exception("인터넷 연결 끊김");
                    }
                });

                downSlider.value = (float)totalDownloadedSize / patchSize;
                sizeInfoText.text = $"{downSlider.value * 100:##}%";
                await UniTask.DelayFrame(1);
            }
            catch
            {
                ShowConnectionFailedMessage();
                return;
            }
        }

        sizeInfoText.text = "다운로드 완료";
        downSlider.value = 1f;
        LoadingButton.interactable = true;
        await UniTask.Delay(1000);
        downSlider.gameObject.SetActive(false);
        sizeInfoText.text = "화면을 터치해 게임을 시작해주세요";
        LoadAssets();
    }

    private async UniTaskVoid Download(string label)
    {
        var handle = Addressables.DownloadDependenciesAsync(label, false);

        try
        {
            await RetryPolicy(async () =>
            {
                while (!handle.IsDone)
                {
                    if (!await IsInternetAvailable())
                    {
                        throw new Exception("인터넷 연결 끊김");
                    }

                    var status = handle.GetDownloadStatus();
                    patchMap[label] = (int)status.DownloadedBytes;
                    totalDownloadedSize = patchMap.Values.Sum();
                    await UniTask.DelayFrame(1);
                }

                var finalStatus = handle.GetDownloadStatus();
                patchMap[label] = (int)finalStatus.TotalBytes;
                totalDownloadedSize = patchMap.Values.Sum();
            });
        }
        catch
        {
            ShowConnectionFailedMessage();
            return;
        }

        Addressables.Release(handle);
    }

    #endregion


    private void ShowConnectionFailedMessage()
    {
        downloadMessage.SetActive(false);
        waitMessage.SetActive(false);
        connectFailMessage.SetActive(true);
    }

    private async UniTask<bool> IsInternetAvailable()
    {
        try
        {
            using (var request = UnityWebRequest.Head("https://www.google.com"))
            {
                request.timeout = 5;
                await request.SendWebRequest();
                return request.responseCode == 200;
            }
        }
        catch
        {
            return false;
        }
    }

    private async UniTask RetryPolicy(Func<UniTask> action)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await action();
                return;
            }
            catch
            {
                await UniTask.Delay(1000);
            }
        }

        throw new Exception("재시도 횟수 초과");
    }

    private void LoadAssets()
    {
        foreach (var t in assetReferences)
        {
            if (t == null) continue;

            // 적절한 타입을 로드하도록 결정
            if (t.RuntimeKey.ToString().EndsWith(".playable"))
            {
                var load = t.LoadAssetAsync<PlayableAsset>();
                load.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        var asset = op.Result;
                        Debug.Log($"Loaded PlayableAsset: {asset.name}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to load PlayableAsset {t.RuntimeKey}");
                    }

                    Addressables.Release(load);
                };
            }
            else if (t.RuntimeKey.ToString().EndsWith(".prefab"))
            {
                var load = t.LoadAssetAsync<GameObject>();
                load.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        var asset = op.Result;
                        var inst = Addressables.InstantiateAsync(t);
                        inst.Completed += (op2) =>
                        {
                            if (op2.Status == AsyncOperationStatus.Succeeded)
                            {
                                gameObjects.Add(op2.Result);
                            }
                            else
                            {
                                Debug.LogError($"Failed to instantiate asset {t.RuntimeKey}");
                            }
                        };
                    }
                    else
                    {
                        Debug.LogError($"Failed to load GameObject {t.RuntimeKey}");
                    }

                    Addressables.Release(load);
                };
            }
            else if (t.RuntimeKey.ToString().EndsWith(".inputactions"))
            {
                var load = t.LoadAssetAsync<InputAction>();
                load.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        var asset = op.Result;
                        Debug.Log($"Loaded InputAction: {asset.name}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to load InputAction {t.RuntimeKey}");
                    }

                    Addressables.Release(load);
                };
            }

            // 필요한 경우 다른 자산 유형에 대해 else-if 블록을 추가
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
}