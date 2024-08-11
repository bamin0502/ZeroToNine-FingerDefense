using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private DataManager dataManager;

    private string playerName;
    private bool nicknameCheck;
    private bool stageChoiceTutorialCheck;
    private bool deckUITutorialCheck;
    private bool game1TutorialCheck;
    private bool game2TutorialCheck;
    private bool game3TutorialCheck;
    private bool game4TutorialCheck;
    private int gold;
    private int diamond;
    private int ticket;
    private int mileage;
    private List<int> obtainedGachaIDs = new List<int>();
    private List<(int itemId, int itemCount)> items = new List<(int, int)>();
    public event Action OnResourcesChanged;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        Application.targetFrameRate = 60;

        dataManager = GetComponent<DataManager>();
    }

    private void Start()
    {
        LoadGameData();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ResetGameData();
            Logger.Log("Game data has been reset.");
        } 
        
        if(Input.GetKeyDown(KeyCode.F1))
        {
            TestCode();
        }
#endif
    }

    private void LoadGameData()
    {
        GameData gameData = dataManager.LoadFile<GameData>("GameData.json") ?? new GameData();

        playerName = gameData.PlayerName;
        nicknameCheck = gameData.NicknameCheck;
        stageChoiceTutorialCheck = gameData.StageChoiceTutorialCheck;
        deckUITutorialCheck = gameData.DeckUITutorialCheck;
        game1TutorialCheck = gameData.Game1TutorialCheck;
        game2TutorialCheck = gameData.Game2TutorialCheck;
        game3TutorialCheck = gameData.Game3TutorialCheck;
        game4TutorialCheck = gameData.Game4TutorialCheck;
        gold = gameData.Gold;
        diamond = gameData.Diamond;
        ticket = gameData.Ticket;
        mileage = gameData.Mileage;
        obtainedGachaIDs = gameData.ObtainedGachaIDs;
        items = gameData.ItemId;
        
        OnResourcesChanged?.Invoke();
    }

    public void SaveGameData()
    {
        GameData gameData = new GameData
        {
            PlayerName = playerName,
            NicknameCheck = nicknameCheck,
            StageChoiceTutorialCheck = stageChoiceTutorialCheck,
            DeckUITutorialCheck = deckUITutorialCheck,
            Game1TutorialCheck = game1TutorialCheck,
            Game2TutorialCheck = game2TutorialCheck,
            Game3TutorialCheck = game3TutorialCheck,
            Game4TutorialCheck = game4TutorialCheck,
            Gold = gold,
            Diamond = diamond,
            Ticket = ticket,
            Mileage = mileage,
            ObtainedGachaIDs = obtainedGachaIDs,
            ItemId = items
        };
        dataManager.SaveFile("GameData.json", gameData);
    }
    
    public string PlayerName
    {
        get => playerName;
        set
        {
            playerName = value;
            SaveGameData();
        }
    }

    public bool NicknameCheck
    {
        get => nicknameCheck;
        set
        {
            nicknameCheck = value;
            SaveGameData();
        }
    }

    public bool StageChoiceTutorialCheck
    {
        get => stageChoiceTutorialCheck;
        set
        {
            stageChoiceTutorialCheck = value;
            SaveGameData();
        }
    }

    public bool DeckUITutorialCheck
    {
        get => deckUITutorialCheck;
        set
        {
            deckUITutorialCheck = value;
            SaveGameData();
        }
    }

    public bool Game1TutorialCheck
    {
        get => game1TutorialCheck;
        set
        {
            game1TutorialCheck = value;
            SaveGameData();
        }
    }
    
    public bool Game2TutorialCheck
    {
        get => game2TutorialCheck;
        set
        {
            game2TutorialCheck = value;
            SaveGameData();
        }
    }
    
    public bool Game3TutorialCheck
    {
        get => game3TutorialCheck;
        set
        {
            game3TutorialCheck = value;
            SaveGameData();
        }
    }
    
    public bool Game4TutorialCheck
    {
        get => game4TutorialCheck;
        set
        {
            game4TutorialCheck = value;
            SaveGameData();
        }
    }
    
    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            SaveGameData();
            OnResourcesChanged?.Invoke();
        }
    }
    
    public int Diamond
    {
        get { return diamond; }
        set
        {
            diamond = value;
            SaveGameData();
            OnResourcesChanged?.Invoke();
        }
    }
    
    public int Ticket
    {
        get { return ticket; }
        set
        {
            ticket = value;
            SaveGameData();
            OnResourcesChanged?.Invoke();
        }
    }
    
    public int Mileage
    {
        get => mileage;
        set
        {
            mileage = value;
            SaveGameData();
            OnResourcesChanged?.Invoke();
        }
    }
    
    public List<int> ObtainedGachaIDs
    {
        get => obtainedGachaIDs;
        set
        {
            obtainedGachaIDs = value;
            SaveGameData();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainScene":
                break;
            case "Game Scene":
                break;
        }
    }

    //테스트용 코드 삭제 예정
    private void ResetGameData()
    {
        // 데이터 초기화
        playerName = "";
        nicknameCheck = false;
        stageChoiceTutorialCheck = false;
        deckUITutorialCheck = false;
        game1TutorialCheck = false;
        game2TutorialCheck = false;
        game3TutorialCheck = false;
        game4TutorialCheck = false;
        gold = 0;
        diamond = 0;
        ticket = 0;
        mileage = 0;
        obtainedGachaIDs.Clear();
        
        SceneManager.LoadScene(0);
        // 초기화된 데이터 저장
        SaveGameData();
    }

    private void TestCode()
    {
        gold += 1000;
        diamond += 1000;
        ticket += 10;
        mileage += 20;
        
        Logger.Log($"TestCode executed: Gold={gold}, Diamond={diamond}, Ticket={ticket}, Mileage={mileage}");
        SaveGameData();
        OnResourcesChanged?.Invoke();
    }
    
    public void AddTickets(int amount)
    {
        Ticket += amount;
    }

    public void RemoveTickets(int amount)
    {
        Ticket -= amount;
    }

    public void AddDiamonds(int amount)
    {
        Diamond += amount;
        SaveGameData();
        OnResourcesChanged?.Invoke();
    }

    public void RemoveDiamonds(int amount)
    {
        Diamond -= amount;
        SaveGameData();
        OnResourcesChanged?.Invoke();
    }
    
    public void AddItem(int itemId, int itemCount)
    {
        var item = items.Find(i => i.itemId == itemId);
        if (item != default)
        {
            int index = items.IndexOf(item);
            items[index] = (itemId, item.itemCount + itemCount);
        }
        else
        {
            items.Add((itemId, itemCount));
        }

        // 저장 로직 추가
        SaveGameData();
    }

    public void RemoveGold(int totalCost)
    {
        Gold -= totalCost;
        if (Gold < 0) Gold = 0;
        SaveGameData();
    }

    public void AddGold(int goldAmount)
    {
        Gold += goldAmount;
        SaveGameData();
        OnResourcesChanged?.Invoke();
    }
}
