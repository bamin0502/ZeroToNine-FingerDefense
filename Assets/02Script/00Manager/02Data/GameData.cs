using System;
using System.Collections.Generic;

[Serializable]
public class GameData : IResourceSubject
{
    public string PlayerName = "";
    private int gold;
    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            NotifyObservers(ResourceType.Gold, gold);
        }
    }
    private int diamond;
    public int Diamond
    {
        get => diamond;
        set
        {
            diamond = value;
            NotifyObservers(ResourceType.Diamond, diamond);
        }
    }
    private int ticket;
    public int Ticket
    {
        get => ticket;
        set
        {
            ticket = value;
            NotifyObservers(ResourceType.Ticket, ticket);
        }
    }
    private int mileage;
    public int Mileage
    {
        get => mileage;
        set
        {
            mileage = value;
            NotifyObservers(ResourceType.Mileage, mileage);
        }
    }
    private bool nicknameCheck;
    public bool NicknameCheck
    {
        get => nicknameCheck;
        set
        {
            nicknameCheck = value;
            NotifyObservers(ResourceType.NicknameCheck, nicknameCheck ? 1 : 0);
        }
    }
    private bool stageChoiceTutorialCheck;
    public bool StageChoiceTutorialCheck
    {
        get => stageChoiceTutorialCheck;
        set
        {
            stageChoiceTutorialCheck = value;
            NotifyObservers(ResourceType.StageChoiceTutorialCheck, stageChoiceTutorialCheck ? 1 : 0);
        }
    }
    private bool deckUITutorialCheck;
    public bool DeckUITutorialCheck
    {
        get => deckUITutorialCheck;
        set
        {
            deckUITutorialCheck = value;
            NotifyObservers(ResourceType.DeckUITutorialCheck, deckUITutorialCheck ? 1 : 0);
        }
    }
    private bool game1TutorialCheck;
    public bool Game1TutorialCheck
    {
        get => game1TutorialCheck;
        set
        {
            game1TutorialCheck = value;
            NotifyObservers(ResourceType.Game1TutorialCheck, game1TutorialCheck ? 1 : 0);
        }
    }
    private bool game2TutorialCheck;
    public bool Game2TutorialCheck
    {
        get => game2TutorialCheck;
        set
        {
            game2TutorialCheck = value;
            NotifyObservers(ResourceType.Game2TutorialCheck, game2TutorialCheck ? 1 : 0);
        }
    }
    private bool game3TutorialCheck;
    public bool Game3TutorialCheck
    {
        get => game3TutorialCheck;
        set
        {
            game3TutorialCheck = value;
            NotifyObservers(ResourceType.Game3TutorialCheck, game3TutorialCheck ? 1 : 0);
        }
    }
    private bool shopDragTutorialCheck;
    public bool ShopDragTutorialCheck
    {
        get => shopDragTutorialCheck;
        set
        {
            shopDragTutorialCheck = value;
            NotifyObservers(ResourceType.ShopTutorialCheck, shopDragTutorialCheck ? 1 : 0);
        }
    }
    
    private bool shopGimmickTutorialCheck;
    public bool ShopGimmickTutorialCheck
    {
        get => shopGimmickTutorialCheck;
        set
        {
            shopGimmickTutorialCheck = value;
            NotifyObservers(ResourceType.ShopTutorialCheck, shopGimmickTutorialCheck ? 1 : 0);
        }
    }
    
    private bool shopCharacterTutorialCheck;
    public bool ShopCharacterTutorialCheck
    {
        get => shopCharacterTutorialCheck;
        set
        {
            shopCharacterTutorialCheck = value;
            NotifyObservers(ResourceType.ShopTutorialCheck, shopCharacterTutorialCheck ? 1 : 0);
        }
    }
    
    private bool shopFeatureTutorialCheck;
    public bool ShopFeatureTutorialCheck
    {
        get => shopFeatureTutorialCheck;
        set
        {
            shopFeatureTutorialCheck = value;
            NotifyObservers(ResourceType.ShopTutorialCheck, shopFeatureTutorialCheck ? 1 : 0);
        }
    }
    
    
    public int StageClearNum; //스테이지 클리어 한 최종 ID
    private int stageClearNum
    {
        get => StageClearNum;
        set => StageClearNum = value;
    }
    
    //플레이어가 소유한 가챠 ID
    public List<int> ObtainedGachaIDs = new List<int>();
    //플레이어가 소유한 캐릭터 ID
    public List<int> characterIds = new List<int>();
    //스테이지 클리어 여부
    public Dictionary<int, bool> StageClear = new();

    #region MonsterDrag
    public enum MonsterDrag
    {
        NONE = -1,
        LOCK=0,
        UNLOCK=1,
        ACTIVE=2,
    }
    MonsterDrag monsterDrag;
    public Dictionary<int, int> MonsterDragLevel = new ();
    

    #endregion
    
    #region MonsterGimmick
    public enum MonsterGimmick
    {
        NONE = -1,
        ATTACKRANGE = 0,
        ATTACKDAMAGE = 1,
        ATTACKDURATION = 2,
    }
    MonsterGimmick monsterGimmick;
    public List<(int monsterGimmick,int level)> MonsterGimmickLevel = new List<(int,int)>();
    public void UpdateMonsterGimmickLevel(int gimmick, int level)
    {
        for (var i = 0; i < MonsterGimmickLevel.Count; i++)
        {
            if(MonsterGimmickLevel[i].monsterGimmick == gimmick)
            {
                MonsterGimmickLevel[i] = (gimmick, level);
                return;
            }
        }
    }
    #endregion

    #region PlayerUpgrade
    public enum PlayerUpgrade
    {
        NONE = -1,
        CHARACTER_ARRANGEMENT = 3,
        PLAYER_HEALTH = 4,
        INCREASE_DRAG = 5,
    }
    PlayerUpgrade playerUpgrade;
    public List<(int playerUpgrade, int level)> PlayerUpgradeLevel = new List<(int,int)>();
    public void UpdatePlayerUpgradeLevel(int player, int level)
    {
        for (var i = 0; i < PlayerUpgradeLevel.Count; i++)
        {
            if(PlayerUpgradeLevel[i].playerUpgrade == player)
            {
                PlayerUpgradeLevel[i] = (player, level);
                return;
            }
        }
    }
    #endregion

    #region Item
    public void AddItem(int itemId, int itemCount)
    {
        //리스트로 인덱스 참조해서 개수 증가 시키도록 수정
        // 리스트에서 인덱스를 찾음
        int itemIndex = Items.FindIndex(item => item.itemId == itemId);
    
        if (itemIndex != -1)
        {
            // 기존 아이템이 있으면 수량을 합산
            var existingItem = Items[itemIndex];
            existingItem.itemCount += itemCount;
            Items[itemIndex] = existingItem; // 업데이트된 아이템을 다시 리스트에 저장
        }
        else
        {
            // 기존 아이템이 없으면 새로 추가
            Items.Add((itemId, itemCount));
            itemIndex = Items.Count - 1;
        }

        NotifyObservers(ResourceType.ItemId, itemId);
        NotifyObservers(ResourceType.ItemCount, Items[itemIndex].itemCount);
    }
    public List<(int itemId, int itemCount)> Items = new List<(int, int)>();
    #endregion
    
    #region ObserverPattern
    private List<IResourceObserver> observers = new List<IResourceObserver>();
    // 옵저버 패턴 관련 메서드
    public void RegisterObserver(IResourceObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IResourceObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public void NotifyObservers(ResourceType resourceType, int newValue)
    {
        foreach (var observer in observers)
        {
            observer.OnResourceUpdate(resourceType, newValue);
        }
    }
    

    #endregion
    

    public void Init()
    {
        var stageManager = DataTableManager.Get<StageTable>(DataTableIds.Stage);
        var stageIds = stageManager.GetKeys();
        foreach (var stageId in stageIds)
        {
            StageClear.Add(stageId, false);
        }
    }
}