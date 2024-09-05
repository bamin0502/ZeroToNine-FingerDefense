using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSkill
{
    public SkillData skillData;
    protected SkillType skillType;
    protected IFindable targetingMethod;

    protected GameObject caster;

    private float skillTimer = 0f;
    public bool IsSkillReady { get; protected set; } = false;

    public Image coolTimeBar;

    public BaseSkill(SkillData skillData, SkillType skillType, IFindable targetingMethod, GameObject caster)
    {
        this.skillData = skillData;
        this.skillType = skillType;
        this.targetingMethod = targetingMethod;
        this.caster = caster;
    }

    public void TimerUpdate(float coolTimeBuff = 0f)
    {
        if (IsSkillReady || skillData == null)
            return;

        skillTimer += Time.deltaTime;
        if (skillTimer >= skillData.CoolTime - coolTimeBuff)
        {
            IsSkillReady = true;
            skillTimer = 0f;
        }

        if (coolTimeBar)
        {
            if (IsSkillReady)
                coolTimeBar.fillAmount = 1f;
            else if (skillData.CoolTime - coolTimeBuff > 0f && skillTimer > 0f)
                coolTimeBar.fillAmount = skillTimer / (skillData.CoolTime - coolTimeBuff);
            else
                coolTimeBar.fillAmount = 0f;
        }
    }

    public abstract bool UseSkill(bool isBuffApplied = false);
}