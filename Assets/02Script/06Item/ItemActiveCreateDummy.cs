using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Item/Active Create Dummy", fileName = "Item.asset")]
public class ItemActiveCreateDummy : ActiveItem
{
    [Header("이펙트")]
    public EffectController entryEffectPrefab;
    public EffectController exitEffectPrefab;
    [Header("신의 대리인")]
    public PlayerCharacterController characterPrefab;
    public Rect spawnRange;

    [Header("신의 대리인의 스탯")]
    public float hp;
    private Elements element = Elements.NONE;
    public int skillId1;
    public int skillId2;

    public float moveSpeed;
    public float findRange;
    public float thresholdRange;

    private bool isDragging;
    private InputManager inputManager;
    private DragAndDrop dragAndDrop;

    private SkillTable skillTable;

    private PlayerCharacterController activeAgent;

    public override void Init()
    {
        base.Init();
        isDragging = false;

        var inputSystemGo = GameObject.FindWithTag("InputManager");
        dragAndDrop = inputSystemGo?.GetComponent<DragAndDrop>();
        inputManager = inputSystemGo?.GetComponent<InputManager>();

        skillTable = DataTableManager.Get<SkillTable>(DataTableIds.Skill);
    }

    public override void UseItem()
    {
        if (!inputManager)
            return;
        
        isDragging = !isDragging;
        button.buttonEffect.gameObject.SetActive(isDragging);
        if (isDragging)
            inputManager.OnClick += CreateDummy;
        else
            inputManager.OnClick -= CreateDummy;
    }

    public override void CancelItem()
    {
        base.CancelItem();

        if (activeAgent != null)
        {
            var exitEffect = Instantiate(exitEffectPrefab, activeAgent.transform.position, Quaternion.identity);
            exitEffect.LifeTime = 1f;
            Destroy(activeAgent.gameObject);
        }
    }

    public override void UpdateItem()
    {
        base.UpdateItem();
    }

    private void CreateDummy(InputAction.CallbackContext context)
    {
        var spawnPos = Camera.main!.ScreenToWorldPoint(dragAndDrop.GetPointerPosition());
        if (spawnPos.x < spawnRange.x || spawnPos.x > spawnRange.x + spawnRange.width
            || spawnPos.y < spawnRange.y || spawnPos.y > spawnRange.y + spawnRange.height)
            return;
            
        isDragging = false;
        button.buttonEffect.gameObject.SetActive(false);
        if (inputManager)
            inputManager.OnClick -= CreateDummy;
            
        spawnPos.z = spawnPos.y;
        var instantiatedCharacter = Instantiate(characterPrefab, spawnPos, Quaternion.identity);

        var data = new PlayerCharacterData
        {
            Hp = hp,
            Element = (int)element,
            Skill1 = skillId1,
            Skill2 = skillId2
        };

        instantiatedCharacter.Status.Data = data;
        
        if (instantiatedCharacter.TryGetComponent<PlayerAttackBehavior>(out var attackBehavior))
        {
            var normalAttackData = skillTable.Get(data.Skill1);
            var skillAttackData = skillTable.Get(data.Skill2);

            var normalAttack = SkillFactory.CreateSkill(normalAttackData, instantiatedCharacter.gameObject);
            var skillAttack = SkillFactory.CreateSkill(skillAttackData, instantiatedCharacter.gameObject);

            attackBehavior.normalAttack = normalAttack;
            attackBehavior.SkillAttack = skillAttack;
        }

        if (instantiatedCharacter.TryGetComponent<DummyMoveBehavior>(out var moveBehavior))
        {
            moveBehavior.moveSpeed = moveSpeed;
            moveBehavior.findRange = findRange;
            moveBehavior.thresholdRange = thresholdRange;

            moveBehavior.Init();
        }

        var entryEffect = Instantiate(entryEffectPrefab, spawnPos, Quaternion.identity);
        entryEffect.LifeTime = 1f;
        activeAgent = instantiatedCharacter;

        button.buttonEffect.gameObject.SetActive(false);
        base.UseItem();
    }
}
