
public enum DamageReason
{
    NONE = -1,
    PLAYER_HIT_DAMAGE,
    MONSTER_HIT_DAMAGE,
    DOT_DAMAGE,
    FALL_DAMAGE
}

public interface IDamageable
{
    bool IsDamageable { get; }

    bool TakeDamage(float damage, DamageReason reason = DamageReason.NONE, Elements element = Elements.NONE);
    bool RecoverHeal(float heal);
}