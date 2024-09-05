using UnityEngine;

public class PlacementSkill : SkillType
{
    public float Duration { get; private set; }
    public float Radius { get; private set; }
    public int Target { get; private set; }

    private float areaOffsetY = -2.6f;
    
    public PlacementSkill(IFindable secondaryTargeting, SkillData data)
    : base(secondaryTargeting, data)
    {
        Duration = data.Duration;
        Radius = data.Range;
        Target = data.Target;
    }

    public override bool UseSkill(GameObject target, bool isBuffApplied = false)
    {
        var area = SkillFactory.CreateSkillArea();
        if (area == null)
            return false;
        area.transform.position = new Vector2(target.transform.position.x, areaOffsetY);
        area.Init(this);
        return true;
    }

    public bool EnterArea(GameObject target, SkillArea area)
    {
        var isSkillApplied = false;
        if (buffSkill != null)
        {
            isSkillApplied = buffSkill.EnterSkillArea(target, area);
        }

        if (attackSkill != null)
        {
            isSkillApplied = isSkillApplied || attackSkill.EnterSkillArea(target, area);
        }

        return isSkillApplied;
    }

    public bool ExitArea(GameObject target, SkillArea area)
    {
        var isSkillApplied = false;
        if (buffSkill != null)
        {
            isSkillApplied = buffSkill.ExitSkillArea(target, area);
        }

        if (attackSkill != null)
        {
            isSkillApplied = isSkillApplied || attackSkill.ExitSkillArea(target, area);
        }

        return isSkillApplied;
    }
}
