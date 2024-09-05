using Spine;
using UnityEngine;

public class BackMoveState : IState
{
    private MonsterController controller;
    private Vector3 direction = Vector3.zero;

    private TrackEntry moveTrackEntry;

    public BackMoveState(MonsterController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        moveTrackEntry = controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.WALK, true, 1f);
        if (controller.moveTargetPos != null)
        {
            direction.x = controller.moveTargetPos.x
            > controller.transform.position.x ? -1f : 1f;
        }
        controller.SetFlip(!(direction.x > 0));
    }

    public void Update()
    {
        controller.transform.position += direction * controller.Status.CurrentMoveSpeed * controller.speedMultiplier * Time.deltaTime;
        if (moveTrackEntry != null && controller.monsterAni.CurrentTrackEntry == moveTrackEntry)
            moveTrackEntry.TimeScale = controller.Status.CurrentMoveSpeed * controller.speedMultiplier;
    }

    public void Exit()
    {
        controller.SetFlip(direction.x > 0);
    }
}
