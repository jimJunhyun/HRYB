using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace GapperGames
{

    public class CloudShadows : ScriptableRendererFeature
    {
        private Material rendererMaterial;
        private FullScreenRenderPass fullScreenPass;

        public static bool isEnabled = false;
        public static bool enabledInPost = false;

        //Create Render Pass
        public override void Create()
        {
            //Initialize renderer material
            if (rendererMaterial == null)
            {
                rendererMaterial = (Material)Resources.Load("Rendering/CloudShadows");
            }

            //Initalize render pass
            fullScreenPass = new FullScreenRenderPass();
            fullScreenPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //Get SkyAndClouds settings
            SkyAndClouds sac = VolumeManager.instance.stack.GetComponent<SkyAndClouds>();
            isEnabled = sac.IsActive() && enabledInPost;

            if(!isEnabled){ return; }

            //Don't run if no renderer material
            if (rendererMaterial == null)
            {
                Debug.LogWarningFormat("Missing Post Processing effect Material. {0} Fullscreen pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            //Setup and execute render pass
            fullScreenPass.Setup(rendererMaterial, 0, "UpscalePass", renderingData);

            renderer.EnqueuePass(fullScreenPass);
        }

        //Clean up render pass
        protected override void Dispose(bool disposing)
        {
            fullScreenPass.Dispose();
        }

        //Render Pass class
        class FullScreenRenderPass : ScriptableRenderPass
        {
            private Material m_PassMaterial;
            private int m_PassIndex;
            private PassData m_PassData;
            private ProfilingSampler m_ProfilingSampler;
            private RTHandle m_CopiedColor;
            private static readonly int m_BlitTextureShaderID = Shader.PropertyToID("_BlitTexture");

            //I wonder if you can guess what this does...
            public void Setup(Material mat, int index, string featureName, in RenderingData renderingData)
            {
                //Initialize variables
                m_PassMaterial = mat;
                m_PassIndex = index;
                m_ProfilingSampler ??= new ProfilingSampler(featureName);

                //Initialize Camera Descriptor
                var colorCopyDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                colorCopyDescriptor.depthBufferBits = (int)DepthBits.None;
                RenderingUtils.ReAllocateIfNeeded(ref m_CopiedColor, colorCopyDescriptor, name: "_FullscreenPassColorCopy");

                m_PassData ??= new PassData();
            }

            //I feel like these function names are self explanatory :/
            public void Dispose()
            {
                m_CopiedColor?.Release();
            }

            //This one will execute YOU! (probably joking)
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                //Initialize Variables
                m_PassData.effectMaterial = m_PassMaterial;
                m_PassData.passIndex = m_PassIndex;
                m_PassData.profilingSampler = m_ProfilingSampler;
                m_PassData.copiedColor = m_CopiedColor;

                //I wonder...
                ExecutePass(m_PassData, ref renderingData, ref context);
            }

            //This one turns you into cheese or something?
            private static void ExecutePass(PassData passData, ref RenderingData renderingData, ref ScriptableRenderContext context)
            {
                //Initialize Variables
                var passMaterial = passData.effectMaterial;
                var passIndex = passData.passIndex;
                var copiedColor = passData.copiedColor;
                var profilingSampler = passData.profilingSampler;

                //General Null checks (for health and safety reasons)

                if (passMaterial == null)
                {
                    //Should not happen as we check it in feature
                    return;
                }

                if (renderingData.cameraData.isPreviewCamera)
                {
                    return;
                }

                //Get Command Buffer
                CommandBuffer cmd = CommandBufferPool.Get();

                var cameraData = renderingData.cameraData;

                //Render the Pass
                using (new ProfilingScope(cmd, profilingSampler))
                {
                    CoreUtils.SetRenderTarget(cmd, cameraData.renderer.cameraColorTargetHandle);
                    CoreUtils.DrawFullScreen(cmd, passMaterial);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
            }

            private class PassData
            {
                internal Material effectMaterial;
                internal int passIndex;
                public ProfilingSampler profilingSampler;
                public RTHandle copiedColor;
            }
        }

    }

}
