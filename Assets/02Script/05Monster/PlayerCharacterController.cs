using Spine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController : CombatEntity<CharacterStatus>, IControllable, ITargetable
{
    public PlayerCharacterSpawner spawner;

    public Transform[] mosnterPosition;

    public MonsterController monsterUp { get; set; }
    public MonsterController monsterDown { get; set; }

    private CharacterSpineAni anim;
    private TrackEntry deathTrackEntry;

    public Image elementImage;
    public TextMeshProUGUI monsterCountText;

    public int MonsterCount
    {
        get
        {
            int count = 0;
            if (monsterUp)
                count++;
            if (monsterDown)
                count++;
            return count;
        }
    }
    public bool IsTargetable => !IsDead;

    protected override void Awake()
    {
        base.Awake();
        entityType = EntityType.PLAYER_CHARACTER;

        anim = GetComponent<CharacterSpineAni>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        monsterUp = null;
        monsterDown = null;

        Status.Init();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();
        elementImage?.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (monsterCountText)
            monsterCountText.text = $"{MonsterCount} / 2";
    }

    public bool TryAddMonster(MonsterController monster)
    {
        if (monster == monsterUp || monster == monsterDown)
            return false;

        if (MonsterCount == 2)
            return false;

        if (!monsterUp)
            monsterUp = monster;
        else
            monsterDown = monster;

        monster.attackTarget = this;
        UpdateMonsterPosition();
        return true;
    }

    public bool TryRemoveMonster(MonsterController monster)
    {
        if (!monster)
            return false;

        if (monsterUp != monster && monsterDown != monster)
            return false;

        if (monsterUp == monster)
            monsterUp = null;
        else
            monsterDown = null;

        monster.attackMoveTarget = null;
        monster.attackTarget = null;
        UpdateMonsterPosition();
        return true;
    }

    private void UpdateMonsterPosition()
    {
        switch (MonsterCount)
        {
            case 0:
                return;
            case 1:
                {
                    if (!monsterUp)
                    {
                        monsterUp = monsterDown;
                        monsterDown = null;
                    }
                    monsterUp.attackMoveTarget = mosnterPosition[0];
                }
                break;
            case 2:
                {
                    monsterUp.attackMoveTarget = mosnterPosition[1];
                    monsterDown.attackMoveTarget = mosnterPosition[2];
                }
                break;
        }
    }

    public override void Die(DamageReason reason = DamageReason.NONE)
    {
        base.Die(reason);

        TryRemoveMonster(monsterUp);
        TryRemoveMonster(monsterDown);

        deathTrackEntry = anim.SetAnimation(CharacterSpineAni.CharacterState.PASSOUT, false, 1f);
        if (deathTrackEntry != null)
            deathTrackEntry.Complete += Die;
    }

    private void Die(TrackEntry trackEntry)
    {
        if (spawner)
            spawner.RemoveActiveCharacter(this);
        else
            Destroy(gameObject);
            
        if (deathTrackEntry != null)
            deathTrackEntry.Complete -= Die;
    }
}
