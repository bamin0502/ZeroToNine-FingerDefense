using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private float targetOriginY;
    public bool IsDragging { get; private set; } = false;

    private GameObject draggingObject;
    private const float autoDropTime = 2.0f; // 드래그 시작 후 2초 뒤에 자동으로 놓기

    private static readonly RaycastHit2D[] hits = new RaycastHit2D[10];
    private InputManager inputManager;

    private StageManager stageManager;
    
    
    private void Awake()
    {
        mainCamera = Camera.main;
        inputManager=GetComponent<InputManager>();
    }

    private void Start()
    {
        stageManager = GameObject.FindWithTag("StageManager")?.GetComponent<StageManager>();
    }

    private void OnEnable()
    {
        inputManager.OnClick += OnPointerDown;
        inputManager.OnRelease += OnPointerUp;
        inputManager.OnDrag += OnPointerDrag;
    }

    private void OnDisable()
    {
        inputManager.OnClick -= OnPointerDown;
        inputManager.OnRelease -= OnPointerUp;
        inputManager.OnDrag -= OnPointerDrag;
    }

    private void OnPointerDown(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject()
        || (stageManager != null && stageManager.CurrentState != StageState.PLAYING))
        {
            return;
        }
        
        if (context.control.device is Mouse or Touchscreen)
        {
            if (!IsDragging)
            {
                var mouseScreenPos = GetPointerPosition();
                var mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
                LayerMask mask = LayerMask.GetMask("Monster");
                
                var hitCount = Physics2D.RaycastNonAlloc(mouseWorldPos, Vector2.zero, hits);

                GameObject highestSortingOrderObject = null;
                var highestSortingOrder = int.MinValue;

                for (var i = 0; i < hitCount; i++)
                {
                    var hit = hits[i];
                    if (hit.collider != null && mask == (mask | (1 << hit.collider.gameObject.layer)))
                    {
                        var spriteRenderer = hit.collider.gameObject.GetComponentInChildren<MeshRenderer>();
                        if (spriteRenderer != null && spriteRenderer.sortingOrder > highestSortingOrder)
                        {
                            highestSortingOrder = spriteRenderer.sortingOrder;
                            highestSortingOrderObject = hit.collider.gameObject;
                        }
                    }
                }

                if (highestSortingOrderObject != null)
                {
                    var target = highestSortingOrderObject;
                    if (target.TryGetComponent<MonsterController>(out var controller))
                    {
                        if (controller.TryTransitionState<DragState>())
                        {
                            targetOriginY = target.transform.position.y;
                            IsDragging = true;
                            draggingObject = target;

                            // 일정 시간이 지나면 자동으로 놓기
                            AutoDropAfterTime(autoDropTime).Forget();
                        }
                        else
                        {
                            Logger.Log("This GameObject cannot be dragged!");
                        }
                    }
                }
            }
            else
            {
                Logger.Log("Already dragging!");
            }
        }
    }

    private void OnPointerUp(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject()
            || (stageManager != null && stageManager.CurrentState != StageState.PLAYING))
        {
            return;
        }
        
        if (context.control.device is Mouse or Touchscreen)
        {
            if (IsDragging)
            {
                DropObject();
            }
        }
    }

    private void OnPointerDrag(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        } 
        
        if (context.control.device is Mouse or Touchscreen)
        {
            if (IsDragging && draggingObject)
            {
                var pos = mainCamera.ScreenToWorldPoint(GetPointerPosition());
                var transform1 = draggingObject.transform;
                pos.z = transform1.position.z;
                transform1.position = pos;
            }
        }
    }

    public Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            return Mouse.current.position.ReadValue();
        }
    }

    private void DropObject()
    {
        IsDragging = false;
        targetOriginY = 0f;
        draggingObject = null;
    }
    
    private async UniTask AutoDropAfterTime(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale:true);

        if (IsDragging)
        {
            DropObject();
        }
    }
    
    
}
