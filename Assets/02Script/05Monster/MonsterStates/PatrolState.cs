using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private MonsterController controller;

    private float patrolTimer = 0f;
    private float patrolInterval = 0.25f;

    private IFindable findBehavior;

    public PatrolState(MonsterController controller, IFindable findBehavior)
    {
        this.controller = controller;
        this.findBehavior = findBehavior;
    }

    public void Enter()
    {
        patrolTimer = 0f;

        controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.WALK, true, controller.Status.currentMoveSpeed);
    }

    public void Update()
    {
        if (controller.moveTarget == null)
        {
            controller.TryTransitionState<IdleState<MonsterController>>();
            return;
        }

        // 성 포탈까지 이동
        var direction = (controller.moveTarget.transform.position - controller.transform.position).normalized;
        controller.transform.position += direction * controller.Status.currentMoveSpeed * Time.deltaTime;
        controller.SetFlip(direction.x > 0);
        if (Vector2.Distance(controller.transform.position, controller.moveTarget.transform.position) < 0.1)
        {
            controller.transform.position = controller.moveTarget.transform.position;
            controller.TryTransitionState<IdleState<MonsterController>>();
            return;
        }

        // 타겟 검색
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolInterval)
        {
            patrolTimer = 0f;
            FindTarget();
        }

        if (controller.attackTarget != null)
        {
            controller.TryTransitionState<ChaseState>();
            return;
        }
    }

    public void Exit()
    {
    }

    private void FindTarget()
    {
        var nearCollider = findBehavior.FindTarget();
        if (nearCollider == null)
            return;
        
        if (nearCollider.TryGetComponent<PlayerCharacterController>(out var target)
        && target != controller.attackTarget)
        {
            controller.attackTarget?.TryRemoveMonster(controller);
            target.TryAddMonster(controller);
        }
    }
}
