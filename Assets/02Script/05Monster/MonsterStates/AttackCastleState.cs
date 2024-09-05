using Spine;
using UnityEngine;

public class AttackCastleState : IState
{
    private MonsterController controller;

    private float attackTimer;
    private float attackCoolDown;

    private TrackEntry attackTrackEntry;

    private bool isAnimationEnded;
    public AttackCastleState(MonsterController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        attackCoolDown = 1f / controller.Status.Data.AtkSpeed;
        attackTimer = attackCoolDown;

        controller.SetFlip(false);

        controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.IDLE, true, 1f);
        isAnimationEnded = true;
    }

    public void Update()
    {
        if ((controller.stageManager?.CurrentState ?? StageState.NONE) != StageState.PLAYING)
            return;

        if (Vector2.Distance(controller.transform.position, controller.moveTargetPos) > 0.1)
        {
            controller.TryTransitionState<PatrolState>();
            return;
        }

        if (!isAnimationEnded)
            return;

        if (attackTrackEntry != null)
            attackTrackEntry.TimeScale = controller.Status.CurrentAtkSpeed;

        if (Mathf.Approximately(controller.Status.CurrentAtkSpeed, 0f))
        {
            controller.monsterAni.CurrentTrackEntry.TimeScale = 0f;
            return;
        }
        
        attackTimer += Time.deltaTime;
        attackCoolDown = 1f / controller.Status.CurrentAtkSpeed;
        if (attackTimer >= attackCoolDown)
        {
            controller.stageManager?.DamageCastle(controller.Status.CurrentAtk);
            attackTimer = 0f;
            AttackStart();
        }
    }

    public void Exit()
    {
        
    }

    private void AttackStart()
    {
        attackTrackEntry = controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.ATTACK, false, 1f);
        if (attackTrackEntry != null)
            attackTrackEntry.Complete += AttackEnd;
        isAnimationEnded = false;
    }

    private void AttackEnd(TrackEntry entry)
    {
        if (attackTrackEntry != null)
            attackTrackEntry.Complete -= AttackEnd;

        if (attackTrackEntry == controller.monsterAni.CurrentTrackEntry)
            controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.IDLE, true, 1f);

        isAnimationEnded = true;
    }
}