using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class StagePanelController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform stagePanel;
    public float scaleFactor = 0.7f;
    public float animationDuration = 0.2f;
    private readonly int[] leftPaddings = { 200,-100,-375,-650,-1100 };
    
    private int currentIndex = 0;
    private Vector2 dragStartPosition;
    private HorizontalLayoutGroup layoutGroup;

    public GameObject stage1Tutorial;
    public GameObject stage2Tutorial;
    public GameObject stage3Tutorial;
    
    private void Start()
    {
        layoutGroup = stagePanel.GetComponent<HorizontalLayoutGroup>();
        SetCurrentIndexBasedOnClearedStages();
        UpdatePadding(true); 
        UpdateStageSlots(true);   
        
        Logger.Log($"해당 스테이지 인덱스 : {currentIndex}");
    }
    private void SetCurrentIndexBasedOnClearedStages()
    {
        // int lastClearedStage = 0;
        var stageClear = GameManager.instance.GameData.StageClearNum;
        
        // lastClearedStage에 따른 currentIndex 및 leftPadding 설정
        switch (stageClear)
        {
            case 13001:
                currentIndex = 1;
                leftPaddings[currentIndex] = -100;
                break;
            case 13002:
                currentIndex = 2;
                leftPaddings[currentIndex] = -375;
                break;
            case 13003:
                // 13004 스테이지가 보이도록 설정 (첫 번째 챕터의 끝)
                currentIndex = 3;
                leftPaddings[currentIndex] = -650;
                break;
            case 13004:
                currentIndex = 4;
                leftPaddings[currentIndex] = -1100;
                break;
            case 13005:
                currentIndex = 0;
                leftPaddings[currentIndex] = 200;
                break;
            case 13006:
                currentIndex = 1;
                leftPaddings[currentIndex] = -100;
                break;
            case 13007:
                currentIndex = 2;
                leftPaddings[currentIndex] = -375;
                break;
            case 13008:
                // 13004 스테이지가 보이도록 설정 (첫 번째 챕터의 끝)
                currentIndex = 3;
                leftPaddings[currentIndex] = -650;
                break;
            case 13009:
                currentIndex = 4;
                leftPaddings[currentIndex] = -1100;
                break;
            case 13010:
                currentIndex = 0;
                leftPaddings[currentIndex] = 200;
                break;
            case 13011:
                currentIndex = 1;
                leftPaddings[currentIndex] = -100;
                break;
            case 13012:
                currentIndex = 2;
                leftPaddings[currentIndex] = -375;
                break;
            case 13013:
                // 13004 스테이지가 보이도록 설정 (첫 번째 챕터의 끝)
                currentIndex = 3;
                leftPaddings[currentIndex] = -650;
                break;
            case 13014:
                currentIndex = 4;
                leftPaddings[currentIndex] = -1100;
                break;
            case 13015:
                currentIndex = 0;
                leftPaddings[currentIndex] = 200;
                break;
            case 13016:
                currentIndex = 1;
                leftPaddings[currentIndex] = -100;
                break;
            case 13017:
                currentIndex = 2;
                leftPaddings[currentIndex] = -375;
                break;
            case 13018:
                // 13004 스테이지가 보이도록 설정 (첫 번째 챕터의 끝)
                currentIndex = 3;
                leftPaddings[currentIndex] = -650;
                break;
            case 13019:
                currentIndex = 4;
                leftPaddings[currentIndex] = -1100;
                break;
            case 13020:
                currentIndex = 0;
                leftPaddings[currentIndex] = 200;
                break;
            default:
                currentIndex = 0;
                leftPaddings[currentIndex] = 200;
                break;
        }
    }


    private void OnEnable()
    {
       
        Logger.Log($"해당 스테이지 인덱스 : {currentIndex}");
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
    
    
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (stagePanel.childCount <= 1)
        {
            return;
        }
        
        Vector2 dragDelta = eventData.position - dragStartPosition;
        float minDragDistance = 50.0f;
        if (dragDelta.x > minDragDistance && currentIndex > 0)
        {
            currentIndex--;
        }
        else if (dragDelta.x < -minDragDistance && currentIndex < leftPaddings.Length - 1)
        {
            currentIndex++;
        }

        UpdatePadding();
        UpdateStageSlots();
    }

    private void UpdatePadding(bool instant = false)
    {
        if (stagePanel.childCount <= 1)
        {
            layoutGroup.padding.left = 0;
            LayoutRebuilder.ForceRebuildLayoutImmediate(stagePanel);
            return;
        }
        if (stage1Tutorial.gameObject.activeSelf)
        {
            currentIndex = 0;
        }
        else if (stage2Tutorial.gameObject.activeSelf)
        {
            currentIndex = 1;
        }  
        else if (stage3Tutorial.gameObject.activeSelf)
        {
            currentIndex = 2;
        }
        if (currentIndex < leftPaddings.Length)
        {
            if (instant)
            {
                layoutGroup.padding.left = leftPaddings[currentIndex];
                LayoutRebuilder.ForceRebuildLayoutImmediate(stagePanel);
            }
            else
            {
                DOTween.To(() => layoutGroup.padding.left, x => layoutGroup.padding.left = x, leftPaddings[currentIndex], animationDuration)
                    .SetEase(Ease.InOutQuad)
                    .OnUpdate(() =>
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(stagePanel);
                    });
            }
        }
    }

    private void UpdateStageSlots(bool instant = false)
    {
        for (var i = 0; i < stagePanel.childCount; i++)
        {
            var rect = stagePanel.GetChild(i).GetComponent<RectTransform>();
            float distanceFromCenter = Mathf.Abs(i - currentIndex); // 현재 인덱스와 각 슬롯의 인덱스 간의 거리 계산
            var scale = Mathf.Lerp(scaleFactor, 1.0f, 1.0f - Mathf.Clamp01(distanceFromCenter)); // 거리 기반 스케일 계산

            if (instant)
            {
                rect.localScale = new Vector3(scale, scale, 1);
            }
            else
            {
                rect.DOScale(new Vector3(scale, scale, 1), animationDuration).SetEase(Ease.InOutQuad);
            }

            var isInteractable = (distanceFromCenter < 1.0f);
            SetButtonsInteractable(rect, isInteractable);

            // 앵커와 피벗을 중앙으로 설정
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0); 
        }

        return;

        void SetButtonsInteractable(Transform slot, bool isInteractable)
        {
            var buttons = slot.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.interactable = isInteractable;
            }
        }
    }

}
