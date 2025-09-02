using UnityEngine;
using DG.Tweening;

public class BobObject : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [SerializeField] private float amplitude = 0.05f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    private Tween bobTween;

    private void OnEnable()
    {
        StartBobbing();
    }

    private void OnDisable()
    {
        StopBobbing();
    }

    private void OnDestroy()
    {
        StopBobbing();
    }

    private void StartBobbing()
    {
        bobTween?.Kill();
        bobTween = transform.DOLocalMoveY(amplitude, duration)
            .SetRelative(true)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(ease)
            .SetDelay(delay);
    }

    private void StopBobbing()
    {
        bobTween?.Kill();
        bobTween = null;
    }
}
