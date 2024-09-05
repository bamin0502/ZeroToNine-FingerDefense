using UnityEngine;

public abstract class ActiveItem : BaseItem
{
    public override bool IsPassive { get => false; }

    [Header("액티브 아이템")]
    public float duration;
    private bool isItemUsed = false;
    private float durationTimer = 0f;

    public float coolDown;
    private bool isCooledDown = true;
    private float coolDownTimer = 0f;

    public override void Init()
    {
        base.Init();
        
        isItemUsed = false;
        durationTimer = 0f;
        isCooledDown = true;
        coolDownTimer = 0f;
    }

    public override void UseItem()
    {
        count--;

        isItemUsed = true;
        durationTimer = 0f;

        isCooledDown = false;
        coolDownTimer = 0f;

        button.ActiveButton(false);
    }

    public override void CancelItem()
    {
        isItemUsed = false;
    }

    public override void UpdateItem()
    {
        UpdateDurationTimer();
        UpdateCoolDownTimer();
    }

    private void UpdateDurationTimer()
    {
        if (!isItemUsed)
            return;

        durationTimer += Time.deltaTime;
        if (durationTimer >= duration)
        {
            CancelItem();
        }
    }

    private void UpdateCoolDownTimer()
    {
        if (count == 0 || isCooledDown)
            return;

        coolDownTimer += Time.deltaTime;
        if (coolDown > 0f)
            button.SetFillAmountBackground(1f - coolDownTimer / coolDown);
        if (coolDownTimer >= coolDown)
        {
            button.ActiveButton(true);
            isCooledDown = true;
        }
    }
}
