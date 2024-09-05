using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum InitPosOption { CASTER, TARGET, SKY, SPECIFIED }
    public enum TargetPosOption { INITIAL, CURRENT }
    public enum MoveOption { CONSTANT_SPEED, CONSTANT_ACCELERATION }
    public enum SkillTarget { TARGET, PROJECTILE }
    // public bool isEffectEnd = false;

    // 발동 옵션: 1. 콜라이더 / 2. 거리 / 3. 타이머
    // 타겟이 사라질 경우 옵션: 1. 즉시 삭제 / 2. 일정 시간 이후 삭제 / 3. 현재 방향 유지 후 화면 밖을 나갈 경우 삭제

    [Header("투사체의 초기 위치")]
    public InitPosOption initPosOption;
    [Header("투사체의 목표 위치")]
    public TargetPosOption targetPosOption;
    [Header("투사체의 이동 방식")]
    public MoveOption moveOption;
    [HideInInspector] public SkillTarget skillTarget; // Projectile Skill에서 결정

    [HideInInspector] public GameObject caster;
    public Vector3 casterOffsetPos = Vector3.zero;
    [HideInInspector] public GameObject target;
    public Vector3 targetOffsetPos = Vector3.zero;
    private Vector3 targetInitPos;
    public ParticleSystem endEffect;
    public float speed = 20f;
    public float acceleration = 9.8f;

    private IMovable movable;

    public SkillType skill;
    [HideInInspector] public bool isBuffApplied = false;
    // [HideInInspector] public int skillType;

    private bool isTargetSet = false;
    private Vector3 prevTargetPos;

    [Header("충돌 이펙트")]
    [SerializeField] private List<GameObject> impactEffects;
    [SerializeField] private float impactEffectLifeTime;

    private void OnEnable()
    {
        isTargetSet = false;
    }

    public void Init(GameObject casterGo, GameObject targetGo, SkillType skill)
    {
        if (!targetGo)
        {
            Logger.LogError("투사체의 타겟이 존재하지 않습니다!");
            return;
        }

        this.caster = casterGo;
        this.target = targetGo;
        this.skill = skill;

        switch (initPosOption)
        {
            case InitPosOption.CASTER:
                {
                    if (!caster)
                    {
                        Logger.LogError("투사체의 캐스터가 존재하지 않습니다!");
                        return;
                    }
                    transform.position = caster.transform.position + casterOffsetPos;
                }
                break;
            case InitPosOption.TARGET:
                transform.position = target.transform.position + casterOffsetPos;
                break;
            case InitPosOption.SKY:
                transform.position = new Vector2(target.transform.position.x + casterOffsetPos.x, 25f); // To-Do: 카메라 컨트롤러의 줌아웃 너비로부터 받아온다
                break;
            case InitPosOption.SPECIFIED:
                transform.position = casterOffsetPos;
                break;
        }

        targetInitPos = target.transform.position + targetOffsetPos;
        prevTargetPos = targetInitPos;

        switch (moveOption)
        {
            case MoveOption.CONSTANT_SPEED:
                movable = new MoveConstantSpeed(gameObject, speed);
                break;
            case MoveOption.CONSTANT_ACCELERATION:
                movable = new MoveConstantAcceleration(gameObject, speed, acceleration);
                break;
        }

        isTargetSet = true;
    }

    private void Update()
    {
        if (!isTargetSet)
            return;

        var targetPos = Vector3.zero;
        switch (targetPosOption)
        {
            case TargetPosOption.INITIAL:
                targetPos = targetInitPos;
                break;
            case TargetPosOption.CURRENT:
                {
                    if (!target || !target.activeSelf) // 타겟이 사라진 경우
                    {
                        targetInitPos = prevTargetPos;
                        targetPosOption = TargetPosOption.INITIAL;
                        return;
                    }
                    targetPos = target.transform.position + targetOffsetPos;
                    prevTargetPos = targetPos;
                }
                break;
        }
        movable.Move(targetPos);

        // if (isEffectEnd && endEffect.isStopped)
        // {
        //     switch (skillTarget)
        //     {
        //         case SkillTarget.TARGET:
        //             if (target && target.activeSelf) // 타겟이 사라진 경우
        //             {
        //                 skill?.UseSkill(target, isBuffApplied);
        //             }
        //             break;
        //         case SkillTarget.PROJECTILE:
        //             skill?.UseSkill(gameObject, isBuffApplied);
        //             break;
        //     }
        //     Destroy(gameObject);
        // }

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            switch (skillTarget)
            {
                case SkillTarget.TARGET:
                    if (target && target.activeSelf) // 타겟이 사라진 경우
                    {
                        skill?.UseSkill(target, isBuffApplied);
                    }
                    break;
                case SkillTarget.PROJECTILE:
                    skill?.UseSkill(gameObject, isBuffApplied);
                    break;
            }
            Destroy(gameObject);
            CreateImpactEffect();
        }

        var pos = transform.position;
        pos.z = pos.y;
        transform.position = pos;
    }

    public void CreateImpactEffect()
    {
        var pos = transform.position;
        pos.z = pos.y;
        var effectParent = new GameObject("Impact Effect");
        var effectController = effectParent.AddComponent<EffectController>();
        effectController.LifeTime = impactEffectLifeTime;
        effectParent.transform.position = pos;
        if (impactEffects != null)
        {
            foreach (var impactEffect in impactEffects)
            {
                var effect = GameObject.Instantiate(impactEffect, pos, Quaternion.identity);
                effect.transform.SetParent(effectParent.transform);
            }
        }
    }

    // 블랙홀에 대한 대응
    private void OnTriggerStay2D(Collider2D other)
    {
        if (targetPosOption == TargetPosOption.INITIAL)
            return;

        if (!target && !target.activeSelf)
            return;

        if (other.TryGetComponent<BlackHole>(out var blackHole)
            && target.TryGetComponent<MonsterController>(out var monster))
        {
            if (blackHole.targetMonsters.Contains(monster))
            {
                skill?.UseSkill(target, isBuffApplied);
                Destroy(gameObject);
                CreateImpactEffect();
            }
        }
    }
}