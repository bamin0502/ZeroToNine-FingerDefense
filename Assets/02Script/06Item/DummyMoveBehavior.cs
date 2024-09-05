using UnityEngine;

public class DummyMoveBehavior : MonoBehaviour
{
    private enum DummyState
    {
        NONE = -1,
        IDLE,
        ATTACK,
        MOVE
    }

    [HideInInspector] public float moveSpeed = 5f;
    [HideInInspector] public float findRange = 5f;
    [HideInInspector] public float thresholdRange = 1f;

    private IFindable findBehavior;
    private GameObject target;
    private ITargetable targetable;

    private PlayerAttackBehavior attackBehavior;
    private CharacterSpineAni characterAni;

    private DummyState dummyState;

    [SerializeField] private bool isDirectedRight = true;
    private float defaultRightScale;

    public void Init()
    {
        findBehavior = new FindingTargetInCircle(transform, findRange, Defines.Layers.MONSTER_LAYER);
    }

    public void PlayAttack()
    {
        if (dummyState == DummyState.ATTACK)
            return;

        dummyState = DummyState.ATTACK;
        attackBehavior.enabled = true;
    }

    public void PlayMove()
    {
        if (dummyState == DummyState.MOVE)
            return;

        dummyState = DummyState.MOVE;
        attackBehavior.enabled = false;
        characterAni.SetAnimation(CharacterSpineAni.CharacterState.RUN, true, 1f);
    }

    public void PlayIdle()
    {
        if (target || dummyState == DummyState.IDLE)
            return;

        dummyState = DummyState.IDLE;
        attackBehavior.enabled = false;
        characterAni.SetAnimation(CharacterSpineAni.CharacterState.IDLE, true, 1f);
    }

    private void Awake()
    {
        characterAni = GetComponent<CharacterSpineAni>();
        attackBehavior = GetComponent<PlayerAttackBehavior>();

        defaultRightScale = isDirectedRight ? transform.localScale.x : -transform.localScale.x;
    }

    private void OnEnable()
    {
        dummyState = DummyState.NONE;
        target = null;
    }

    private void FixedUpdate()
    {
        if (!target || !(targetable?.IsTargetable ?? false)
            || Vector2.Distance(transform.position, target.transform.position) > findRange)
        {
            target = findBehavior?.FindTarget();
            targetable = target?.GetComponent<ITargetable>();
        }

        PlayIdle();
    }

    private void Update()
    {
        if (!target)
            return;

        SetFlip(target.transform.position.x > transform.position.x);

        if (Vector2.Distance(transform.position, target.transform.position) > thresholdRange)
        {
            PlayMove();
            var position = ((Vector2)(target.transform.position - transform.position)).normalized;
            transform.position += (Vector3)position * moveSpeed * Time.deltaTime;
        }
        else
        {
            PlayAttack();
        }
    }

    public void SetFlip(bool isRight)
    {
        var newScaleX = isRight ? defaultRightScale : defaultRightScale * -1f;
        var newScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);

        transform.localScale = newScale;
    }
}
