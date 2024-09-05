using System.Collections.Generic;
using UnityEngine;
using CsvHelper;
using UnityEngine.AddressableAssets;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration.Attributes;

public class GachaTable : DataTable
{
    public Dictionary<int, GachaData> table = new Dictionary<int, GachaData>();
    
    public GachaData Get(int id)
    {
        return table.GetValueOrDefault(id);
    }
    
    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);
        
        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();
        using var reader = new StringReader(textAsset.text);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<GachaData>();
        foreach (var record in records)
        {
            table.Add(record.Id, record);
        }
        
    }
}

public class GachaData
{
    public int Id { get; set; }
    [Name("Name")]
    public string NameId { get; set; }
    public int Grade { get; set; }
    public int AssetNo { get; set; }
    
}
