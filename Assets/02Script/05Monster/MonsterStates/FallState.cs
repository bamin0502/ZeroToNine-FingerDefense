using UnityEngine;

public class FallState : IState
{
    MonsterController controller;
    private Collider2D collider;
    private float startY;
    
    private float velocity;
    private float gravity = -9.8f;
    
    private TutorialGameTrigger tutorialGameTrigger;

    public FallState(MonsterController controller)
    {
        this.controller = controller;
        collider = controller.GetComponent<Collider2D>();
        
        //To-Do Tutorial 몬스터 이동 상태 추가
        if (controller.IsTutorialMonster)
        {
            tutorialGameTrigger = controller.GetComponent<TutorialGameTrigger>();
        }
    }

    public void Enter()
    {
        startY = controller.transform.position.y;
        velocity = 0f;

        if (!controller.IsDead)
            controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.LAYDOWN_AFTER, true, 1f);

        if (controller.Status.Data.Height <= startY - controller.targetFallY)
        {
            if (controller.stageManager)
                controller.stageManager.DragCount--;
        }
    }

    public void Update()
    {
        velocity += gravity * Time.deltaTime;
        controller.transform.position += new Vector3(0, velocity * Time.deltaTime, 0);

        if (controller.transform.position.y <= controller.targetFallY)
        {
            controller.transform.position = new Vector3(controller.transform.position.x, controller.targetFallY, 0f);

            if (controller.Status.Data.Height <= startY - controller.targetFallY)
            {
                controller.Die(DamageReason.FALL_DAMAGE);
            }
            
            if (!controller.IsDead && !controller.IsTutorialMonster)
                controller.TryTransitionState<PatrolState>();
            
            else if (!controller.IsDead && controller.IsTutorialMonster)
            {
                // 튜토리얼 몬스터가 생존한 경우 OnFallSurvived 호출
                if (tutorialGameTrigger != null)
                {
                    tutorialGameTrigger.OnFallSurvived();
                   
                }
                controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.IDLE, true, 1f);
                controller.TryTransitionState<IdleState<MonsterController>>();
                //controller.TryTransitionState<MoveState>();
            }
            else
            {
                controller.TryTransitionState<IdleState<MonsterController>>();
            }
           
        }
    }

    public void Exit()
    {
        collider.enabled = true;
        controller.shadowImage?.gameObject.SetActive(true);
    }
}
