using UnityEngine;

public class Buff
{
    public BuffData buffData { get; private set; }
    private float expireTimer = 0f;
    public bool IsTimerStop { get; set; }
    public bool IsBuffExpired { get; private set; } = false;
    // public IDamageable damageable;

    public float dotDamage { get; private set; }
    private float dotDmgTimer = 0f;
    private bool isDotDamageExist = false;
    public bool isDotDamage = false;

    public EffectController effect;

    public Buff(BuffData data, bool isTimerStop = false)
    {
        buffData = data;
        IsTimerStop = isTimerStop;
        dotDamage = GetDotDamage();
        if (dotDamage != 0f)
            isDotDamageExist = true;
    }

    public void ReleaseBuff()
    {
        if (effect != null)
        {
            GameObject.Destroy(effect.gameObject);
        }
    }

    public void TimerUpdate()
    {
        if (isDotDamageExist)
        {
            dotDmgTimer += Time.deltaTime;
            if (dotDmgTimer >= buffData.DmgTerm)
            {
                dotDmgTimer = 0f;
                isDotDamage = true;
            }
        }

        if (IsTimerStop)
            return;

        expireTimer += Time.deltaTime;

        if (expireTimer >= buffData.LastingTime)
            IsBuffExpired = true;
    }

    public float GetDotDamage()
    {
        var buffActions = buffData.BuffActions;
        var dotDamage = 0f;
        foreach (var action in buffActions)
        {
            if (action.type == (int)BuffType.DOT_HP)
                dotDamage += action.value;
        }

        return dotDamage;
    }

    public override string ToString()
    {
        return buffData.Id.ToString();
    }
}