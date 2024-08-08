using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkill : BaseSkill
{
    public ProjectileSkill(SkillData skillData, SkillType skillType, IFindable targetingMethod, GameObject caster)
    : base(skillData, skillType, targetingMethod, caster) { }

    public override bool UseSkill(bool isBuffApplied = false)
    {
        var target = targetingMethod.FindTarget();
        if (target == null || skillType == null)
            return false;

        var effect = EffectFactory.CreateEffect(skillData.Projectile);
        if (!effect)
            return false;

        if (!effect.gameObject.TryGetComponent<Projectile>(out var projectile))
        {
            Logger.LogError("해당 투사체의 스크립트를 확인해주세요.");
            return false;
        }

        projectile.isBuffApplied = isBuffApplied;
        projectile.skillTarget = (Projectile.SkillTarget)Mathf.Clamp(skillData.Type, 0, 1);
        projectile.Init(caster, target, skillType);

        IsSkillReady = false;
        return true;
    }
}
