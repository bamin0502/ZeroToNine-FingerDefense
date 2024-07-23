using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MonsterSpawner : MonoBehaviour
{
    private MonsterFactory factory;

    public MonsterController monsterPrefab; // 추후 AssetReferenceMonster로 교체
    public Transform poolTransform;
    public Transform spawnTransform;
    private Vector2 spawnPosition;
    public float spawnRadius;

    private MonsterTable monsterTable;
    private WaveTable waveTable;

    public Transform moveTarget;

    private int stageId = Variables.LoadTable.stageId; // 테스트용. 나중에 다른 클래스의 static 변수로 변경.
    private int waveId = 1;
    public int MonsterCount { get; private set; }

    private float spawnWaveTimer = 0f;
    private WaveData currentWaveData;
    private bool isWaveTerm;
    [SerializeField] private float waveTerm = 3f;

    private bool isWaveEnd;


    private void Awake()
    {
        factory = new MonsterFactory();
        factory.monsterPrefab = monsterPrefab;
        factory.Init();

        monsterTable = DataTableManager.Get<MonsterTable>(DataTableIds.Monster);
        waveTable = DataTableManager.Get<WaveTable>(DataTableIds.Wave);

        // 스테이지 매니저의 내용
        // 스테이지 내 등장 몬스터 수(잘못된 id 몬스터는 나오지 않는걸로)
        // 죽거나 포털로 들어가면 카운트 다운 0
        stageId = 13001; // 테스트
        int wave = 1;
        MonsterCount = 0;
        HashSet<int> monsters = new();
        while (waveTable.Get(stageId, wave++) is { } waveData)
        {
            MonsterCount += waveData.Repeat;
            foreach (var monster in waveData.monsters)
            {
                monsters.Add(monster.monsterId);
            }
        }
        Logger.Log($"{stageId}스테이지의 몬스터 수: {MonsterCount}");

        // factory.Init(monsters);

        currentWaveData = waveTable.Get(stageId, waveId);
        if (currentWaveData != null)
        {
            spawnWaveTimer = 0f;
            isWaveTerm = true;
        }
    }

    private void OnEnable()
    {
        isWaveEnd = false;
    }

    private void Start()
    {
        factory.poolTransform = poolTransform; // To-Do: 추후 삭제
        spawnPosition = new Vector2(spawnTransform.position.x, spawnTransform.position.y);
    }

    private void Update()
    {
        if (MonsterCount <= 0 || !isWaveTerm || isWaveEnd)
            return;

        spawnWaveTimer += Time.deltaTime;
        if (spawnWaveTimer >= waveTerm)
        {
            spawnWaveTimer = 0f;
            isWaveTerm = false;

            SpawnRandomMonster().Forget();
        }
    }

    private async UniTask SpawnRandomMonster()
    {
        Logger.Log($"현재 웨이브: {waveId}, 이번 몬스터 수: {currentWaveData.Repeat}");

        var repeatCount = 0;
        var monsters = currentWaveData.monsters;
        while (repeatCount++ < currentWaveData.Repeat)
        {
            var spwanMonsterId = Utils.WeightedRandomPick<int>(monsters);
            var monsterGo = factory.GetMonster(monsterTable.Get(spwanMonsterId));
            monsterGo.transform.position = spawnPosition + Random.insideUnitCircle * spawnRadius;
            monsterGo.moveTarget = moveTarget;
            await UniTask.WaitForSeconds(currentWaveData.RepeatTerm);
        }

        isWaveTerm = true;
        waveTerm = currentWaveData.WaveTerm;

        currentWaveData = waveTable.Get(stageId, ++waveId);
        if (currentWaveData == null)
        {
            isWaveTerm = false;
            isWaveEnd = true;
            Logger.Log("모든 몬스터가 소환되었습니다.");
        }
    }
}
