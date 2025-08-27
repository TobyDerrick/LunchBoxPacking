#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("Layout/Circular Layout")]
public class CircularLayoutGroup : LayoutGroup
{
    [Header("Radial Settings")]
    public float radius = 100f;
    [Range(0f, 360f)] public float minAngle = 0f;
    [Range(0f, 360f)] public float maxAngle = 360f;
    public float startAngle = 0f;

    [Header("Animation Settings")]
    public float tweenDuration = 0.5f;
    public Ease tweenEase = Ease.OutQuad;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        ArrangeChildren(); 
    }

    public override void CalculateLayoutInputVertical() { }

    public override void SetLayoutHorizontal()
    {
        ArrangeChildren(Application.isPlaying);
    }

    public override void SetLayoutVertical()
    {
        ArrangeChildren(Application.isPlaying);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        ArrangeChildren(false); 
    }
#endif

    private void ArrangeChildren(bool animate = true)
    {
        m_Tracker.Clear();

        int childCount = rectChildren.Count;
        if (childCount == 0) return;

        float offsetAngle = (childCount > 1) ? (maxAngle - minAngle) / (childCount) : 0f;
        float angle = startAngle + 90;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = rectChildren[i];
            if (child == null) continue;

            m_Tracker.Add(this, child,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.Pivot);

            child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

            Vector3 targetPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f) * radius;

            if (animate && Application.isPlaying)
            {
                child.DOLocalMove(targetPos, tweenDuration).SetEase(tweenEase);
            }
            else
            {
                child.localPosition = targetPos;
            }

            angle += offsetAngle;
        }
    }

    public Tweener AnimateRadius(float targetRadius, float duration, Ease ease = Ease.OutQuad)
    {
        float startRadius = radius;
        return DOTween.To(() => startRadius, x =>
        {
            radius = x;
            ArrangeChildren(false);
        }, targetRadius, duration).SetEase(ease);


    }

    public void RefreshLayout()
    {
        SetLayoutHorizontal();
        SetLayoutVertical();
    }
}

