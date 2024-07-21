using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public static class SkillFactory
{
    public static BaseSkill CreateSkill(SkillData data, Transform center)
    {
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

        IFindable findable = new FindingTargetInCircle(center, data.RangeValue, layerMask);
        SkillType skill = null;
        switch ((SkillRangeTypes)data.RangeType)
        {
            case SkillRangeTypes.SingleTarget:
                skill = new SingleTargetSkill();
                break;
            case SkillRangeTypes.MultipleTarget:
                skill = new MultipleTargetSkill();
                break;
            case SkillRangeTypes.AreaTarget:
                skill = new SingleTargetSkill();
                break;
        }

        if (data.Damage != 0f)
        {
            // skill.skillActions.Add(new AttackSkill(data.Damage));
            skill.attackSkill = new AttackSkill(data.Damage);
        }
        if (data.BuffId != 0)
        {
            var buffTable = DataTableManager.Get<BuffTable>(DataTableIds.Buff);
            var buffData = buffTable.Get(data.BuffId);
            // skill.skillActions.Add(new BuffSkill(buffData));
            skill.buffSkill = new BuffSkill(buffData);
        }

        BaseSkill baseSkill = new InstantSkill(skill, findable); // To-Do: Instant Skill과 Projectile Skill 분기

        return baseSkill;
    }
}
