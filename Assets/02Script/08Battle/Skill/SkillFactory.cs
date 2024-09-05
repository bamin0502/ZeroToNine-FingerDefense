using Defines;
using UnityEngine;

public static class SkillFactory
{
    private static readonly string skillArea = "SkillArea";

    public static BaseSkill CreateSkill(SkillData data, GameObject gameObject)
    {
        if (data == null || gameObject == null)
            return null;

        // Target: 타겟 레이어
        LayerMask layerMask = Layers.DEFAULT_LAYER;
        switch (data.Target)
        {
            case 0:
                layerMask = Layers.PLAYER_LAYER;
                break;
            case 1:
                layerMask = Layers.MONSTER_LAYER;
                break;
        }

        // 1차 타겟팅: 0인 경우, 본인 타겟팅(Target 의미 없음) / 그 외는 지정된 타겟팅
        // 1차 타겟팅은 단일 타겟팅
        IFindable primaryTargeting;
        if (data.Center <= 0f)
        {
            primaryTargeting = new FindingSelf(gameObject);
        }
        else
        {
            primaryTargeting = new FindingTargetInCircle(gameObject.transform, data.Center, layerMask);
        }

        // 스킬 타입: 0: 단일 / 1: 범위 / 2: 장판
        // To-Do: 스킬 타입이 범위와 장판인 경우, Range를 통해 2차 타겟팅
        // 단일: 2차 타겟팅은 FindingSelf
        // 범위: 2차 타겟팅은 FindingTargetInCircle.FindTargets
        // 장판: Range만큼의 장판 생성!!
        SkillType skillType = null;
        switch ((SkillRangeTypes)data.Type)
        {
            case SkillRangeTypes.SingleTarget:
                skillType = new TargetSkill(new FindingSelf(gameObject), data);
                break;
            case SkillRangeTypes.MultipleTarget:
                skillType = new TargetSkill(new FindingTargetInCircle(gameObject.transform, data.Range, layerMask), data);
                break;
            case SkillRangeTypes.AreaTarget:
                skillType = new PlacementSkill(new FindingTargetInCircle(gameObject.transform, data.Range, layerMask), data);
                break;
        }

        // Damage: 공격 스킬 / BuffId: 버프 스킬
        if (data.Damage != 0f)
        {
            // skill.skillActions.Add(new AttackSkill(data.Damage));
            skillType.attackSkill = new AttackSkill(data.Damage, gameObject);
        }
        if (data.BuffId != 0)
        {
            var buffTable = DataTableManager.Get<BuffTable>(DataTableIds.Buff);
            var buffData = buffTable.Get(data.BuffId);
            // skill.skillActions.Add(new BuffSkill(buffData));
            skillType.buffSkill = new BuffSkill(buffData);
        }

        // To-Do: Instant Skill과 Projectile Skill 분기
        BaseSkill baseSkill = null;
        if (data.Projectile == 0)
            baseSkill = new InstantSkill(data, skillType, primaryTargeting, gameObject);
        else
            baseSkill = new ProjectileSkill(data, skillType, primaryTargeting, gameObject);

        return baseSkill;
    }

    public static BaseSkill CreateSkill(int skillId, GameObject gameObject)
    {
        if (skillId == 0)
            return null;

        var skillTable = DataTableManager.Get<SkillTable>(DataTableIds.Skill);
        var skillData = skillTable.Get(skillId);
        if (skillData == null)
        {
            Logger.LogError($"유효하지 않는 스킬 아이디입니다: {skillId}");
            return null;
        }
        else
        {
            return CreateSkill(skillData, gameObject);
        }
    }

    public static SkillArea CreateSkillArea()
    {
        var areaPrefab = Resources.Load<SkillArea>(skillArea);
        var area = GameObject.Instantiate(areaPrefab);

        return area;
    }
}
