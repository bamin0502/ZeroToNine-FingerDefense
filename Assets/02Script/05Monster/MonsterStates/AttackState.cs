using Spine;
using UnityEngine;

public class AttackState : IState
{
    private MonsterController controller;

    private float attackTimer;
    private float attackCoolDown;

    private TrackEntry attackTrackEntry;
    private bool isAnimationEnded;

    public AttackState(MonsterController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        attackCoolDown = 1f / controller.Status.Data.AtkSpeed;
        attackTimer = attackCoolDown;

        controller.SetFlip(false);

        attackTrackEntry = controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.IDLE, true, 1f);
        isAnimationEnded = true;
    }

    public void Update()
    {
        if (!controller.attackTarget)
        {
            controller.TryTransitionState<PatrolState>();
            return;
        }

        if (controller.attackTarget.IsDead)
        {
            controller.attackTarget.TryRemoveMonster(controller);
            controller.TryTransitionState<PatrolState>();
            return;
        }

        if (Vector2.Distance(controller.transform.position, controller.attackMoveTarget.position) > 0.1) // Attack Move Target의 위치 변경
        {
            controller.TryTransitionState<ChaseState>();
            return;
        }

        if (!isAnimationEnded)
            return;

        if (attackTrackEntry != null && controller.monsterAni.CurrentTrackEntry == attackTrackEntry)
            attackTrackEntry.TimeScale = controller.Status.CurrentAtkSpeed;

        if (Mathf.Approximately(controller.Status.CurrentAtkSpeed, 0f))
            return;
        
        attackTimer += Time.deltaTime;
        attackCoolDown = 1f / controller.Status.CurrentAtkSpeed;
        if (attackTimer > attackCoolDown)
        {
            controller.SetFlip(controller.attackTarget.transform.position.x > controller.transform.position.x);
            controller.attackTarget.TakeDamage(controller.Status.CurrentAtk, DamageReason.MONSTER_HIT_DAMAGE, controller.Status.element);
            AttackStart();
        }
    }

    public void Exit()
    {
        controller.attackTarget?.TryRemoveMonster(controller);
    }

    private void AttackStart()
    {
        attackTrackEntry = controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.ATTACK, false, 1f);
        if (attackTrackEntry != null)
            attackTrackEntry.Complete += AttackEnd;
        isAnimationEnded = false;
        if (controller.sfxAudioSource && controller.attackAudioClip)
            controller.sfxAudioSource.PlayOneShot(controller.attackAudioClip);
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
