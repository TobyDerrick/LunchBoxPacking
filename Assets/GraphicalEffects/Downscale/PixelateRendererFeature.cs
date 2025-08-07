using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[Serializable]
public class PixelateSettings
{
    [Range(0, 1000)] public int blockSize;
}
public class PixelateRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private PixelateSettings pixelateSettings;
    [SerializeField] private Material pixelateMaterial;
    private PixelateRenderPass pixelateRenderPass;

    public override void Create()
    {
        if (pixelateMaterial = null) return;
        pixelateRenderPass = new PixelateRenderPass(pixelateMaterial, pixelateSettings);

        pixelateRenderPass.renderPassEvent = RenderPassEvent.AfterRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pixelateRenderPass == null) return;

        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(pixelateRenderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (Application.isPlaying)
        {
            Destroy(pixelateMaterial);
        }
        else
        {
            DestroyImmediate(pixelateMaterial);
        }
    }
}
