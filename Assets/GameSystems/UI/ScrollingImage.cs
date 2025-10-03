using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RawImage))]
public class ScrollingImage : MonoBehaviour
{
    [Header("Target")]
    public RawImage rawImage;

    [Header("Scroll Speed")]
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;

    [Header("Aspect Ratio")]
    [Tooltip("Preserve the texture aspect ratio inside the RawImage rect")]
    public bool preserveAspect = true;

    [Header("Scale")]
    [Tooltip("Zoom/scale factor applied on top of aspect preservation")]
    [Range(0.1f, 10f)]
    public float scale = 1f;

    private void OnEnable()
    {
        if (rawImage == null)
            rawImage = GetComponent<RawImage>();

        UpdateAspect();
    }

    private void Update()
    {
        if (rawImage == null || rawImage.texture == null)
            return;

        Rect rect = rawImage.uvRect;
        rect.x += scrollSpeedX * Time.deltaTime;
        rect.y += scrollSpeedY * Time.deltaTime;
        rawImage.uvRect = rect;

        if (preserveAspect)
            UpdateAspect();
    }

    private void UpdateAspect()
    {
        if (rawImage == null || rawImage.texture == null) return;

        float texAspect = (float)rawImage.texture.width / rawImage.texture.height;
        float rectAspect = rawImage.rectTransform.rect.width / rawImage.rectTransform.rect.height;

        Rect uv = rawImage.uvRect;

        if (rectAspect > texAspect)
        {
            float scaleFactor = rectAspect / texAspect;
            uv.width = scaleFactor / scale;
            uv.height = 1f / scale;
        }
        else
        {
            float scaleFactor = texAspect / rectAspect;
            uv.width = 1f / scale;
            uv.height = scaleFactor / scale;
        }

        rawImage.uvRect = uv;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rawImage == null)
            rawImage = GetComponent<RawImage>();

        if (preserveAspect)
            UpdateAspect();
    }
#endif
}
