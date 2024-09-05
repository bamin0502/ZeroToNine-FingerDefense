using Spine;
using UnityEngine;

public class ChaseState : IState
{
    private MonsterController controller;

    private TrackEntry moveTrackEntry;

    public ChaseState(MonsterController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        if (controller.monsterAni.CurrentMonsterState != MonsterSpineAni.MonsterState.WALK)
            moveTrackEntry = controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.WALK, true, controller.Status.CurrentMoveSpeed);
        else
            moveTrackEntry = controller.monsterAni.CurrentTrackEntry;
    }

    public void Update()
    {
        if (controller.attackTarget == null)
        {
            controller.TryTransitionState<PatrolState>();
            return;
        }

        var direction = ((Vector2)(controller.attackMoveTarget.position - controller.transform.position)).normalized;
        controller.transform.position += (Vector3)direction * controller.Status.CurrentMoveSpeed * Time.deltaTime;
        if (moveTrackEntry != null)
            moveTrackEntry.TimeScale = controller.Status.CurrentMoveSpeed;
        controller.SetFlip(direction.x > 0);
        if (Vector2.Distance(controller.transform.position, controller.attackMoveTarget.position) < 0.1)
        {
            controller.transform.position = (Vector2)controller.attackMoveTarget.position;
            controller.TryTransitionState<AttackState>();
            return;
        }
    }

    public void Exit()
    {
    }
}
