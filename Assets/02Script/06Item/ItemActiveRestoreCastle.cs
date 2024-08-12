using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Active Restore Castle HP", fileName = "Item.asset")]
public class ItemActiveRestoreCastle : ActiveItem
{
    public bool isPercentage;
    public float restoreCastleValue;

    public override void UseItem()
    {
        StageManager?.RestoreCastle(restoreCastleValue, isPercentage);
        base.UseItem();
    }
}
