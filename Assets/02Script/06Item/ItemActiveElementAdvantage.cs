using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Active Element Advantage", fileName = "Item.asset")]
public class ItemActiveElementAdvantage : ActiveItem
{
    private List<SpriteRenderer> activeSprites = new();

    public override void Init()
    {
        base.Init();

        if (StageMgr)
            StageMgr.isPlayerElementAdvantage = false;
    }

    public override void UseItem()
    {
        base.UseItem();

        if (StageMgr)
            StageMgr.isPlayerElementAdvantage = true;

        var characters = GameObject.FindGameObjectsWithTag(Defines.Tags.PLAYER_TAG);
        foreach (var character in characters)
        {
            if (character.TryGetComponent<PlayerCharacterController>(out var controller))
            {
                // controller.elementImage.sprite = sprite;
                controller.elementImage?.gameObject.SetActive(true);
            }
        }
    }

    public override void CancelItem()
    {
        base.CancelItem();

        if (StageMgr)
            StageMgr.isPlayerElementAdvantage = false;

        var characters = GameObject.FindGameObjectsWithTag(Defines.Tags.PLAYER_TAG);
        foreach (var character in characters)
        {
            if (character.TryGetComponent<PlayerCharacterController>(out var controller))
            {
                controller.elementImage?.gameObject.SetActive(false);
            }
        }
    }
}
