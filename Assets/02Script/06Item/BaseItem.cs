using UnityEngine;

public abstract class BaseItem : ScriptableObject
{
    [HideInInspector] public int id;
    [Header("아이템 설명")]
    [TextArea(3, 10)]
    public string itemDesc;

    private StageManager stageManager;
    protected StageManager StageMgr
    {
        get
        {
            if (!stageManager)
            {
                stageManager = GameObject.FindWithTag(Defines.Tags.STAGE_MANAGER_TAG)?.GetComponent<StageManager>();
            }

            return stageManager;
        }
    }

    [HideInInspector] public UiSlotButton button;
    [HideInInspector] public int count;

    public virtual bool IsPassive { get => true; }

    public virtual void Init()
    {
        // stageManager = null;
        // button = null;
        // count = 0;
    }

    public abstract void UseItem();
    public virtual void CancelItem() { }
    public virtual void UpdateItem() { }
}