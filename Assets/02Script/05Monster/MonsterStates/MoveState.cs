using Spine;
using UnityEngine;

public class MoveState : IState
{
    private MonsterController controller;
    private Vector3 direction = new Vector3(-1f, 0f, 0f);

    private TrackEntry moveTrackEntry;
    
    //To-Do Tutorial 몬스터 이동 상태 추가
    private Vector3 startPosition; // 시작 위치를 저장하는 변수
    private float maxDistance = 10f;// 이동할 최대 거리
    public float accumulatedDistance = 0f;// 이동한 거리를 누적하는 변수
    public bool isPaused = false; // 이동을 멈추는 플래그

    public MoveState(MonsterController controller)
    {
        this.controller = controller;
    }

    public MoveState(MonsterController controller, Vector3 dir)
    {
        this.controller = controller;
        direction = dir;
    }

    public void Enter()
    {
        if (controller.IsTutorialMonster)
        {
            // 튜토리얼 몬스터일 경우만 초기화
            ResetTutorialMonsterState();
        }
        
        direction = Vector3.zero;
        
        //튜토리얼 몬스터 이동 상태 추가
        startPosition = controller.transform.position;
        isPaused = false;
        
        if (controller.monsterAni.CurrentMonsterState != MonsterSpineAni.MonsterState.WALK)
            moveTrackEntry = controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.WALK, true, controller.Status.CurrentMoveSpeed);
        else
            moveTrackEntry = controller.monsterAni.CurrentTrackEntry;
    }

    private void ResetTutorialMonsterState()
    {
        startPosition = controller.transform.position;
        accumulatedDistance = 0f;
        isPaused = false;
    }


    public void Update()
    {
        if (controller.IsTutorialMonster)
        {
            float distanceMoved = Vector3.Distance(startPosition, controller.transform.position);
            float totalDistance = accumulatedDistance + distanceMoved;

            if (totalDistance >= maxDistance)
            {
                // 이동을 멈추고 대기 상태로 전환
                controller.monsterAni.SetAnimation(MonsterSpineAni.MonsterState.IDLE, true, 1f);
                isPaused = true; // 이동을 멈추는 플래그 설정
                
                accumulatedDistance = maxDistance; // 거리를 최대값으로 설정하여 더 이상 이동하지 않게 함
                return;
            }
        }
        
        if (controller.moveTargetPos != null)
        {
            direction.x = controller.moveTargetPos.x
            > controller.transform.position.x ? 1f : -1f;
        }
        controller.SetFlip(direction.x > 0);

        controller.transform.position += direction * (controller.Status.CurrentMoveSpeed * Time.deltaTime);
        if (moveTrackEntry != null)
            moveTrackEntry.TimeScale = controller.Status.CurrentMoveSpeed;

        //튜토리얼 몬스터가 아닌 경우에만 패트롤 상태로 전환
        if (!controller.IsTutorialMonster)
        {
            if (controller.CanPatrol)
                controller.TryTransitionState<PatrolState>();    
        }
    }

    public void Exit()
    {
    }
    
    
}
