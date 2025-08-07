using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
public class PixelateRenderPass : ScriptableRenderPass
{
    private static readonly int blockSizeId = Shader.PropertyToID("_BlockSize");
    private const string pixelateTextureName = "_PixelateTexture";

    private PixelateSettings defaultSettings;
    private Material material;

    public PixelateRenderPass(Material material, PixelateSettings defaultSettings)
    {
        this.material = material;
        this.defaultSettings = defaultSettings;
    }

    private void UpdateBlurSettings()
    {
        if(material == null) return;

        material.SetFloat(blockSizeId, defaultSettings.blockSize);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        if (resourceData.isActiveTargetBackBuffer)
            return;

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        var pixelateTextureDescriptor = srcCamColor.GetDescriptor(renderGraph);
        pixelateTextureDescriptor.name = pixelateTextureName;
        pixelateTextureDescriptor.depthBufferBits = 0;
        var dst = renderGraph.CreateTexture(pixelateTextureDescriptor);

        UpdateBlurSettings();

        if (!srcCamColor.IsValid() || !dst.IsValid()) return;

        RenderGraphUtils.BlitMaterialParameters paraPixelate = new(srcCamColor, dst, material, 0);
        renderGraph.AddBlitPass(paraPixelate, pixelateTextureName);
    }
 }