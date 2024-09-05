using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private bool isLifeTimeSet;
    private float timer;
    private float lifeTime;
    public float LifeTime
    {
        set
        {
            lifeTime = value;
            isLifeTimeSet = true;
        }
    }

    public float pullSpeed = 10f;  // 블랙홀로 빨아들이는 속도
    public float rotateSpeed = 200f;  // 오브젝트가 블랙홀 주위를 회전하는 속도 deg/sec
    public float threshold = 0.1f;

    [HideInInspector] public HashSet<MonsterController> targetMonsters = new();
    private HashSet<MonsterController> nonTargetMonsters = new();

    private void OnEnable()
    {
        isLifeTimeSet = false;
        timer = 0f;
    }

    private void OnDisable()
    {
        foreach (var monster in targetMonsters)
        {
            monster.transform.rotation = Quaternion.Euler(Vector3.zero);
            monster.TryTransitionState<PatrolState>();
        }

        targetMonsters.Clear();
        nonTargetMonsters.Clear();
    }

    private void Update()
    {
        TimerUpdate();
        UpdateTargetMonsters();
        UpdateNonTargetMonsters();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<MonsterController>(out var monster))
            return;
        if (monster.IsTargetable)
            AddTargetMonsters(monster);
        else
            nonTargetMonsters.Add(monster);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<MonsterController>(out var monster))
            return;

        if (nonTargetMonsters.Contains(monster))
            nonTargetMonsters.Remove(monster);
    }

    private void TimerUpdate()
    {
        if (!isLifeTimeSet)
            return;

        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateTargetMonsters()
    {
        List<MonsterController> removeMonsters = new ();
        foreach (var monster in targetMonsters)
        {
            if (!monster.IsTargetable)
            {
                removeMonsters.Add(monster);
                continue;
            }
            RotateAndPullObject(monster.gameObject);
        }

        foreach (var monster in removeMonsters)
        {
            targetMonsters.Remove(monster);
            monster.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void UpdateNonTargetMonsters()
    {
        List<MonsterController> removeMonsters = new ();
        foreach (var monster in nonTargetMonsters)
        {
            if (monster.IsTargetable)
            {
                removeMonsters.Add(monster);
                AddTargetMonsters(monster);
            }
        }

        foreach (var monster in removeMonsters)
        {
            nonTargetMonsters.Remove(monster);
        }
    }

    private void AddTargetMonsters(MonsterController monster)
    {
        targetMonsters.Add(monster);
        monster.TryTransitionState<IdleState<MonsterController>>();
        monster.monsterAni.CurrentTrackEntry.TimeScale = 0f;
    }

    private void RemoveTargetMonsters(MonsterController monster)
    {
        targetMonsters.Remove(monster);
        monster.transform.rotation = Quaternion.Euler(Vector3.zero);
        monster.TryTransitionState<PatrolState>();
    }

    private void RotateAndPullObject(GameObject other)
    {
        if (!other.TryGetComponent<MonsterController>(out var monster)
            || !monster.IsTargetable)
            return;

        Vector2 center = transform.position;

        if (Vector2.Distance(center, other.transform.position) > threshold)
        {
            Vector3 pullVec = (center - (Vector2)other.transform.position).normalized * pullSpeed * Time.deltaTime;
            other.transform.position += pullVec;
        }

        var rotatePos = Utils.RotatePosition(center, other.transform.position, rotateSpeed * Time.deltaTime);
        other.transform.position = rotatePos;

        // other.transform.RotateAround(transform.position, Vector3.forward, 360f * Time.deltaTime);
    }
}
