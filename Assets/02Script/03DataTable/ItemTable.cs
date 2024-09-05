using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using CsvHelper;

public class ItemTable : DataTable
{
    public Dictionary<int, ItemData> table = new Dictionary<int, ItemData>();
    
    public bool IsExist(int id)
    {
        return table.ContainsKey(id);
    }
    
    public ItemData Get(int id)
    {
        return table.GetValueOrDefault(id);
    }
    
    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);
        
        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();

        using var reader = new StringReader(textAsset.text);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<ItemData>();
        foreach (var record in records)
        {
            table.Add(record.Id, record);
        }
    }
}

public class ItemData
{
    public int Id { get; set; }
    public int NameId { get; set; }
    public int DescId { get; set; }
    public int ItemType { get; set; }
    public int Stage { get; set; }
    public int Price { get; set; }
    public int Skill { get; set; }
    public int Duration { get; set; }
    public float Value { get; set; }
    public int Cooldown { get; set; }
    public int Limit { get; set; }
    public int IconNo { get; set; }
    public int Purpose { get; set; }
    public string DataName { get; set; }
}