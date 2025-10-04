using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollButtonsAnimate : MonoBehaviour
{
    public Button targetButton;

    [Header("Pulse Settings")]
    public float pulseScale = 1.1f;
    public float pulseDuration = 0.5f;

    [Header("Hover Move Settings")]
    public float moveOffset = 10f;
    public float moveDuration = 0.8f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    void Awake()
    {
        if (targetButton != null)
        {
            originalScale = targetButton.transform.localScale;
            originalPosition = targetButton.transform.localPosition;
        }
    }

    void OnEnable()
    {
        if (targetButton == null) return;

        targetButton.transform.DOKill();
        targetButton.transform.DOScale(originalScale * pulseScale, pulseDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        targetButton.transform.DOLocalMoveY(originalPosition.y + moveOffset, moveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void OnDisable()
    {
        if (targetButton == null) return;

        targetButton.transform.localScale = originalScale;
        targetButton.transform.localPosition = originalPosition;
        targetButton.transform.DOKill();
    }
}
