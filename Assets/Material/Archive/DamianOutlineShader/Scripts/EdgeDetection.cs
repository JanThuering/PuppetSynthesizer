using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeDetection : ScriptableRendererFeature
{
    private class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material material;

        private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int AngleFactorProperty = Shader.PropertyToID("_AngleFactor");
        private static readonly int DepthThresholdProperty = Shader.PropertyToID("_DepthThreshold");
        private static readonly int NormalThresholdProperty = Shader.PropertyToID("_NormalThreshold");
        private static readonly int LuminanceThresholdProperty = Shader.PropertyToID("_LuminanceThreshold");

        public EdgeDetectionPass()
        {
            profilingSampler = new ProfilingSampler(nameof(EdgeDetectionPass));
        }

        public void Setup(ref EdgeDetectionSettings settings, ref Material edgeDetectionMaterial)
        {
            material = edgeDetectionMaterial;
            renderPassEvent = settings.renderPassEvent;

            // material.SetFloat(OutlineThicknessProperty, settings.outlineThickness);
            // material.SetColor(OutlineColorProperty, settings.outlineColor);
            // material.SetFloat(AngleFactorProperty, settings.angleFactor);
            // material.SetFloat(DepthThresholdProperty, settings.depthThreshold);
            // material.SetFloat(NormalThresholdProperty, settings.normalThreshold);
            // material.SetFloat(LuminanceThresholdProperty, settings.luminanceThreshold);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var outlineCmd = CommandBufferPool.Get();

            using (new ProfilingScope(outlineCmd, profilingSampler))
            {
                CoreUtils.SetRenderTarget(outlineCmd, renderingData.cameraData.renderer.cameraColorTargetHandle);
                context.ExecuteCommandBuffer(outlineCmd);
                outlineCmd.Clear();

                Blitter.BlitTexture(outlineCmd, Vector2.one, material, 0);
            }

            context.ExecuteCommandBuffer(outlineCmd);
            CommandBufferPool.Release(outlineCmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }
        }
    }

    [Serializable]
    public class EdgeDetectionSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        [Range(0, 15)] public int outlineThickness = 3;
        public Color outlineColor = Color.black;
        public float angleFactor = 0.05f;
        public float depthThreshold = 0.005f;
        public float normalThreshold = 0.25f;
        public float luminanceThreshold = 0.5f;
    }

    [SerializeField] private EdgeDetectionSettings settings;
    public Material edgeDetectionMaterial;
    private EdgeDetectionPass edgeDetectionPass;

    /// <summary>
    /// Called
    /// - When the Scriptable Renderer Feature loads the first time.
    /// - When you enable or disable the Scriptable Renderer Feature.
    /// - When you change a property in the Inspector window of the Renderer Feature.
    /// </summary>
    public override void Create()
    {
        edgeDetectionPass = new EdgeDetectionPass();
    }

    /// <summary>
    /// Called
    /// - Every frame, once for each camera.
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Don't render for some views.
        if (renderingData.cameraData.cameraType is CameraType.Preview or CameraType.Reflection) return;

        edgeDetectionPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        edgeDetectionPass.Setup(ref settings, ref edgeDetectionMaterial);

        renderer.EnqueuePass(edgeDetectionPass);
    }

    /// <summary>
    /// Clean up resources allocated to the Scriptable Renderer Feature such as materials.
    /// </summary>
    override protected void Dispose(bool disposing)
    {
        edgeDetectionPass = null;
        CoreUtils.Destroy(edgeDetectionMaterial);
    }
}
