
public class MonsterStatus : BaseStatus
{
    private MonsterData data;
    public MonsterData Data
    {
        get => data;
        set
        {
            data = value;
            Init();
        }
    }

    public float CurrentAtk
    {
        get
        {
            if (Data == null)
                return 0f;

            if (buffHandler == null)
                return Data.AtkDmg;
            
            var currentAtk = Data.AtkDmg + buffHandler.buffValues[BuffType.ATK];
            return currentAtk > 0f ? currentAtk : 0f;
        }
    }

    public float CurrentAtkSpeed
    {
        get
        {
            if (Data == null)
                return 0f;

            if (buffHandler == null)
                return Data.AtkSpeed;
            
            var currentAtkSpeed = Data.AtkSpeed + buffHandler.buffValues[BuffType.ATK_SPEED];
            return currentAtkSpeed > 0f ? currentAtkSpeed : 0f;
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            if (Data == null)
                return 0f;

            if (buffHandler == null)
                return Data.MoveSpeed;
            
            var currentMoveSpeed = Data.MoveSpeed + buffHandler.buffValues[BuffType.MOVE_SPEED];
            return currentMoveSpeed > 0f ? currentMoveSpeed : 0f;
        }
    }

    public override void Init()
    {
        if (Data == null)
            return;

        currentMaxHp = Data.Hp;
        CurrentHp = currentMaxHp;
        element = (Elements)Data.Element;
    }
}
