using Spine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackBehavior : MonoBehaviour
{
    public BaseSkill normalAttack;
    private BaseSkill skillAttack;
    public BaseSkill SkillAttack
    {
        get => skillAttack;
        set
        {
            skillAttack = value;
            if (skillAttack != null)
                skillAttack.coolTimeBar = skillAttackCoolTimeBar;
            else
                skillAttackCoolTimeBar?.transform.parent?.gameObject.SetActive(false);
        }
    }

    private BaseSkill currentAttack;

    private CharacterSpineAni characterAni;
    private bool isAnimationEnded = true;

    private TrackEntry attackTrackEntry;
    private PlayerCharacterController controller;

    public Image skillAttackCoolTimeBar;

    private void Awake()
    {
        characterAni = GetComponent<CharacterSpineAni>();
        controller = GetComponent<PlayerCharacterController>();
    }

    private void OnEnable()
    {
        characterAni.SetAnimation(CharacterSpineAni.CharacterState.IDLE, true, 1f);
        isAnimationEnded = true;
    }

    private void FixedUpdate()
    {
        if (controller && controller.IsDead)
            return;
        
        if (controller != null)
            UpdateSkill(normalAttack, controller.BuffHandler.buffValues[BuffType.ATK_SPEED], true);
        else
            UpdateSkill(normalAttack);
        UpdateSkill(SkillAttack);
    }

    private void UpdateSkill(BaseSkill skill, float coolTimeBuff = 0f, bool isBuffApplied = false)
    {
        if (skill == null)
            return;

        skill.TimerUpdate(coolTimeBuff);

        if (!isAnimationEnded)
            return;

        if (skill.IsSkillReady)
        {
            if (skill.UseSkill(isBuffApplied))
            {
                currentAttack = skill;
                SkillStart();
            }
        }
    }

    private void SkillStart()
    {
        attackTrackEntry = characterAni.SetAnimation(CharacterSpineAni.CharacterState.ATTACK, false, 1f);
        if (attackTrackEntry != null)
            attackTrackEntry.Complete += SkillEnd;
        isAnimationEnded = false;
    }

    private void SkillEnd(TrackEntry entry)
    {
        if (attackTrackEntry != null)
            attackTrackEntry.Complete -= SkillEnd;

        if (attackTrackEntry == characterAni.CurrentTrackEntry)
            characterAni.SetAnimation(CharacterSpineAni.CharacterState.IDLE, true, 1f);

        isAnimationEnded = true;
    }
}