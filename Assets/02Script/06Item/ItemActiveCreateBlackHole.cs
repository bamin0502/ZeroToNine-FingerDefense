using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Item/Active Create BlackHole", fileName = "Item.asset")]
public class ItemActiveCreateBlackHole : ActiveItem
{
    [Header("블랙홀 프리팹")]
    public BlackHole blackHolePrefab;

    public float scale;
    public Rect blackHoleRange;

    private bool isDragging;
    private InputManager inputManager;
    private DragAndDrop dragAndDrop;

    private BlackHole activeBlackHole;

    public override void Init()
    {
        base.Init();
        isDragging = false;

        var inputSystemGo = GameObject.FindWithTag("InputManager");
        dragAndDrop = inputSystemGo?.GetComponent<DragAndDrop>();
        inputManager = inputSystemGo?.GetComponent<InputManager>();
    }

    public override void UseItem()
    {
        if (!inputManager)
            return;
        
        isDragging = !isDragging;
        button.buttonEffect.gameObject.SetActive(isDragging);
        if (isDragging)
            inputManager.OnClick += PutBlackHole;
        else
            inputManager.OnClick -= PutBlackHole;
    }

    public override void CancelItem()
    {
        base.CancelItem();
        // if (activeBlackHole)
        //     Destroy(activeBlackHole.gameObject);
    }

    public override void UpdateItem()
    {
        base.UpdateItem();
    }

    private void PutBlackHole(InputAction.CallbackContext context)
    {
        var pos = Camera.main!.ScreenToWorldPoint(dragAndDrop.GetPointerPosition());
        if (pos.x < blackHoleRange.x || pos.x > blackHoleRange.x + blackHoleRange.width
            || pos.y < blackHoleRange.y || pos.y > blackHoleRange.y + blackHoleRange.height)
            return;

        isDragging = false;
        button.buttonEffect.gameObject.SetActive(false);
        if (inputManager)
            inputManager.OnClick -= PutBlackHole;

        pos.y = blackHoleRange.y + blackHoleRange.height / 2f;
        pos.z = pos.y;
        activeBlackHole = GameObject.Instantiate(blackHolePrefab, pos, Quaternion.identity);
        activeBlackHole.transform.localScale = Vector3.one * scale;
        activeBlackHole.LifeTime = duration;

        base.UseItem();
    }
}
