using UnityEngine;
using TMPro;

public class SkillTest : MonoBehaviour
{
    public TMP_InputField skillId;
    public TMP_InputField normalAttackId;
    public SkillTable skillTable;

    public PlayerAttackBehavior playerAttackBehavior;
    public MonsterSpawnTest monsterSpawnTest;

    private void Awake()
    {
        skillTable = DataTableManager.Get<SkillTable>(DataTableIds.Skill);
    }

    public void ApplySkillDataToPlayer()
    {
        var skill = CreateSkill();
        var normalAttack = CreateNoramalAttack();

        playerAttackBehavior.SkillAttack = skill;
        playerAttackBehavior.normalAttack = normalAttack;
    }

    public BaseSkill CreateSkill()
    {
        if (int.TryParse(skillId.text, out var id))
        {
            var skillData = skillTable.Get(id);
            if (skillData == null)
            {
                Logger.Log("유효하지 않는 스킬 아이디입니다.");
                return null;
            }
            var skill = SkillFactory.CreateSkill(skillData, playerAttackBehavior.gameObject);
            return skill;
        }
        else
        {
            Logger.Log("int값이 아닙니다.");
            return null;
        }
    }

    public BaseSkill CreateNoramalAttack()
    {
        if (int.TryParse(normalAttackId.text, out var id))
        {
            var skillData = skillTable.Get(id);
            if (skillData == null)
            {
                Logger.Log("유효하지 않는 스킬 아이디입니다.");
                return null;
            }
            var skill = SkillFactory.CreateSkill(skillData, playerAttackBehavior.gameObject);
            return skill;
        }
        else
        {
            Logger.Log("int값이 아닙니다.");
            return null;
        }
    }
}
