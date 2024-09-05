using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private static string nextScene;

    public Slider loadingBar;

    private void Start()
    {
        StartLoadingScene().Forget();
    }

    private async UniTask StartLoadingScene()
    {
        await UniTask.DelayFrame(1);
        var op= SceneManager.LoadSceneAsync(nextScene);
        op!.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            await UniTask.DelayFrame(1);
            
            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                loadingBar.value = Mathf.Lerp(loadingBar.value, op.progress, timer);
                
                if(loadingBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                loadingBar.value = Mathf.Lerp(loadingBar.value, 1f, timer);
                
                if (Mathf.Approximately(loadingBar.value, 1f))
                {
                    await UniTask.Delay(2000);
                    op.allowSceneActivation = true;
                    await UniTask.Yield();
                }
            }
        }

    }
    
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadSceneAsync(nextScene);
    }
}
