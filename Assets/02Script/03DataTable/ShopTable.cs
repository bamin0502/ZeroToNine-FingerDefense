using System.Collections.Generic;
using UnityEngine;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Text;
using UnityEngine.AddressableAssets;

public class ShopData
{
    public int Id { get; set; }
    public string Text { get; set; }
}

public class ShopTable : DataTable
{
    private Dictionary<int,string> table = new Dictionary<int, string>();
    
    public bool IsExist(int id)
    {
        return table.ContainsKey(id);
    }
    
    public string Get(int id)
    {
        return table.GetValueOrDefault(id);
    }
    
    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);

        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();

        using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(textAsset.text)), Encoding.UTF8);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csvReader.GetRecords<ShopData>();
        foreach (var record in records)
        {
            string processedText = record.Text.Replace("\\n", "\n");
            table.TryAdd(record.Id, processedText);
        }
    }
}


