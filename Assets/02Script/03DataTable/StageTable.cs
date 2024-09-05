using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CsvHelper;
using UnityEngine.AddressableAssets;
using System.Linq;

public class StageData
{
    public int StageId { get; set; }
    public int StageNameId { get; set; }
    public int Monster1Id { get; set; }
    public int Monster2Id { get; set; }
    public int Monster3Id { get; set; }
    public int Monster4Id { get; set; }
    public int AssetNo { get; set; }
    public int Reward1Id { get; set; }
    public int Reward1Value { get; set; }
    public int Reward2Id { get; set; }
    public int Reward2Value { get; set; }
}

public class StageTable : DataTable
{
    private Dictionary<int,StageData> table = new Dictionary<int, StageData>();
    
    public StageData Get(int id)
    {
        return table.GetValueOrDefault(id);
    }

    public List<int> GetKeys()
    {
        return table.Keys.ToList();
    }
    
    public IEnumerable<StageData> GetAll()
    {
        return table.Values;
    }
    
    public override void Load(string path)
    {
        path=string.Format(FormatPath, path);
        
        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();

        using var reader = new StringReader(textAsset.text);
        using var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
        var records = csvReader.GetRecords<StageData>();
        foreach (var record in records)
        {
            table.TryAdd(record.StageId, record);
        }
    }
}
