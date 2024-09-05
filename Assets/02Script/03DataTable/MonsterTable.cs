using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;

// TO-DO: 추후 위치 변경해야 합니다.
public enum Elements
{
    NONE = -1,
    SCISSOR,
    ROCK,
    PAPER
}

public class MonsterData
{
    public enum DragTypes
    {
        None = -1,
        BOSS,
        NORMAL,
        SPECIAL,
        Count
    }

    public int Id { get; set; }
    public float Hp{ get; set; }
    public int DragType { get; set; }
    public int Element { get; set; }
    public float MoveSpeed { get; set; }
    public float AtkDmg { get; set; }
    public float AtkSpeed { get; set; } // 초당 공격횟수
    // public bool IsRanger { get; set; } // 원거리 타입 여부
    // public float AtkRange { get; set; } // 원거리 몬스터 공격 거리
    public float Height { get; set; } // 즉사 높이
    public int Skill { get; set; }
    public int DragSkill { get; set; }
    public int DropGold { get; set; }
    public int AssetNo { get; set; }
    public string Name { get; set; }
    public string Info { get; set; }

    public override string ToString()
    {
        return $"{Id}: {Hp} / {(DragTypes)DragType} / {Element}";
    }
}

public class MonsterTable : DataTable
{
    public Dictionary<int, MonsterData> table = new Dictionary<int, MonsterData>();

    public bool IsExist(int id)
    {
        return table.ContainsKey(id);
    }

    public MonsterData Get(int id)
    {
        return table.GetValueOrDefault(id);
    }

    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);

        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();

        using var reader = new StringReader(textAsset.text);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csvReader.GetRecords<MonsterData>();
        foreach (var record in records)
        {
            table.TryAdd(record.Id, record);
        }
    }
}
