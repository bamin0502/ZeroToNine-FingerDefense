using UnityEngine;
using Spine.Unity;
using Spine;

public class CharacterSpineAni : MonoBehaviour
{
    // public SkeletonAnimation[] skeletonAnimations;
    private SkeletonAnimation skeletonAnimation;
    public string[] skinNames;
    public AnimationReferenceAsset[] characterAnimClip;

    private Spine.AnimationState spineAnimationState;

    public CharacterState CurrentCharacterState { get; private set; }
    private string currentAnimation;

    [Tooltip("케이 캐릭터의 방패에 대한 레이어를 담아두는 변수")]
    private int originalSortingOrder;

    public TrackEntry CurrentTrackEntry { get; private set; }
    
    public enum CharacterState
    {
        ATTACK,
        ATTACK_SHEILD,
        HIT,
        IDLE,
        PASSOUT,
        RUN
    }

    private void Awake()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        CombineSkins();
        spineAnimationState = skeletonAnimation.AnimationState;
    }

    // private void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
    // {
    //     switch (currentAnimation)
    //     {
    //         case "ATTACK":
    //             {
    //                 if (e.Data.Name == "End")
    //                 {
    //                     SetAnimation(CharacterState.IDLE, true, 0.5f);
                        
    //                     if (skeletonAnimations.Length > 2)
    //                     {
    //                         skeletonAnimations[2].TryGetComponent(out MeshRenderer renderer);
    //                         renderer.sortingOrder = originalSortingOrder;
    //                     }
    //                 }
    //             }
    //             break;

    //         case "HIT":
    //             {
    //                 if (e.Data.Name == "End")
    //                 {
    //                     SetAnimation(CharacterState.IDLE, true, 0.5f);
                        
    //                     if (skeletonAnimations.Length > 2)
    //                     {
    //                         skeletonAnimations[2].TryGetComponent(out MeshRenderer renderer);
    //                         renderer.sortingOrder = originalSortingOrder;
    //                     }
    //                 }
    //             }
    //             break;
            
    //         case "PASSOUT":
    //             {
    //                 if (e.Data.Name == "End")
    //                 {
    //                     // characterController.IsDead = true;
    //                 }
    //             }
    //             break;
    //     }
    // }

    public void CombineSkins()
    {
        Skin combinedSkin = new Skin("Combined_Skin");

        foreach (var skinName in skinNames)
        {
            var skin = skeletonAnimation.Skeleton.Data.FindSkin(skinName);
            if (skin != null)
                combinedSkin.AddSkin(skin);
            else
                Logger.LogError($"Skin is not found: {skinName}");
        }

        // 결합된 스킨 적용
        // skeletonAnimations.Skeleton.SetSkin(combinedSkin);
        // skeletonAnimations.Skeleton.SetSlotsToSetupPose();

        // skeletonAnimation.Skeleton.SetSkin((Skin)null);
        // skeletonAnimation.Skeleton.SetSlotsToSetupPose();

        skeletonAnimation.Skeleton.SetSkin(combinedSkin);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();

        // skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
    }

    // public void SetAnimation(CharacterState state, bool loop, float timeScale)
    // {
    //     var clip = characterAnimClip[(int)state];
    //     if (clip.name.Equals(currentAnimation))
    //         return;

    //     foreach (var skeleton in skeletonAnimations)
    //     {
    //         skeleton.AnimationState.SetAnimation(0, clip, loop);
    //         skeleton.timeScale = timeScale;
    //     }

    //     currentAnimation = clip.name;
    //     // SetCharacterState(state);
    // }

    public TrackEntry SetAnimation(CharacterState state, bool loop, float timeScale)
    {
        if (CurrentCharacterState == CharacterState.PASSOUT && !CurrentTrackEntry.IsComplete)
        {
            Logger.LogError($"DEAD 애니메이션 중 {state} 애니메이션을 호출했습니다.");
            return null;
        }

        if (characterAnimClip.Length <= (int)state)
        {
            Logger.LogError("해당 애니메이션 클립이 없습니다.");
            return null;
        }
        
        if (currentAnimation == state.ToString())
            return null;
        
        CurrentCharacterState = state;
        currentAnimation = state.ToString();
        CurrentTrackEntry = spineAnimationState.SetAnimation(0, characterAnimClip[(int)state], loop);
        CurrentTrackEntry.TimeScale = timeScale;

        return CurrentTrackEntry;
    }

    // private void SetCharacterState(CharacterState state)
    // {
    //     switch (state)
    //     {
    //         case CharacterState.HIT:
    //         {
    //             if (skeletonAnimations.Length > 2)
    //             {
    //                 skeletonAnimations[2].TryGetComponent(out MeshRenderer renderer);
    //                 renderer.sortingOrder = 1;
    //             }
    //         }
    //             break;
    //         case CharacterState.ATTACK:
    //         {
    //             if (skeletonAnimations.Length > 2)
    //             {
    //                 skeletonAnimations[2].TryGetComponent(out MeshRenderer renderer);
    //                 renderer.sortingOrder = -1;
    //             }

    //         }
    //             break;
    //         case CharacterState.PASSOUT:
    //         {
    //             if (skeletonAnimations.Length > 2)
    //             {
    //                 skeletonAnimations[2].TryGetComponent(out MeshRenderer renderer);
    //                 renderer.sortingOrder = -1;
    //             }
    //         }
    //             break;
    //     }
    // }
}