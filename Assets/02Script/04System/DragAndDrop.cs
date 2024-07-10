using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private float targetOriginY;
    private bool isDragging = false;


    
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnClick += OnPointerDown;
        InputManager.Instance.OnRelease += OnPointerUp;
        InputManager.Instance.OnDrag += OnPointerDrag;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnClick -= OnPointerDown;
        InputManager.Instance.OnRelease -= OnPointerUp;
        InputManager.Instance.OnDrag -= OnPointerDrag;
    }
    
    // 태그를 통해 드래그 가능한 오브젝트를 찾아서 드래그 가능 여부를 판단
    // 인터페이스가 있는지 여부를 가져와야함 
    private void OnPointerDown(InputAction.CallbackContext context)
    {
        if (!isDragging)
        {
            var mouseScreenPos = GetPointerPosition(context);
            var mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            var hit = Physics2D.Raycast(mouseWorldPos, transform.forward, Mathf.Infinity);

            if (hit)
            {
                var target = hit.collider.gameObject;
                if (target.TryGetComponent<MonsterController>(out var controller))
                {
                    if (controller.TryTransitionState<DragState<MonsterController>>())
                    {
                        targetOriginY = target.transform.position.y;
                        isDragging = true;
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

    private void OnPointerUp(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            isDragging = false;
            FallObject(gameObject, targetOriginY).Forget();
            targetOriginY = 0f;
        }
    }

    private void OnPointerDrag(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            var pos = mainCamera.ScreenToWorldPoint(GetPointerPosition(context));
            var transform1 = transform;
            pos.z = transform1.position.z;
            transform1.position = pos;
            
        }
    }

    private Vector2 GetPointerPosition(InputAction.CallbackContext context)
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

    private async UniTask FallObject(GameObject target, float targetHeight)
    {
        const float gravity = -9.8f;
        var velocity = 0f;

        while (target != null)
        {
            if(target != null && target.transform.position.y <= targetHeight)
            {
                target.transform.position = new Vector3(target.transform.position.x, targetHeight, 0f);
                if (target.TryGetComponent<MonsterController>(out var controller))
                {
                    controller.TryTransitionState<MoveState<MonsterController>>();
                }
                break;
            }
            
            velocity += gravity * Time.deltaTime;
            target.transform.position += new Vector3(0, velocity * Time.deltaTime, 0);
            await UniTask.Yield();
        }
    }
}
