using UnityEngine;

public interface ISkillAction
{
    bool ApplySkillAction(GameObject target, bool isBuffApplied = false);
    bool EnterSkillArea(GameObject target, SkillArea area);
    bool ExitSkillArea(GameObject target, SkillArea area);
}
