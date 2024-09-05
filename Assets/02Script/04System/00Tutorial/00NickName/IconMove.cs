using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class IconMove : MonoBehaviour
{
    public Image icon;                    // 움직일 아이콘
    public Vector3 moveDistance = new Vector3(0, 20f, 0);  // 이동할 거리 (X, Y, Z 방향)
    public float moveDuration = 1f;       // 이동 시간
    public Ease moveEase = Ease.InOutQuad; // 이동의 Ease 타입

    private void Start()
    {
        MoveIcon();
    }

    private void MoveIcon()
    {
        RectTransform iconTransform = icon.rectTransform;

        // 현재 위치에서 X, Y, Z 방향으로 moveDistance 만큼 이동
        Vector3 startPosition = iconTransform.anchoredPosition3D;
        Vector3 endPosition = startPosition + moveDistance;

        iconTransform.DOAnchorPos3D(endPosition, moveDuration)
            .SetEase(moveEase)       // 이동을 부드럽게 설정
            .SetLoops(-1, LoopType.Yoyo); // 무한 반복, 원래 위치로 돌아오면서 반복
    }
}