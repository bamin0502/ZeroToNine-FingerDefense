using UnityEngine;

public enum SkillRangeTypes
{
    SingleTarget, MultipleTarget, AreaTarget
}

public abstract class SkillType
{
    // public List<ISkillAction> skillActions = new List<ISkillAction>();
    public BuffSkill buffSkill;
    public AttackSkill attackSkill;
    protected IFindable secondaryTargeting;

    public int AssetId { get; protected set; }

    public SkillType(IFindable secondaryTargeting, SkillData data)
    {
        this.secondaryTargeting = secondaryTargeting;
        this.AssetId = data.AssetNo;
    }

    public abstract bool UseSkill(GameObject target, bool isBuffApplied = false);

    protected bool ApplySkillActions(GameObject target, bool isBuffApplied = false)
    {
        var isAttacked = attackSkill != null ? attackSkill.ApplySkillAction(target, isBuffApplied) : false;
        var isBuffed = buffSkill != null ? buffSkill.ApplySkillAction(target, isBuffApplied) : false;

        return isAttacked || isBuffed;
    }
}