using UnityEngine;

public class TutorialGameTrigger : MonoBehaviour
{
    private TutorialObserver observer;

    public void SetObserver(TutorialObserver observer)
    {
        this.observer = observer;
    }

    public void OnDragStarted()
    {
        observer.OnMonsterDragStarted(this);
    }

    public void OnDropped()
    {
        observer.OnMonsterDropped(this);
    }

    public void OnFallSurvived()
    {
        observer.OnMonsterSurvived(this);
    }

    private void OnDisabled()
    {
        observer.OnTargetDisabled(this);
    }
    public void OnFailButSurvived()
    {
        observer.OnMonsterSurvived(this);
    }
    private void OnDisable()
    {
        OnDisabled();
    }
    private void OnEnable()
    {
        // 오브젝트가 활성화될 때 Observer에 알림
        if (observer != null)
        {
            observer.AddMonster(this); // 오브젝트가 활성화되면 목록에 추가
        }
        else
        {
            Logger.LogWarning($"{gameObject.name} has no observer assigned.");
        }
    }
}
