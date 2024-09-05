using UnityEngine;

[CreateAssetMenu(menuName = "Item/Active Increase Gold", fileName = "Item.asset")]
public class ItemActiveGetGoldIncrease : ActiveItem
{
    [Header("골드 배율")]
    [Tooltip("해당 아이템의 지속 기간 중 획득하는 모든 골드에 해당 배율을 적용합니다.")]
    public float goldMultiplier;

    public override void UseItem()
    {
        SetGoldMultiplier(goldMultiplier);
        base.UseItem();
    }

    public override void CancelItem()
    {
        SetGoldMultiplier(1f);
        base.CancelItem();
    }

    private void SetGoldMultiplier(float multiplier)
    {
        if (!StageMgr)
            return;

        StageMgr.GoldMultiplier = multiplier;
    }
}
