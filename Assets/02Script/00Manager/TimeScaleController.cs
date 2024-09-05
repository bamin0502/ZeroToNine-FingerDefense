using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    private static float startTimeScale;
    private static float targetTimeScale;
    private static float changeDuration;
    private static float timer = 0f;

    private static bool isTimeScaleChange = false;

    private void Update()
    {
        if (isTimeScaleChange)
            ChangeTimeScale();
    }

    public static void ChangeTimeSclae(float target, float duration)
    {
        if (target < 0f || duration < 0f)
            return;
            
        targetTimeScale = target;
        startTimeScale = Time.timeScale;
        changeDuration = duration;
        timer = 0f;

        isTimeScaleChange = true;
    }

    public static void SetTimeScale(float target)
    {
        Time.timeScale = target;
        isTimeScaleChange = false;
    }
    
    public static float GetTimeScale()
    {
        return Time.timeScale;
    }

    private void ChangeTimeScale()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < changeDuration)
        {
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, timer / changeDuration);
        }
        else
        {
            Time.timeScale = targetTimeScale;
            isTimeScaleChange = false;
        }
    }
}
