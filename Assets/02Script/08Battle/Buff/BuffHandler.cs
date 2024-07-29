using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHandler
{
    public List<Buff> buffs = new();

    public IControllable controller;
    public event Action<float> OnDotDamage;

    public BuffHandler(IControllable controller)
    {
        this.controller = controller;
    }

    private void ResetBuff()
    {
        buffs.Clear();
    }

    public void TimerUpdate()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            buffs[i].TimerUpdate();
            if (buffs[i].IsBuffExpired)
            {
                RemoveBuff(i);
            }
            if (buffs[i].isDotDamage)
            {
                OnDotDamage?.Invoke(buffs[i].dotDamage);
                buffs[i].isDotDamage = false;
            }
        }
    }

    public void AddBuff(BuffData data)
    {
        if (data == null)
        {
            Logger.LogError("해당 버프의 정보가 없습니다.");
            return;
        }

        var buff = new Buff(data);
        AddBuff(buff);
    }

    public void AddBuff(Buff buff)
    {
        if (buff == null)
        {
            Logger.LogError("해당 버프의 정보가 없습니다.");
            return;
        }

        if (buffs.Count >= 3)
        {
            Logger.Log($"Buff 최대({buffs.Count}): {buff.ToString()}");
            return;
        }

        buffs.Add(buff);
        controller.UpdateCurrentState();

        Logger.Log($"Buff 추가: {buff.ToString()}");
    }

    private void RemoveBuff(int index)
    {
        if (index < 0 || index >= buffs.Count)
            return;

        Logger.Log($"Buff 제거: {buffs[index].ToString()}");

        buffs.RemoveAt(index);
        controller.UpdateCurrentState();
    }
}