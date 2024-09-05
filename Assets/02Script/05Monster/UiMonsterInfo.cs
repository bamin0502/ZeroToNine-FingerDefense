using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UiMonsterInfo : MonoBehaviour
{
    public Button exitButton;
    public TextMeshProUGUI monsterNameText;
    public TextMeshProUGUI monsterDescText;

    private StageManager stageManager;
    private StringTable stringTable;

    private void Awake()
    {
        stageManager = GameObject.FindWithTag(Defines.Tags.STAGE_MANAGER_TAG)?.GetComponent<StageManager>();
        stringTable = DataTableManager.Get<StringTable>(DataTableIds.String);

        exitButton?.onClick.AddListener(() => HidePanel());
    }

    private void OnEnable()
    {
        ShowPanel();
    }

    public void SetMonsterInfoText(MonsterData data)
    {
        if (data == null)
            return;

        monsterNameText.text = stringTable.Get(data.Name);
        monsterDescText.text = stringTable.Get(data.Info);
    }

    private void ShowPanel()
    {
        transform.localScale = Vector3.one * 0.1f;

        var seq = DOTween.Sequence();
      
        seq.Append(transform.DOScale(1.1f, 0.2f)).SetUpdate(true);
        seq.Append(transform.DOScale(1f, 0.1f)).SetUpdate(true);

        seq.Play();
    }

    private void HidePanel()
    {
        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1.1f, 0.1f)).SetUpdate(true);
        seq.Append(transform.DOScale(0.2f, 0.2f)).SetUpdate(true);

        if (stageManager)
        {
            seq.Play().OnComplete(() => stageManager.CurrentState = StageState.PLAYING);
        }
    }
}
