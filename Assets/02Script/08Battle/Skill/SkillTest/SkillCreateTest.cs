using UnityEngine;
using TMPro;

public class SkillCreateTest : MonoBehaviour
{
    private SkillData currentSkillData;
    private BaseSkill currentSkill;

    // UI
    public TMP_InputField targetInput;
    private int target;

    public TMP_InputField projectileInput;
    private int projectile;

    public TMP_InputField centerInput;
    private int center;

    public TMP_InputField typeInput;
    private int type;

    public TMP_InputField rangeInput;
    private float range;

    public TMP_InputField damageInput;
    private float damage;

    public TMP_InputField coolTimeInput;
    private float coolTime;

    public TMP_InputField durationInput;
    private float duration;

    public TMP_InputField castingTimeInput;
    private float castingTime;

    public TMP_InputField buffIdInput;
    private int buffId;

    public TMP_InputField assetNoInput;
    private int assetNo;


    public void CreateSkill()
    {
        InputFieldChange();

        var skillData = new SkillData()
        {
            Target = target,
            Projectile = projectile,
            Center = center,
            Type = type,
            Range = range,
            Damage = damage,
            CoolTime = coolTime,
            Duration = duration,
            CastingTime = castingTime,
            BuffId = buffId,
            AssetNo = assetNo
        };

        Logger.Log(skillData.ToString());
        currentSkillData = skillData;
    }

    private void InputFieldChange()
    {
        int.TryParse(targetInput.text, out target);
        int.TryParse(projectileInput.text, out projectile);
        int.TryParse(centerInput.text, out center);
        int.TryParse(typeInput.text, out type);
        float.TryParse(rangeInput.text, out range);
        float.TryParse(damageInput.text, out damage);
        float.TryParse(coolTimeInput.text, out coolTime);
        float.TryParse(durationInput.text, out duration);
        float.TryParse(castingTimeInput.text, out castingTime);
        int.TryParse(buffIdInput.text, out buffId);
        int.TryParse(assetNoInput.text, out assetNo);
    }
}
