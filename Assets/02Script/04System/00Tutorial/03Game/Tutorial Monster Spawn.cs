
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class MonsterSpawnInfo
{
    public int monsterID;      // 몬스터의 ID
    public int spawnCount;     // 생성할 개수
    public float spawnDelay;   // 생성 간의 딜레이
    public float moveDistance; // 몬스터가 이동할 거리
}

public class TutorialMonsterSpawn : TutorialBase
{
    public List<MonsterSpawnInfo> monsterSpawnInfos;
    public TutorialMonsterFactory monsterFactory;
    
    public override void Enter()
    {
        SpawnMonstersAsync().Forget();    
    }
    private async UniTaskVoid SpawnMonstersAsync()
    {
        foreach (var spawnInfo in monsterSpawnInfos)
        {
            for (int i = 0; i < spawnInfo.spawnCount; i++)
            {
                // 몬스터 생성
                var spawnedMonster = monsterFactory.CreateMonster(spawnInfo.monsterID);

                if (spawnedMonster != null)
                {
                    // 몬스터의 이동 거리 설정
                    Vector3 initialPosition = spawnedMonster.transform.position;
                    Vector3 targetPosition = initialPosition + Vector3.right * spawnInfo.moveDistance;
                    spawnedMonster.moveTargetPos = targetPosition;

                    // 몬스터를 이동 상태로 전환 (필요한 경우)
                    spawnedMonster.TryTransitionState<MoveState>();
                }

                // 생성 딜레이
                await UniTask.Delay((int)(spawnInfo.spawnDelay * 1000));
            }
        }
    }
    public override void Execute(TutorialController controller)
    {
        
    }

    public override void Exit()
    {
        
    }
}
