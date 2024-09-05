using UnityEngine;

[CreateAssetMenu(menuName = "Item/Active Monster Direction Change", fileName = "Item.asset")]
public class ItemActiveMonsterDirection : ActiveItem
{
    [Header("이펙트")]
    public EffectController effectPrefab;

    [Header("스피드 배율")]
    [Tooltip("후퇴하는 몬스터들의 현재 이동 속도에 해당 배율을 적용하여 후퇴 속도를 결정합니다.")]
    public float speedMultiplier;

    public override void UseItem()
    {
        var monsters = GameObject.FindGameObjectsWithTag(Defines.Tags.MONSTER_TAG);
        if (monsters == null)
            return;

        foreach (var monster in monsters)
        {
            if (monster.TryGetComponent<MonsterController>(out var controller))
            {
                controller.TryTransitionState<BackMoveState>();
                controller.speedMultiplier = speedMultiplier;

                if (effectPrefab)
                {
                    var effect = Instantiate(effectPrefab);
                    controller.AddEffect(effect);
                    effect.LifeTime = duration;
                }
            }
        }

        base.UseItem();
    }

    public override void CancelItem()
    {
        var monsters = GameObject.FindGameObjectsWithTag(Defines.Tags.MONSTER_TAG);
        if (monsters == null)
            return;
        foreach (var monster in monsters)
        {
            if (monster.TryGetComponent<MonsterController>(out var controller))
                controller.TryTransitionState<PatrolState>();
            controller.speedMultiplier = 1f;
        }

        base.CancelItem();
    }
}
