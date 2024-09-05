using System;

public abstract class BaseStatus
{
    public BuffHandler buffHandler;
    public Elements element = Elements.NONE;

    public event Action OnHpBarUpdate;
    protected float currentMaxHp;
    public float CurrentMaxHp
    {
        get
        {
            if (buffHandler == null)
                return currentMaxHp;

            var maxHp = currentMaxHp + buffHandler.buffValues[BuffType.MAX_HP];
            return maxHp > 0f ? maxHp : 1f; // To-Do: 최대 체력이 0이하일 때의 처리
        }
    }
    protected float currentHp;
    public float CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            OnHpBarUpdate?.Invoke();
        }
    }

    public abstract void Init();
}