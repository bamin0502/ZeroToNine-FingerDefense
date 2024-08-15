using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Active Restore Castle HP", fileName = "Item.asset")]
public class ItemActiveRestoreCastle : ActiveItem
{
    [Header("이펙트")]
    public EffectController effectPrefab2;
    public Vector2 effectPos;

    [Header("성 체력 회복량")]
    public bool isPercentage;
    public float restoreCastleValue;

    public override void UseItem()
    {
        StageMgr?.RestoreCastle(restoreCastleValue, isPercentage);
        if (effectPrefab2)
        {
            Vector3 pos = effectPos;
            pos.z = pos.y;
            var effect = Instantiate(effectPrefab2, pos, Quaternion.identity);
            effect.LifeTime = duration;
        }
        base.UseItem();
    }
}
