using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class MonsterController : CombatEntity<MonsterStatus>, IControllable, ITargetable, IDraggable
{
    public IObjectPool<MonsterController> pool;

    private StateMachine<MonsterController> stateMachine;
    public Type CurrentState
    {
        get => stateMachine.CurrentState.GetType();
    }

    public bool CanPatrol { get; set; }
    [HideInInspector] public Vector3 moveTargetPos;

    public Transform attackMoveTarget { get; set; }
    public PlayerCharacterController attackTarget { get; set; }

    [Header("몬스터 특성")]
    public float findRange = 3f;
    [SerializeField] private bool isDirectedRight = true;
    private float defaultRightScale;
    [HideInInspector] public float speedMultiplier = 1f;

    [HideInInspector] public MonsterSpineAni monsterAni;
    [HideInInspector] public TrackEntry deathTrackEntry;

    public SpriteRenderer shadowImage;

    public BaseSkill deathSkill;
    public BaseSkill dragDeathSkill;

    private bool isTargetReset=false;
    public bool IsDraggable
    {
        get
        {
            if (IsDead)
                return false;

            var currentState = stateMachine.CurrentState.GetType();
            if (currentState == typeof(FallState) || currentState == typeof(DragState))
                return false;

            switch (Status.Data.DragType)
            {
                case (int)MonsterData.DragTypes.BOSS:
                    return false;
                case (int)MonsterData.DragTypes.NORMAL:
                    return true;
                case (int)MonsterData.DragTypes.SPECIAL:
                    return true; // To-Do: 세이브 데이터로부터 해당 몬스터를 들 수 있는지, 조건문을 더 걸어야 합니다.
                default:
                    return false;
            }
        }
    }

    public bool IsTargetable
    {
        get
        {
            var currentState = stateMachine.CurrentState.GetType();
            return !IsDead && isTargetReset && currentState != typeof(DragState) && currentState != typeof(FallState);
        }
    }

    public float targetFallY { get; set; }

    public bool TryTransitionState<T>() where T : IState
    {
        if (typeof(T) == typeof(DragState) && !IsDraggable)
            return false;

        return stateMachine.TransitionTo<T>();
    }

    protected override void Awake()
    {
        base.Awake();
        entityType = EntityType.MONSTER;

        monsterAni = GetComponent<MonsterSpineAni>();

        defaultRightScale = isDirectedRight ? transform.localScale.x : -transform.localScale.x;

        stateMachine = new StateMachine<MonsterController>(this);

        var findBehavior = new FindingTargetInCircle(transform, findRange, 1 << LayerMask.NameToLayer("Player"));
        
        stateMachine.AddState(new IdleState<MonsterController>(this)); // To-Do: 추후 적절하게(Death) 변경
        stateMachine.AddState(new DragState(this));
        stateMachine.AddState(new FallState(this));
        stateMachine.AddState(new MoveState(this));
        stateMachine.AddState(new PatrolState(this, findBehavior));
        stateMachine.AddState(new ChaseState(this));
        stateMachine.AddState(new AttackState(this));
        stateMachine.AddState(new AttackCastleState(this));
        stateMachine.AddState(new BackMoveState(this));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        speedMultiplier = 1f;
        shadowImage?.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize<MoveState>();
    }

    public void ResetMonsterData()
    {
        IsDead = false;
        
        stateMachine.TransitionTo<MoveState>();
        Status.Init();
        UpdateHpBar();
        CanPatrol = false;
        isTargetReset=false;
    }

    private void CrossResetLine()
    {
        stageManager?.monsterSpawner?.TriggerMonsterReset(this);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.Update();

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Defines.Tags.PATROL_LINE_TAG))
        {
            CanPatrol = true;
        }

        if (other.CompareTag("ResetLine")) // To-Do: Defines에서 정의
        {
            if (!isTargetReset)
            {
                isTargetReset = true;
                CrossResetLine();
            }
        }
    }

    public override void Die(DamageReason reason = DamageReason.NONE)
    {
        if (IsDead)
            return;

        base.Die(reason);

        if (stageManager)
            stageManager.GetGold(Status.Data.DropGold);
            
        if (reason == DamageReason.PLAYER_HIT_DAMAGE)
            deathSkill?.UseSkill();
        else if (reason == DamageReason.FALL_DAMAGE || reason == DamageReason.MONSTER_HIT_DAMAGE)
            dragDeathSkill?.UseSkill();
            
        deathTrackEntry = monsterAni.SetAnimation(MonsterSpineAni.MonsterState.DEAD, false, 1f);
        var currentState = stateMachine.CurrentState.GetType();
        if (currentState == typeof(DragState))
            stateMachine.TransitionTo<FallState>();
        else if (currentState != typeof(FallState))
            stateMachine.TransitionTo<IdleState<MonsterController>>();
        if (deathTrackEntry != null)
        {
            deathTrackEntry.Complete += Die;
        }
    }

    private void Die(TrackEntry entry)
    {
        if (deathTrackEntry != null)
            deathTrackEntry.Complete -= Die;
        OnDie();
    }

    private void OnDie()
    {
        IsDead = true;
        stateMachine.TransitionTo<IdleState<MonsterController>>();

        attackTarget?.TryRemoveMonster(this);

        if (stageManager)
            stageManager.MonsterCount--;

        if (pool != null)
            pool.Release(this);
        else
            Destroy(gameObject);
    }

    public void SetFlip(bool isRight)
    {
        var newScaleX = isRight ? defaultRightScale : defaultRightScale * -1f;
        // var transform1 = transform;
        var newScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);

        transform.localScale = newScale;
    }

    public bool TryDrag()
    {
        if (!IsDraggable)
            return false;

        return stateMachine.TransitionTo<DragState>();
    }

    public bool TryFall()
    {
        return stateMachine.TransitionTo<FallState>();
    }
}
