using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    private MonsterFactory factory;
    public Transform poolTransform;
    public Transform spawnTransform;
    private Vector2 spawnPosition;
    public float spawnRadius;

    private MonsterTable monsterTable;
    private WaveTable waveTable;

    private int stageId = Variables.LoadTable.StageId;
    private int waveId = 1;
    public int MonsterCount { get; private set; }

    private float spawnWaveTimer = 0f;
    private WaveData currentWaveData;
    private bool isWaveTerm;
    [SerializeField] private float waveTerm = 3f;

    private bool isWaveEnd;
    public HashSet<int> monsters { get; private set; } = new();
    
    private StageManager stageManager;
    
    //튜토리얼 용 변수
    public TutorialController gameTutorial;
    public TutorialController gameTutorial2;
    public TutorialController gameTutorial3;
    
    public event Action<MonsterController> onResetMonster;
    
    private void Awake()
    {
        factory = new MonsterFactory();
        //factory.monsterPrefab = monsterPrefab;
        //factory.Init();

        monsterTable = DataTableManager.Get<MonsterTable>(DataTableIds.Monster);
        waveTable = DataTableManager.Get<WaveTable>(DataTableIds.Wave);

        // 스테이지 매니저의 내용
        // 스테이지 내 등장 몬스터 수(잘못된 id 몬스터는 나오지 않는걸로)
        // 죽거나 포털로 들어가면 카운트 다운 0
        stageId = Variables.LoadTable.StageId; 
        int wave = 1;
        MonsterCount = 0;
        
        while (waveTable.Get(stageId, wave++) is { } waveData)
        {
            MonsterCount += waveData.Repeat;
            foreach (var monster in waveData.monsters)
            {
                monsters.Add(monster.monsterId);
            }
        }
        Logger.Log($"{stageId}스테이지의 몬스터 수: {MonsterCount}");
        factory.Init(monsters).Forget();

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
        stageManager = GameObject.FindWithTag("StageManager").GetComponent<StageManager>();
        
        factory.poolTransform = poolTransform; // To-Do: 추후 삭제
        var position = spawnTransform.position;
        spawnPosition = new Vector2(position.x, position.y);
    }

    private void Update()
    {
        if (MonsterCount <= 0 || !isWaveTerm || isWaveEnd )
            return;

        spawnWaveTimer += Time.deltaTime;
        if (spawnWaveTimer >= waveTerm)
        {
            spawnWaveTimer = 0f;
            isWaveTerm = false;

            if (Time.timeScale != 0)
            {
                SpawnRandomMonster().Forget();    
            }
        }
    }

    private async UniTask SpawnRandomMonster()
    {
        if (currentWaveData == null) return;
        //각 게임 튜토리얼에 맞춰서 몬스터를 소환한다. 
        if (DataManager.LoadFile().Game1TutorialCheck==false)
        {
            //게임1 튜토리얼
            //11001번에 해당하는 몬스터 한마리만 뽑는다.
            var monsterGo = factory.GetMonster(monsterTable.Get(11001));
            if (stageManager)
            {
                monsterGo.transform.position = spawnPosition + Random.insideUnitCircle * spawnRadius;
                monsterGo.moveTargetPos = Utils.GetRandomPositionBetweenTwoPositions(stageManager.castleLeftBottomPos.position, stageManager.castleRightTopPos.position);

                if (gameTutorial.gameObject.activeSelf)
                {
                    monsterGo.gameObject.AddComponent<TutorialGameTrigger>();
                } 
                
                var monsterController = monsterGo.GetComponent<MonsterController>();
                if (monsterController != null)
                {
                    monsterController.IsTutorialMonster = true; // 튜토리얼 몬스터로 설정
                }

                monsterGo.ResetMonsterData();
            }    
        }
        
        else if (DataManager.LoadFile().Game2TutorialCheck == false)
        {
            //게임2 튜토리얼
            for(var i=0; i<2; i++)
            { 
                var monsterGo = factory.GetMonster(monsterTable.Get(11001));
                if (stageManager)
                {
                    monsterGo.transform.position = spawnPosition + Random.insideUnitCircle * spawnRadius;
                    monsterGo.moveTargetPos = Utils.GetRandomPositionBetweenTwoPositions(stageManager.castleLeftBottomPos.position, stageManager.castleRightTopPos.position);
                    
                    if (monsterGo.transform.parent != poolTransform)
                    {
                        monsterGo.transform.SetParent(poolTransform);
                        Logger.Log($"Parenting Corrected: {monsterGo.transform.parent?.name ?? "No Parent"}");
                    }
                    // TutorialObserver를 직접 찾아서 AddMonster 호출 (수정 예정)
                    if (gameTutorial2.gameObject.activeSelf)
                    {
                        monsterGo.gameObject.AddComponent<TutorialGameTrigger>();
                    }
                    
                    var monsterController = monsterGo.GetComponent<MonsterController>();
                    if (!ReferenceEquals(monsterController, null))
                    {
                        monsterController.IsTutorialMonster = true; // 튜토리얼 몬스터로 설정
                    }

                    monsterGo.ResetMonsterData();
                }        
            }
            
        }
        
        else if (DataManager.LoadFile().Game3TutorialCheck == false)
        {
            var monsterGo = factory.GetMonster(monsterTable.Get(12031));
            if (stageManager)
            {
                monsterGo.transform.position = spawnPosition + Random.insideUnitCircle * spawnRadius;
                monsterGo.moveTargetPos = Utils.GetRandomPositionBetweenTwoPositions(stageManager.castleLeftBottomPos.position, stageManager.castleRightTopPos.position);

                if (gameTutorial3.gameObject.activeSelf)
                {
                    monsterGo.gameObject.AddComponent<TutorialGameTrigger>();
                } 
                
                var monsterController = monsterGo.GetComponent<MonsterController>();
                if (monsterController != null)
                {
                    monsterController.IsTutorialMonster = true; // 튜토리얼 몬스터로 설정
                }

                monsterGo.ResetMonsterData();
            }    
            
            for (int i = 0; i < 2; i++)
            {
                var monsterGo2 = factory.GetMonster(monsterTable.Get(11001));
                if (stageManager)
                {
                    monsterGo2.transform.position = spawnPosition + Random.insideUnitCircle * spawnRadius;
                    monsterGo2.moveTargetPos = Utils.GetRandomPositionBetweenTwoPositions(stageManager.castleLeftBottomPos.position, stageManager.castleRightTopPos.position);

                    if (gameTutorial3.gameObject.activeSelf)
                    {
                        monsterGo2.gameObject.AddComponent<TutorialGameTrigger>();
                    } 
                
                    var monsterController = monsterGo2.GetComponent<MonsterController>();
                    if (monsterController != null)
                    {
                        monsterController.IsTutorialMonster = true; // 튜토리얼 몬스터로 설정
                    }

                    monsterGo.ResetMonsterData();    
                }
                
            }
        }
        
        else
        {
            Logger.Log($"현재 웨이브: {waveId}, 이번 몬스터 수: {currentWaveData.Repeat}");
            var repeatCount = 0;
            var monsters = currentWaveData.monsters;

            while (repeatCount++ < currentWaveData.Repeat)
            {
                if ((stageManager?.CurrentState ?? StageState.NONE) == StageState.GAME_OVER)
                    break;

                var spwanMonsterId = Utils.WeightedRandomPick(monsters);
                var monsterGo = factory.GetMonster(monsterTable.Get(spwanMonsterId));

                if (stageManager)
                {
                    monsterGo.transform.position = spawnPosition + Random.insideUnitCircle * spawnRadius;
                    monsterGo.moveTargetPos = Utils.GetRandomPositionBetweenTwoPositions(stageManager.castleLeftBottomPos.position, stageManager.castleRightTopPos.position);
                    monsterGo.ResetMonsterData();
                }
                
                // 기존 반복 작업의 지연 시간 적용
                await UniTask.Delay(TimeSpan.FromSeconds(currentWaveData.RepeatTerm), ignoreTimeScale: false, cancellationToken: this.GetCancellationTokenOnDestroy());
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

    public void TriggerMonsterReset(MonsterController monster)
    {
        onResetMonster?.Invoke(monster);
    }
    
    //튜토리얼 몬스터 이동 상태 추가
    public void RespawnSpecificMonster(int targetMonsterId)
    {
        var monsterGo = factory.GetMonster(monsterTable.Get(targetMonsterId));
        if (stageManager != null)
        {
            // 위치 설정
            monsterGo.transform.position = spawnPosition + Random.insideUnitCircle * spawnRadius;
            monsterGo.moveTargetPos = Utils.GetRandomPositionBetweenTwoPositions(stageManager.castleLeftBottomPos.position, stageManager.castleRightTopPos.position);

            // `TutorialGameTrigger` 추가
            var tutorialTrigger = monsterGo.GetComponent<TutorialGameTrigger>();
            if (tutorialTrigger == null)
            {
                monsterGo.gameObject.AddComponent<TutorialGameTrigger>();
                Debug.Log($"TutorialGameTrigger 추가됨: {monsterGo.name}");
            }
            else
            {
                Debug.Log($"TutorialGameTrigger 이미 존재함: {monsterGo.name}");
            }

            // MonsterController 설정
            var monsterController = monsterGo.GetComponent<MonsterController>();
            if (monsterController != null)
            {
                monsterController.IsTutorialMonster = true; // 튜토리얼 몬스터로 설정

                // 몬스터의 초기 상태 및 데이터 리셋
                monsterController.ResetMonsterData();

                // 이동 상태의 초기화
                var moveState = new MoveState(monsterController);
                moveState.accumulatedDistance = 0f;
                moveState.isPaused = false;

                // 몬스터를 활성화하고 움직임 시작
                monsterController.gameObject.SetActive(true);
            }
        }    
    }
}
