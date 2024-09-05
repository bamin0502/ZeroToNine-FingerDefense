using UnityEngine;

public class InstantSkill : BaseSkill
{
    public InstantSkill(SkillData skillData, SkillType skillType, IFindable targetingMethod, GameObject caster)
    : base(skillData, skillType, targetingMethod, caster) { }

    public override bool UseSkill(bool isBuffApplied = false)
    {
        var target = targetingMethod.FindTarget();
        if (target == null || skillType == null)
            return false;

        if (skillType.UseSkill(target, isBuffApplied))
        {
            IsSkillReady = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}