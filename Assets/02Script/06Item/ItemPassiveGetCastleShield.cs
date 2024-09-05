using UnityEngine;

[CreateAssetMenu(menuName = "Item/Passive Get Castle Shield", fileName = "Item.asset")]
public class ItemPassiveGetCastleShield : BaseItem
{
    public bool isPercentage;
    public float castleShield;

    public override void UseItem()
    {
        StageMgr?.GetShield(castleShield, isPercentage);
    }
}
