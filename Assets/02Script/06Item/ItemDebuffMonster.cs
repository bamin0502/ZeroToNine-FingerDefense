using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDebuffMonster : BaseItem
{
    public int id;
    public BuffType buffType;
    public float damageTerm;
    public bool isPercentage;
    public float buffValue;
    public bool isPermanent;
    public float lastingTime;

    protected void GiveBuff(MonsterController monster)
    {
        var buffData = new BuffData();
        var value = buffValue;
        if (isPercentage)
        {
            switch (buffType)
            {
                case BuffType.ATK_SPEED:
                    value *= monster.Status.Data.AtkSpeed;
                    break;
                case BuffType.MOVE_SPEED:
                    value *= monster.Status.Data.MoveSpeed;
                    break;
                case BuffType.DOT_HP:
                    {
                        value *= monster.Status.Data.Hp;
                        buffData.DmgTerm = damageTerm;
                    }
                    break;
                case BuffType.ATK:
                    value *= monster.Status.Data.AtkDmg;
                    break;
                default:
                    break;
            }
        }
        buffData.BuffActions.Add(((int)buffType, value));
        buffData.LastingTime = lastingTime;
        monster.TryTakeBuff(buffData, out var buff, isPermanent);
    }
}