using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{

    #region Events
    public event Action<InputAction.CallbackContext> OnClick;
    public event Action<InputAction.CallbackContext> OnRelease;
    public event Action<InputAction.CallbackContext> OnDrag;
    public event Action<InputAction.CallbackContext> OnBack;
    
    public event Action<Vector2> OnTouchStartedEvent;
    public event Action<Vector2> OnTouchEndedEvent;
    
    #endregion
    private Control control;

    public GameObject QuitPanel;
    public StageManager stageManager;
    
    private void Awake()
    {
        control = new Control();
    }

    private void Start()
    {
        if (stageManager == null)
        {
        }
    }

    private void OnEnable()
    {
        if (stageManager != null)
        {
        }
        control.Enable();
        control.MonsterDrag.Click.performed += OnClickHandler;
        control.MonsterDrag.Release.performed += OnReleaseHandler;
        control.MonsterDrag.Drag.performed += OnDragHandler;
        control.UI.Back.performed += OnBackHandler;
        control.Touch.TouchPress.started += OnTouchStarted;
        control.Touch.TouchPress.canceled += OnTouchEnded;
        
    }

    
    private void OnDisable()
    {
        control.Disable();
        control.MonsterDrag.Click.performed -= OnClickHandler;
        control.MonsterDrag.Release.performed -= OnReleaseHandler;
        control.MonsterDrag.Drag.performed -= OnDragHandler;
        control.UI.Back.performed -= OnBackHandler;
        control.Touch.TouchPress.started -= OnTouchStarted;
        control.Touch.TouchPress.canceled -= OnTouchEnded;
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        var touchPosition = control.Touch.TouchPosition.ReadValue<Vector2>();
        OnTouchStartedEvent?.Invoke(touchPosition);
    }
    
    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        var touchPosition = control.Touch.TouchPosition.ReadValue<Vector2>();
        OnTouchEndedEvent?.Invoke(touchPosition);
    }


    private void OnClickHandler(InputAction.CallbackContext context)
    {
        OnClick?.Invoke(context);
    }

    private void OnReleaseHandler(InputAction.CallbackContext context)
    {
        OnRelease?.Invoke(context);
    }

    private void OnDragHandler(InputAction.CallbackContext context)
    {
        OnDrag?.Invoke(context);
    }
    
    
    private void OnBackHandler(InputAction.CallbackContext context)
    {
        OnBack?.Invoke(context);
        QuitPanel.SetActive(!QuitPanel.activeSelf);
        QuitPanel.transform.SetAsLastSibling();
        
        if(stageManager.CurrentState==StageState.GAME_OVER)
        {
            TimeScaleController.SetTimeScale(0f);
        }
        else
        {
            TimeScaleController.SetTimeScale(QuitPanel.activeSelf ? 0 : 1);   
        }
        
    }
    
}
