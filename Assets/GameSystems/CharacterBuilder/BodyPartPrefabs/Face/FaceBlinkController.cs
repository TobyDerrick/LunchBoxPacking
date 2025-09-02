using UnityEngine;
using System.Collections;

public class FaceBlinkController : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private Renderer faceRenderer;
    [SerializeField] private int frameCount = 0;
    [SerializeField] private float blinkDuration = 0.1f;
    [SerializeField] private float minBlinkInterval = 2f;
    [SerializeField] private float maxBlinkInterval = 6f;

    private float nextBlinkTime;

    public void Initialize()
    {
        if (faceRenderer == null)
            faceRenderer = GetComponent<Renderer>();

        // Auto-detect frame count if 0
        if (frameCount <= 0 && faceRenderer.material.mainTexture != null)
        {
            Texture tex = faceRenderer.material.mainTexture;
            frameCount = tex.height / tex.width; // vertical frames, assuming square frames
        }

        if (frameCount <= 0) frameCount = 1;

        // Set texture scale
        faceRenderer.material.mainTextureScale = new Vector2(1f, 1f / frameCount);

        // Set initial offset to first frame (eyes open)
        faceRenderer.material.mainTextureOffset = new Vector2(0f, (frameCount - 1) / (float)frameCount);

        ScheduleNextBlink();
    }

    private void Update()
    {
        if (Time.time >= nextBlinkTime)
        {
            StartCoroutine(PlayBlink());
            ScheduleNextBlink();
        }
    }

    private void ScheduleNextBlink()
    {
        nextBlinkTime = Time.time + Random.Range(minBlinkInterval, maxBlinkInterval);
    }

    private IEnumerator PlayBlink()
    {
        for (int i = 0; i < frameCount; i++)
        {
            faceRenderer.material.mainTextureOffset = new Vector2(0f, (frameCount - 1 - i) / (float)frameCount);
            yield return new WaitForSeconds(blinkDuration);
        }

        // Reset to first frame (eyes open)
        faceRenderer.material.mainTextureOffset = new Vector2(0f, (frameCount - 1) / (float)frameCount);
    }
}
