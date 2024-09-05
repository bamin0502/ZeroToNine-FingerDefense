using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum BuffType
{
    NONE = -1,
    ATK_SPEED,
    MOVE_SPEED,
    DOT_HP, // 도트 딜
    ATK,
    MAX_HP, // 최대 체력 -> 
    COUNT
}

public class BuffData
{
    public int Id { get; set; }
    public float LastingTime { get; set; }
    public float DmgTerm { get; set; }
    public int EffectNo { get; set; }
    public List<(int type, float value)> BuffActions = new();
}

public class BuffTable : DataTable
{
    public Dictionary<int, BuffData> table = new();

    public BuffData Get(int id)
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
            csvReader.Read();
            var columnCount = csvReader.ColumnCount;
            int patternStartIndex = 5;

            while (csvReader.Read())
            {
                var buffData = new BuffData()
                {
                    Id = csvReader.GetField<int>(0),
                    LastingTime = csvReader.GetField<float>(2),
                    DmgTerm = csvReader.GetField<float>(3),
                    EffectNo = csvReader.GetField<int>(4)
                };

                for (int i = patternStartIndex; i < columnCount; i += 2)
                {
                    (int type, float value) buff = (csvReader.GetField<int>(i), csvReader.GetField<float>(i + 1));
                    buffData.BuffActions.Add(buff);
                }

                if (!table.TryAdd(buffData.Id, buffData))
                    Logger.Log($"중복된 버프 아이디가 존재합니다!: {buffData.Id}");
            }
        }
    }
}
