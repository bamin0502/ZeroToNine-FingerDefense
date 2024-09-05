using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using CsvHelper;
using System.IO;
using System.Globalization;

public class AssetData
{
    public int AssetNo { get; set; }
    public string Name { get; set; }
}

public class AssetListTable : DataTable
{
    public Dictionary<int, string> table = new Dictionary<int, string>();

    public string Get(int assetNo)
    {
        return table.GetValueOrDefault(assetNo);
    }

    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);

        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<AssetData>();
            foreach (var record in records)
            {
                table.TryAdd(record.AssetNo, record.Name);
            }
        }
    }
}
