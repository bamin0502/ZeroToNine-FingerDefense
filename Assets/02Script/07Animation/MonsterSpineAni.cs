using Spine;
using UnityEngine;
using Spine.Unity;

[RequireComponent(typeof(MonsterController))]
public class MonsterSpineAni : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] monsterAnimClip;
    
    private Spine.AnimationState spineAnimationState;
    
    // private MonsterController monsterController;
    
    public MonsterState CurrentMonsterState { get; private set; }
    public TrackEntry CurrentTrackEntry { get; private set; }
    private string currentAnimation;

    // public event Action monsterDeathEvent;

    public enum MonsterState
    {
        ATTACK,
        BESHOT,
        DEAD,
        IDLE,
        LAYDOWN_AFTER,
        WALK,
    }

    private void Awake()
    {
        skeletonAnimation = GetComponentInChildren(typeof(SkeletonAnimation)) as SkeletonAnimation;

        // monsterController = TryGetComponent(out MonsterController controller) ? controller : null;
        spineAnimationState = skeletonAnimation.AnimationState;
    }

    private void Start()
    {
        // SetAnimation(MonsterState.IDLE, true, 0.3f);
    }
    
    public TrackEntry SetAnimation(MonsterState state, bool loop, float timeScale)
    {
        if (CurrentMonsterState == MonsterState.DEAD && !CurrentTrackEntry.IsComplete)
        {
            Logger.LogError($"DEAD 애니메이션 중 {state} 애니메이션을 호출했습니다.");
            return null;
        }

        if (monsterAnimClip.Length <= (int)state)
        {
            Logger.LogError("해당 애니메이션 클립이 없습니다.");
            return null;
        }
        
        if (currentAnimation == state.ToString())
            return null;
        
        CurrentMonsterState = state;
        currentAnimation = state.ToString();
        var trackEntry = spineAnimationState.SetAnimation(0, monsterAnimClip[(int)state], loop);
        trackEntry.TimeScale = timeScale;

        CurrentTrackEntry = trackEntry;

        return trackEntry;
    }
}
