using UnityEngine;

public class BuffSkill : ISkillAction
{
    public BuffData buffData { get; private set; }

    public BuffSkill(BuffData data)
    {
        buffData = data;
    }

    public bool ApplySkillAction(GameObject target, bool isBuffApplied = false)
    {
        if (target.TryGetComponent<IBuffGettable>(out var buffGettable) && buffGettable.TakeBuff(buffData))    
            return true;

        return false;
    }

    public bool EnterSkillArea(GameObject target, SkillArea area)
    {
        if (target.TryGetComponent<IBuffGettable>(out var buffGettable))
        {
            if (!buffGettable.TryTakeBuff(buffData, out var buff, true))
                return false;

            if (area.Buffs.TryGetValue(target, out var prevBuff))
            {
                prevBuff.IsTimerStop = false;
                area.Buffs.Remove(target);
            }
            
            area.Buffs.Add(target, buff);
            return true;
        }
        
        return false;
    }

    public bool ExitSkillArea(GameObject target, SkillArea area)
    {
        if (!target.TryGetComponent<ITargetable>(out var targetable)
            || !targetable.IsTargetable) // To-Do: 조건문 수정
            return false;

        if (area.Buffs.TryGetValue(target, out var buff)
            && buff != null)
        {
            buff.IsTimerStop = false;
            area.Buffs.Remove(target);
            return true;
        }

        return false;
    }
}