using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerCharacterData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Hp { get; set; }
    public int Grade { get; set; }
    public int Element { get; set; }
    public int Class { get; set; }
    public int Plus { get; set; } // 강화 수치
    public int Skill1 { get; set; }
    public int Skill2 { get; set; }
    
    public float RespawnCoolTime { get; set; } // 재소환 대기시간
    public int AssetNo { get; set; }
    public int Power { get; set; } // 전투력

    public int SkillIcon { get; set; } 
    public int Base { get; set; } 
    public int SkillName { get; set; } 
    public int SkillText { get; set; } 
}

public class PlayerCharacterTable : DataTable
{
    private Dictionary<int, PlayerCharacterData> table = new Dictionary<int, PlayerCharacterData>();

    public PlayerCharacterData Get(int id)
    {
        return table.GetValueOrDefault(id);    
    }
    
    public IEnumerable<PlayerCharacterData> GetAll()
    {
        return table.Values;
    }

    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);
        
        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();
        
        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<PlayerCharacterData>();
            foreach (var record in records)
            {
                table.TryAdd(record.Id, record);
            }
        }
    }
}
