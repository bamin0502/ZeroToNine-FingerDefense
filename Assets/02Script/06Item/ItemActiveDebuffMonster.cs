using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Active Monster Debuff", fileName = "Item.asset")]
public class ItemActiveDebuffMonster : ActiveItem
{
    [Header("이펙트")]
    public EffectController effectPrefab;
    public int effectCount;
    public Vector2 effectPos;
    public Vector2 effectInterval;

    [Header("몬스터 디버프")]
    [SerializeField] public List<ItemDebuffMonster> debuffs;

    public override void UseItem()
    {
        var monsters = GameObject.FindGameObjectsWithTag(Defines.Tags.MONSTER_TAG);
        if (monsters == null)
            return;

        foreach (var monster in monsters)
        {
            if (!monster.TryGetComponent<MonsterController>(out var controller))
                continue;
            foreach (var debuff in debuffs)
            {
                debuff.GiveBuff(controller);
            }
        }
        
        CreateEffect();
        base.UseItem();
    }

    private void CreateEffect()
    {
        if (!effectPrefab)
            return;

        for (int i = 0; i < effectCount; i++)
        {
            Vector3 pos = effectPos + effectInterval * i;
            pos.z = pos.y;
            var effect = Instantiate(effectPrefab, pos, Quaternion.identity);
            effect.LifeTime = duration;
        }
    }
}
