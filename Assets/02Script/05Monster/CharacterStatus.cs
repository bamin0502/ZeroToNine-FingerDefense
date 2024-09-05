
public class CharacterStatus : BaseStatus
{
    private PlayerCharacterData data;
    public PlayerCharacterData Data
    {
        get => data;
        set
        {
            data = value;
            Init();
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