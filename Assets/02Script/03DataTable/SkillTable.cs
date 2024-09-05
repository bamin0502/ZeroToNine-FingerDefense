using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Target { get; set; } // 0: 플레이어, 1: 몬스터 -> Center가 본인(0)인 경우를 제외하면 1차 타겟과 2차 타겟은 동일
    public int Projectile { get; set; } // 0: 근거리(즉발), 1: 원거리(투사체)
    public float Center { get; set; } // 0: 본인, 0 >: 타겟 -> 1차 타겟팅
    public int Type { get; set; } // 0: 단일, 1: 범위, 2: 장판
    public float Range { get; set; } // 스킬 범위 - 2차 타겟팅(범위, 장판)
    public float Damage { get; set; }
    public float CoolTime { get; set; }
    public float Duration { get; set; } // 설치 공격 지속 시간
    public float CastingTime { get; set; } // 캐스팅 도중 맞으면 스킬 증발 -> 프로토타입 이후에 고려
    public int IsDot { get; set; } // 도트 스킬
    public int BuffId { get; set; }
    public int AssetNo { get; set; }

    public override string ToString()
    {
        return $"{Id}: {Target} / {Projectile} / {Center} / {Type} / {Range} / {Damage} / {CoolTime} / {Duration} / {CastingTime} / {BuffId} / {AssetNo}";
    }

    public SkillData CreateNewSkillData()
    {
        var data = new SkillData()
        {
            Id = this.Id,
            Name = this.Name,
            Target = this.Target,
            Projectile = this.Projectile,
            Center = this.Center,
            Type = this.Type,
            Range = this.Range,
            Damage = this.Damage,
            CoolTime = this.CoolTime,
            Duration = this.Duration,
            CastingTime = this.CastingTime,
            IsDot = this.IsDot,
            BuffId = this.BuffId,
            AssetNo = this.AssetNo
        };

        return data;
    }
}

public class SkillTable : DataTable
{
    public Dictionary<int, SkillData> table = new Dictionary<int, SkillData>();

    public SkillData Get(int id)
    {
        return table.GetValueOrDefault(id);
    }

    public override void Load(string path)
    {
        path = string.Format(FormatPath, path);

        var textAsset = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<SkillData>();
            foreach (var record in records)
            {
                table.TryAdd(record.Id, record);
            }
        }
    }
}
