using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 드래그 테스트용 스크립트이며, 터치 관련 시스템이 구축되면 삭제해야 합니다.
/// </summary>
public class TestDrag : MonoBehaviour
{
    private bool isDragging = false;
    private GameObject dragTarget;
    private float targetOriginY;

    private void Update()
    {
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit;
            // LayerMask layerMask = 1 << LayerMask.NameToLayer("Monster");
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            hit = Physics2D.Raycast(mouseWorldPos, transform.forward, Mathf.Infinity);
            if (hit)
            {
                var target = hit.collider.gameObject;
                if (target.TryGetComponent<MonsterController>(out var controller)
                && controller.TryTransitionState<DragState<MonsterController>>())
                {
                    dragTarget = target;
                    targetOriginY = dragTarget.transform.position.y;
                    isDragging = true;
                }
            }
        }

        if (isDragging)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = dragTarget.transform.position.z;
            dragTarget.transform.position = pos;

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                FallObject(dragTarget, dragTarget.transform.position.y, targetOriginY).Forget();

                targetOriginY = 0f;
                dragTarget = null;
            }
        }
    }
    
    // 타겟의 자유 낙하 운동을 비동기로 처리
    private async UniTask FallObject(GameObject target, float startY, float targetY)
    {
        var gravity = -9.8f;
        var velocity = 0f;

        while (target != null)
        {
            if (target.transform.position.y <= targetY)
            {
                target.transform.position = new Vector3(target.transform.position.x, targetY, 0f);
                if (target != null && target.TryGetComponent<MonsterController>(out var controller))
                {
                    if (controller.Data.Height <= startY - targetY)
                        controller.pool.Release(controller);
                    else
                        controller.TryTransitionState<MoveState>();
                }
                break;
            }

            velocity += gravity * Time.deltaTime;
            target.transform.position += new Vector3(0, velocity * Time.deltaTime, 0);

            await UniTask.NextFrame();
        }
    }
}