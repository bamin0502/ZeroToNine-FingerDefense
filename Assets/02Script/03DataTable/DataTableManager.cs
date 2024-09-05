using System.Collections.Generic;

public static class DataTableIds
{
    public static readonly string Monster = "MonsterTable";
    public static readonly string Wave = "WaveTable";
    public static readonly string PlayerCharacter = "CharacterTable";
    public static readonly string Skill = "SkillTable";
    public static readonly string Buff = "BuffDebuffTable";
    public static readonly string Stage = "StageTable";
    public static readonly string Asset = "AssetListTable";
    public static readonly string String = "StringTable";
    public static readonly string Item = "ItemTable";
    public static readonly string Shop = "ShopTable";
    public static readonly string Gacha = "GachaTable";
    public static readonly string Upgrade = "UpgradeTable";

}

public static class DataTableManager
{
    private static Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableManager()
    {
        MonsterTable monsterTable = new();
        monsterTable.Load(DataTableIds.Monster);
        tables.Add(DataTableIds.Monster, monsterTable);

        WaveTable waveTable = new();
        waveTable.Load(DataTableIds.Wave);
        tables.Add(DataTableIds.Wave, waveTable);

        PlayerCharacterTable playerCharacterTable = new();
        playerCharacterTable.Load(DataTableIds.PlayerCharacter);
        tables.Add(DataTableIds.PlayerCharacter, playerCharacterTable);
        
        SkillTable skillTable = new();
        skillTable.Load(DataTableIds.Skill);
        tables.Add(DataTableIds.Skill, skillTable);

        BuffTable buffTable = new();
        buffTable.Load(DataTableIds.Buff);
        tables.Add(DataTableIds.Buff, buffTable);
        
        StageTable stageTable = new StageTable();
        stageTable.Load(DataTableIds.Stage);
        tables.Add(DataTableIds.Stage, stageTable);

        AssetListTable assetList = new AssetListTable();
        assetList.Load(DataTableIds.Asset);
        tables.Add(DataTableIds.Asset, assetList);
        
        StringTable stringTable = new StringTable();
        stringTable.Load(DataTableIds.String);
        tables.Add(DataTableIds.String, stringTable);
        
        ItemTable itemTable = new ItemTable();
        itemTable.Load(DataTableIds.Item);
        tables.Add(DataTableIds.Item, itemTable);
        
        ShopTable shopTable = new ShopTable();
        shopTable.Load(DataTableIds.Shop);
        tables.Add(DataTableIds.Shop, shopTable);
        
        GachaTable gachaTable = new GachaTable();
        gachaTable.Load(DataTableIds.Gacha);
        tables.Add(DataTableIds.Gacha, gachaTable);
        
        UpgradeTable upgradeTable = new UpgradeTable();
        upgradeTable.Load(DataTableIds.Upgrade);
        tables.Add(DataTableIds.Upgrade, upgradeTable);
        
    }
    
    public static T Get<T>(string id) where T : DataTable
    {
        if (tables.TryGetValue(id, out var value))
        {
            return value as T;
        }

        Logger.LogError($"해당 데이터 테이블의 내용이 없습니다: {id}");
        return null;
    }
}