using UnityEngine;

public class TargetSkill : SkillType
{
    public TargetSkill(IFindable secondaryTargeting, SkillData data)
    : base(secondaryTargeting, data) { }

    public override bool UseSkill(GameObject primaryTarget, bool isBuffApplied = false)
    {
        if (!primaryTarget)
            return false;

        secondaryTargeting.ChangeCenter(primaryTarget);
        var targets = secondaryTargeting.FindTargets();

        var targetCount = 0;
        foreach (var target in targets)
        {
            if (target != null && ApplySkillActions(target, isBuffApplied))
            {
                targetCount++;
            }
        }

        if (targetCount > 0)
        {
            var effect = EffectFactory.CreateEffect(AssetId);
            if (effect != null)
            {
                effect.LifeTime = 1f;
                effect.gameObject.transform.position = primaryTarget.transform.position;
            }
            return true;
        }
        else
            return false;
    }
}