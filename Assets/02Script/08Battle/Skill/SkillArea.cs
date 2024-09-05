using System.Collections.Generic;
using UnityEngine;

public class SkillArea : MonoBehaviour
{
    public PlacementSkill placementSkill;
    private float areaDuration;
    private float timer = 0f;
    private string targetTag;

    private bool isInit = false;

    public int effectCount = 15;

    public Dictionary<GameObject, Buff> Buffs { get; private set; } = new();
    private List<EffectController> effects = new();

    public void Init(PlacementSkill skill)
    {
        if (skill == null)
            return;

        switch (skill.Target)
        {
            case 0:
                targetTag = Defines.Tags.PLAYER_TAG;
                break;
            case 1:
                targetTag = Defines.Tags.MONSTER_TAG;
                break;
            default:
                return;
        }

        placementSkill = skill;
        areaDuration = skill.Duration;
        var collider = GetComponent<CircleCollider2D>();
        if (collider != null)
            collider.radius = skill.Radius;

        isInit = true;

        var isSoundPlayed = false;
        for (int i = 0; i < effectCount; i++)
        {
            var effect = EffectFactory.CreateEffect(skill.AssetId);
            if (effect != null)
            {
                effect.transform.position = Random.insideUnitCircle * skill.Radius + new Vector2(transform.position.x, transform.position.y);
                effect.transform.SetParent(transform);

                if (!isSoundPlayed && !effect.isAutoPlay)
                {
                    effect.PlayAudioClip(effect.enableAudioClip);
                    isSoundPlayed = true;
                }

                effects.Add(effect);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var buff in Buffs)
        {
            buff.Value.IsTimerStop = false;
        }
        Buffs.Clear();

        var isSoundPlayed = false;
        foreach (var effect in effects)
        {
            if (effect != null)
                Destroy(effect);

            if (!isSoundPlayed && !effect.isAutoPlay)
            {
                effect.PlayAudioClip(effect.disableAudioClip);
                isSoundPlayed = true;
            }
        }
        effects.Clear();
    }

    private void Update()
    {
        if (!isInit)
            return;

        timer += Time.deltaTime;
        if (timer >= areaDuration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag))
            return;

        if (other.TryGetComponent<ITargetable>(out var targetable))
        {
            if (targetable.IsTargetable)
            {
                placementSkill.EnterArea(other.gameObject, this);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag))
            return;

        if (Buffs.ContainsKey(other.gameObject))
            return;

        if (other.TryGetComponent<ITargetable>(out var targetable))
        {
            if (targetable.IsTargetable)
            {
                placementSkill.EnterArea(other.gameObject, this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        placementSkill.ExitArea(other.gameObject, this);
    }
}
