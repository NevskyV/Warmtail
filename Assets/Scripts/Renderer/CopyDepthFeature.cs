using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class DepthCopy2DFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader _depthCopyShader;
    [SerializeField] private RenderPassEvent _injectionPoint =
        RenderPassEvent.AfterRenderingOpaques;

    private Material _mMaterial;
    private DepthCopyPass _mPass;

    public override void Create()
    {
        if (_depthCopyShader == null)
            _depthCopyShader = Shader.Find("Hidden/DepthCopy2D");

        if (_depthCopyShader == null)
        {
            Debug.LogError("[DepthCopy2D] Shader not found!");
            return;
        }

        _mMaterial = CoreUtils.CreateEngineMaterial(_depthCopyShader);
        _mPass = new DepthCopyPass(_mMaterial)
        {
            renderPassEvent = _injectionPoint
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (_mPass != null && _mMaterial != null)
            renderer.EnqueuePass(_mPass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_mMaterial);
    }

    // ═══════════════════════════════════════════
    private class DepthCopyPass : ScriptableRenderPass
    {
        private static readonly int SCameraDepthTexID =
            Shader.PropertyToID("_CameraDepthTexture");
        private static readonly int SSourceDepthID =
            Shader.PropertyToID("_SourceDepth");

        private readonly Material _mMaterial;

        public DepthCopyPass(Material mat) => _mMaterial = mat;

        private class PassData
        {
            public TextureHandle Source;
            public TextureHandle Destination;
            public Material Material;
        }

        public override void RecordRenderGraph(
            RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData   = frameData.Get<UniversalCameraData>();

            TextureHandle depthSource = resourceData.activeDepthTexture;
            if (!depthSource.IsValid())
            {
                depthSource = resourceData.backBufferDepth;
                if (!depthSource.IsValid()) return;
            }

            // Целевая текстура — обычная color R32F
            var desc = new TextureDesc(
                cameraData.cameraTargetDescriptor.width,
                cameraData.cameraTargetDescriptor.height)
            {
                colorFormat     = GraphicsFormat.R32_SFloat,
                depthBufferBits = DepthBits.None,
                msaaSamples     = MSAASamples.None,
                filterMode      = FilterMode.Point,
                wrapMode        = TextureWrapMode.Clamp,
                name            = "_CameraDepthCopy",
                clearBuffer     = false
            };

            TextureHandle destination = renderGraph.CreateTexture(desc);

            // ── Unsafe Pass — полный контроль над CommandBuffer ──
            using (var builder = renderGraph.AddUnsafePass<PassData>(
                       "DepthCopy2D", out var passData))
            {
                passData.Source      = depthSource;
                passData.Destination = destination;
                passData.Material    = _mMaterial;

                builder.UseTexture(depthSource, AccessFlags.Read);
                builder.UseTexture(destination, AccessFlags.WriteAll);

                // После этого прохода _CameraDepthTexture = destination
                builder.SetGlobalTextureAfterPass(destination, SCameraDepthTexID);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc(static (PassData data,
                    UnsafeGraphContext ctx) =>
                {
                    CommandBuffer cmd =
                        CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);

                    // Явно указываем куда рисовать
                    cmd.SetRenderTarget(data.Destination);

                    // Явно привязываем depth как входную текстуру
                    cmd.SetGlobalTexture(SSourceDepthID, data.Source);

                    // Рисуем fullscreen triangle нашим шейдером
                    cmd.DrawProcedural(
                        Matrix4x4.identity,
                        data.Material,
                        0,
                        MeshTopology.Triangles,
                        3, 1);
                });
            }
        }

        private static readonly int SSourceDepthIDStatic =
            Shader.PropertyToID("_SourceDepth");
    }
}