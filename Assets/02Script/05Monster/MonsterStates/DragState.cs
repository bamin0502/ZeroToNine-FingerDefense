using UnityEngine;

public class DragState : IState
{
    private MonsterController controller;
    private MeshRenderer renderer;
    private Collider2D collider;

    private DragAndDrop dragAndDrop;

    private bool isReachHeight;

    private float limitCastleX;
    private float limitCameraX = 10f;
    private float limitCameraOffset= 0.4f;
    
    private TutorialGameTrigger tutorialGameTrigger;

    public DragState(MonsterController controller)
    {
        this.controller = controller;

        renderer = controller.GetComponentInChildren<MeshRenderer>();
        collider = controller.GetComponent<Collider2D>();
       
        //To-Do Tutorial 몬스터 이동 상태 추가
        if (controller.IsTutorialMonster)
        {
            tutorialGameTrigger = controller.GetComponent<TutorialGameTrigger>();
        }
    }

    public void Enter()
    {
        controller.targetFallY = controller.transform.position.y;
        isReachHeight = false;

        var stageManager = controller.stageManager;
        if (stageManager)
        {
            limitCastleX = Utils.GetXFromLinear(stageManager.castleRightTopPos.position, stageManager.castleLeftBottomPos.position, controller.transform.position.y);
        }
        else
        {
            limitCastleX = controller.moveTargetPos.x;
        }

        if (controller.attackTarget)
            controller.attackTarget.TryRemoveMonster(controller);

        dragAndDrop = GameObject.FindGameObjectWithTag("InputManager").GetComponent<DragAndDrop>();

        collider.enabled = false;
        renderer.sortingOrder = 1;
        controller.shadowImage?.gameObject.SetActive(false);

        controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.LAYDOWN_AFTER, true, 1f);
        TimeScaleController.ChangeTimeSclae(0.1f, 0.25f);
        if (Camera.main != null && Camera.main.TryGetComponent<GameCameraController>(out var cameraController))
        {
            cameraController.ZoomOutCamera();
            limitCameraX = cameraController.transform.position.x + cameraController.targetWidth / 2f - limitCameraOffset;
        }
        
        // 드래그 시작 시점에서 튜토리얼 몬스터인 경우에만 OnDragStarted 호출
        if (controller.IsTutorialMonster && tutorialGameTrigger != null)
        {
            tutorialGameTrigger.OnDragStarted();
        }
    }

    public void Update()
    {
        if (dragAndDrop.IsDragging)
        {
            var pos = Camera.main!.ScreenToWorldPoint(dragAndDrop.GetPointerPosition());
            if (pos.y <= controller.targetFallY)
                pos.y = controller.targetFallY;
            if (pos.x <= limitCastleX)
                pos.x = limitCastleX;
            if (pos.x >= limitCameraX)
                pos.x = limitCameraX;
            pos.z = pos.y;
            controller.transform.position = pos;

            if (!isReachHeight && pos.y > controller.Status.Data.Height + controller.targetFallY)
            {
                AudioManager.Vibration();
                isReachHeight = true;
            }
            else if (isReachHeight && pos.y < controller.Status.Data.Height + controller.targetFallY)
            {
                isReachHeight = false;
            }
        }
        else
        {
            controller.TryTransitionState<FallState>();
        }
    }

    public void Exit()
    {
        renderer.sortingOrder = 0;
        
        if (Time.timeScale == 0f)
            return;

        TimeScaleController.SetTimeScale(1f);
        if (Camera.main != null && Camera.main.TryGetComponent<GameCameraController>(out var cameraController))
        {
            cameraController.ResetCamera();
        }

        var pos = controller.transform.position;
        pos.x = Mathf.Clamp(pos.x, limitCastleX, limitCameraX);
        controller.transform.position = pos;
    }
}
