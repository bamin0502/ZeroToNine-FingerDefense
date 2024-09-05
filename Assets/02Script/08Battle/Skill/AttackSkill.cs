using UnityEngine;

public class AttackSkill : ISkillAction
{
    private float damage;
    private BuffHandler buffHandler;
    private DamageReason caster = DamageReason.NONE;
    private Elements element = Elements.NONE;

    public AttackSkill(float damage, GameObject gameObject = null)
    {
        this.damage = damage;
        if (gameObject != null)
        {
            if (gameObject.TryGetComponent<IBuffGettable>(out var buffGettable))
                buffHandler = buffGettable.BuffHandler;

            if (gameObject.CompareTag(Defines.Tags.PLAYER_TAG))
                caster = DamageReason.PLAYER_HIT_DAMAGE;
            else if (gameObject.CompareTag(Defines.Tags.MONSTER_TAG))
                caster = DamageReason.MONSTER_HIT_DAMAGE;

            if (gameObject.TryGetComponent<MonsterController>(out var monsterController))
                element = monsterController.Status.element;
            else if(gameObject.TryGetComponent<PlayerCharacterController>(out var playerController))
                element = playerController.Status.element;
        }
    }

    public bool ApplySkillAction(GameObject target, bool isBuffApplied = false)
    {
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            if (isBuffApplied && buffHandler != null)
                return damageable.TakeDamage(damage + buffHandler.buffValues[BuffType.ATK], caster, element);
                
            return damageable.TakeDamage(damage, caster, element);
        }

        return false;
    }

    public bool EnterSkillArea(GameObject target, SkillArea area)
    {
        return ApplySkillAction(target);
    }

    public bool ExitSkillArea(GameObject target, SkillArea area)
    {
        return true;
    }
}