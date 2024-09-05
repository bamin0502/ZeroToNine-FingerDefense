using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public enum ScreenMode
    {
        FULL_SCREEN_FIXED_WIDTH,
        ASPECT_RATIO,
        SAFE_AREA_FIXED_WIDTH
    }
    private Camera mainCamera;

    [SerializeField] private ScreenMode currentScreenMode;
    private ScreenMode prevScreenMode;
    public bool IsScreenModeChange
    {
        get => prevScreenMode != currentScreenMode;
    }

    [SerializeField] private float targetWidth = 20f;
    [SerializeField] private float targetHeight = 10f;
    public float bottomY = -5f;
    [Header("몬스터 드래그 줌 아웃 / 줌 인")]
    [SerializeField] private float zoomOutWidth = 25f;
    [SerializeField] private float zoomTime = 0.25f;

    [Header("레터박스")]
    [Tooltip("Left, Right, Bottom, Top 순으로 넣어주세요.")]
    [SerializeField] private List<Image> letterBoxes;
    [SerializeField] private RectTransform letterBoxCanvasRect;

    public float currentWidth { get; private set; }

    private float startWidth;
    private float endWidth;
    private float changeDuration;
    private float timer = 0f;
    private bool isWidthChange = false;

    private float currentScreenWidth;
    private float currentScreenHeight;
    public bool IsResolutionChange
    {
        get => currentScreenHeight != Screen.height
            || currentScreenWidth != Screen.width;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        currentWidth = targetWidth;
        
        prevScreenMode = currentScreenMode;
        currentScreenHeight = Screen.height;
        currentScreenWidth = Screen.width;

        ChangeResolution();
    }

    private void Update()
    {
        if (isWidthChange)
            ChangeTargetWidth();

        if (IsResolutionChange || IsScreenModeChange)
        {
            ChangeResolution();
        }
    }

    private void LateUpdate()
    {
        prevScreenMode = currentScreenMode;
        currentScreenHeight = Screen.height;
        currentScreenWidth = Screen.width;
    }

    private void ChangeResolution()
    {
        if (Screen.height > Screen.width) // 강제 세로모드에 대한 대응
        {
            AdjustCameraUsingLetterBox();
            return;
        }

        switch (currentScreenMode)
        {
            case ScreenMode.FULL_SCREEN_FIXED_WIDTH:
                AdjustCamera();
                break;
            case ScreenMode.ASPECT_RATIO:
                AdjustCameraUsingLetterBox();
                break;
            case ScreenMode.SAFE_AREA_FIXED_WIDTH:
                AdjustCameraForSafeArea();
                break;
        }
    }

    private void AdjustCamera()
    {
        var normalRect = new Rect(0f, 0f, 1f, 1f);
        mainCamera.rect = normalRect;
        SetLetterBoxInactive();

        var aspectRatio = (float)Screen.width / (float)Screen.height;
        var orthographicSize = currentWidth / (aspectRatio * 2f);
        mainCamera.orthographicSize = orthographicSize;

        var cameraPositionY = bottomY + orthographicSize;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, cameraPositionY, mainCamera.transform.position.z);
    }

    private void AdjustCameraForSafeArea()
    {
        var safeAreaRect = Screen.safeArea;
        var cameraRect = safeAreaRect;
        cameraRect.x /= Screen.width;
        cameraRect.y /= Screen.height;
        cameraRect.width /= Screen.width;
        cameraRect.height /= Screen.height;

        mainCamera.rect = cameraRect;

        var aspectRatio = (float)safeAreaRect.width / (float)safeAreaRect.height;
        var orthographicSize = currentWidth / (aspectRatio * 2f);
        mainCamera.orthographicSize = orthographicSize;

        var cameraPositionY = bottomY + orthographicSize;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, cameraPositionY, mainCamera.transform.position.z);

        SetLetterBoxSize(cameraRect);
    }

    private void AdjustCameraUsingLetterBox()
    {
        float screenAspectRatio = (float)Screen.width / (float)Screen.height;
        float targetAspectRatio = currentWidth / targetHeight;

        if (screenAspectRatio >= targetAspectRatio)
        {
            // 좌우 레터박스
            float viewportWidth = targetAspectRatio / screenAspectRatio;
            var adjustedRect = new Rect((1f - viewportWidth) / 2f, 0f, viewportWidth, 1f);
            mainCamera.rect = adjustedRect;
            // SetLetterBoxSize((1f - viewportWidth) / 2f, 1f, true);
            SetLetterBoxSize(adjustedRect);
        }
        else
        {
            // 상하 레터박스
            float viewportHeight = screenAspectRatio / targetAspectRatio;
            var adjustedRect = new Rect(0f, (1f - viewportHeight) / 2f, 1f, viewportHeight);
            mainCamera.rect = adjustedRect;
            // SetLetterBoxSize(1f, (1f - viewportHeight) / 2f, false);
            SetLetterBoxSize(adjustedRect);
        }

        var orthographicSize = targetHeight / 2f;
        mainCamera.orthographicSize = orthographicSize;
        var cameraPositionY = bottomY + orthographicSize;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, cameraPositionY, mainCamera.transform.position.z);
    }

    private void SetLetterBoxSize(float widthPercentage, float heightPercentage, bool isLeftRight)
    {
        float newWidth = letterBoxCanvasRect.rect.width * widthPercentage;
        float newHeight = letterBoxCanvasRect.rect.height * heightPercentage;

        foreach (var letterBox in letterBoxes)
        {
            letterBox.gameObject.SetActive(false);
        }

        for (int i = 0; i < 2; i++)
        {
            var index = isLeftRight ? i : i + 2;
            letterBoxes[index].gameObject.SetActive(true);
            letterBoxes[index].rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
    }

    private void SetLetterBoxSize(Rect rect)
    {
        float newLeftWidth = letterBoxCanvasRect.rect.width * rect.x;
        float newRightWidth = letterBoxCanvasRect.rect.width * (1f - rect.x - rect.width);
        float newBottomHeight = letterBoxCanvasRect.rect.height * rect.y;
        float newTopHeight = letterBoxCanvasRect.rect.height * (1f - rect.y - rect.height);

        // To-Do: 추후 수정
        letterBoxes[0].gameObject.SetActive(true);
        letterBoxes[0].rectTransform.sizeDelta = new Vector2(newLeftWidth, letterBoxCanvasRect.rect.height);
        letterBoxes[1].gameObject.SetActive(true);
        letterBoxes[1].rectTransform.sizeDelta = new Vector2(newRightWidth, letterBoxCanvasRect.rect.height);

        letterBoxes[2].gameObject.SetActive(true);
        letterBoxes[2].rectTransform.sizeDelta = new Vector2(letterBoxCanvasRect.rect.width, newBottomHeight);
        letterBoxes[3].gameObject.SetActive(true);
        letterBoxes[3].rectTransform.sizeDelta = new Vector2(letterBoxCanvasRect.rect.width, newTopHeight);
    }

    private void SetLetterBoxInactive()
    {
        foreach (var letterBox in letterBoxes)
        {
            letterBox.gameObject.SetActive(false);
        }
    }

    private void SetRect(RectTransform rectTransform, Rect rect)
    {
        rectTransform.anchorMin = new Vector2(rect.x, rect.y);
        rectTransform.anchorMax = new Vector2(rect.x + rect.width, rect.y + rect.height);
    }

    public static void SetLetterBoxRect(RectTransform rectTransform)
    {
        var rect = Camera.main.rect;

        rectTransform.anchorMin = new Vector2(rect.x, rect.y);
        rectTransform.anchorMax = new Vector2(rect.x + rect.width, rect.y + rect.height);
    }

    public void ZoomOutCamera()
    {
        SetTargetWidth(zoomOutWidth, zoomTime);
    }

    public void ResetCamera()
    {
        SetTargetWidth(targetWidth, zoomTime);
    }

    public void SetTargetWidth(float target, float duration)
    {
        if (target < 0f || duration < 0f)
            return;

        startWidth = currentWidth;
        endWidth = target;
        changeDuration = duration;
        timer = 0f;

        isWidthChange = true;
    }

    private void ChangeTargetWidth()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < changeDuration)
        {
            currentWidth = Mathf.Lerp(startWidth, endWidth, timer / changeDuration);
        }
        else
        {
            currentWidth = endWidth;
            isWidthChange = false;
        }

        ChangeResolution();
    }
}