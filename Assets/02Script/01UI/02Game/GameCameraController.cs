using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameCameraController : MonoBehaviour
{
    public enum ScreenMode
    {
        FULL_SCREEN,
        ASPECT_RATIO,
        SAFE_AREA
    }
    private Camera mainCamera;

    [Header("스크린 모드")]
    [SerializeField] private ScreenMode defaultScreenMode;
    private ScreenMode currentScreenMode;
    private ScreenMode prevScreenMode;
    public bool IsScreenModeChange
    {
        get => prevScreenMode != currentScreenMode;
    }

    [Header("타켓 값")]
    public float targetWidth = 20f;
    [SerializeField] private float targetHeight = 20f;
    public float bottomY;

    [Header("한계 종횡비")]
    public float lowerLimitAspectRatio = 1f;
    public float upperLimitAspectRatio = 2f;

    [Header("몬스터 드래그 줌 아웃 / 줌 인")]
    [SerializeField] private float zoomOutWidth = 25f;
    [SerializeField] private float zoomTime = 0.25f;

    [Header("레터박스")]
    [SerializeField] private Color letterBoxColor;

    [Header("UI 카메라")]
    [SerializeField] private List<Camera> uiCameras;

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

    public event Action onScreenChange;

    private void Awake()
    {
        mainCamera = Camera.main;
        currentWidth = targetWidth;
        prevScreenMode = currentScreenMode;
    }

    void OnEnable()
    {

        RenderPipelineManager.beginFrameRendering += OnBeginCameraRendering;

    }
    void OnDisable()
    {

        RenderPipelineManager.beginFrameRendering -= OnBeginCameraRendering;

    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera[] camera)
    {
        GL.Clear(true, true, letterBoxColor);
    }

    private void Update()
    {
        if (isWidthChange)
        {
            ChangeTargetWidth();
        }

        if (IsResolutionChange || IsScreenModeChange)
        {
            ChangeResolution();
            onScreenChange?.Invoke();
        }

        prevScreenMode = currentScreenMode;
        currentScreenHeight = Screen.height;
        currentScreenWidth = Screen.width;
    }

    private void ChangeResolution()
    {
        var screenAspectRatio = (float)Screen.width / (float)Screen.height;
        var targetAspectRatio = currentWidth / targetHeight;

        if (IsResolutionChange)
        {
            if (lowerLimitAspectRatio > screenAspectRatio) // 세로로 길쭉한 스크린
            {
                currentScreenMode = ScreenMode.ASPECT_RATIO;
                targetAspectRatio = lowerLimitAspectRatio;
            }
            else if (upperLimitAspectRatio < screenAspectRatio) // 가로로 길쭉한 스크린
            {
                currentScreenMode = ScreenMode.ASPECT_RATIO;
                targetAspectRatio = upperLimitAspectRatio;
            }
            else // 기본 모드
            {
                currentScreenMode = defaultScreenMode;
            }
        }

        switch (currentScreenMode)
        {
            case ScreenMode.FULL_SCREEN:
                ApplyFullScreenRect();
                break;
            case ScreenMode.ASPECT_RATIO:
                ApplyLetterBoxRect(screenAspectRatio, targetAspectRatio);
                break;
            case ScreenMode.SAFE_AREA:
                ApplySafeAreaRect();
                break;
        }

        ResizeAndRepositionCamera();
        AdjustUiCamera();
    }

    private void ApplyFullScreenRect()
    {
        var normalRect = new Rect(0f, 0f, 1f, 1f);
        mainCamera.rect = normalRect;
    }

    private void ApplySafeAreaRect()
    {
        var safeAreaRect = Screen.safeArea;
        var cameraRect = safeAreaRect;
        cameraRect.x /= Screen.width;
        cameraRect.y /= Screen.height;
        cameraRect.width /= Screen.width;
        cameraRect.height /= Screen.height;

        mainCamera.rect = cameraRect;
    }

    private void ApplyLetterBoxRect(float screenAspectRatio, float targetAspectRatio)
    {
        if (screenAspectRatio >= targetAspectRatio) // 좌우 레터박스
        {
            float viewportWidth = targetAspectRatio / screenAspectRatio;
            var adjustedRect = new Rect((1f - viewportWidth) / 2f, 0f, viewportWidth, 1f);
            mainCamera.rect = adjustedRect;
        }
        else // 상하 레터박스
        {
            float viewportHeight = screenAspectRatio / targetAspectRatio;
            var adjustedRect = new Rect(0f, (1f - viewportHeight) / 2f, 1f, viewportHeight);
            mainCamera.rect = adjustedRect;
        }
    }

    private void ResizeAndRepositionCamera()
    {
        var aspectRatio = mainCamera.aspect;

        // Resize
        var orthographicSize = currentWidth / (aspectRatio * 2f);
        mainCamera.orthographicSize = orthographicSize;

        // Reposition
        var cameraPositionY = bottomY + orthographicSize;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, cameraPositionY, mainCamera.transform.position.z);
    }

    private void AdjustUiCamera()
    {
        foreach (var uiCamera in uiCameras)
        {
            uiCamera.rect = mainCamera.rect;
            uiCamera.orthographicSize = mainCamera.orthographicSize;
        }
    }

    public void ZoomOutCamera() => SetTargetWidth(zoomOutWidth, zoomTime);
    public void ResetCamera() => SetTargetWidth(targetWidth, zoomTime);

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

        ResizeAndRepositionCamera();
    }
}