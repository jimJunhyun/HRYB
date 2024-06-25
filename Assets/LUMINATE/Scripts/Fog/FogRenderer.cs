using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GapperGames
{
    public enum FogPreset
    {
        Ultra,
        High,
        Medium,
        Low,
        Custom
    }
    public class FogRenderer : ScriptableRendererFeature
    {


        private Material rendererMaterial;
        private Material upscaleMaterial;

        private LowResRenderPass lowResPass;
        private FullScreenRenderPass fullScreenPass;

        public static bool isEnabled = false;

        public override void Create()
        {
            lowResPass = new LowResRenderPass();
            lowResPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            fullScreenPass = new FullScreenRenderPass();
            fullScreenPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Fog fog = VolumeManager.instance.stack.GetComponent<Fog>();
            isEnabled = fog.IsActive();

            if(!renderingData.postProcessingEnabled)
            {
                isEnabled = false;
            }

            if (!isEnabled) { return; }

            if (rendererMaterial == null)
            {
                rendererMaterial = (Material)Resources.Load("Rendering/FogRenderer");
            }
            if (upscaleMaterial == null)
            {
                upscaleMaterial = (Material)Resources.Load("Rendering/FogUpscaler");
            }

            upscaleMaterial.SetFloat("_BlurStrength", fog.blurStrength.value);
            upscaleMaterial.SetFloat("_BlurRadius", fog.blurRadius.value);
            upscaleMaterial.SetFloat("_BlurIterations", fog.blurIterations.value);
            upscaleMaterial.SetFloat("_AlphaMultiplier", fog.alphaMultiplier.value);

            rendererMaterial.SetColor("_Color", fog.fogColor.value);
            rendererMaterial.SetFloat("_Brightness", fog.brightness.value);
            rendererMaterial.SetFloat("_Density", fog.density.value);
            rendererMaterial.SetFloat("_Min_Density", fog.minDensity.value);
            rendererMaterial.SetFloat("_Anisotropy", fog.anisotropy.value);
            rendererMaterial.SetFloat("_Jitter", fog.jitter.value);
            rendererMaterial.SetFloat("_Steps", fog.steps.value);
            rendererMaterial.SetFloat("_StepSize", fog.stepSize.value);

            if (rendererMaterial == null)
            {
                //Debug.LogWarningFormat("Missing Post Processing effect Material. {0} Fullscreen pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }
            if (upscaleMaterial == null)
            {
                //Debug.LogWarningFormat("Missing Post Processing effect Material. {0} Fullscreen pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }
            lowResPass.Setup(rendererMaterial, "LowResPass", fog.downsample.value, fog.additionalLightsIntensityMultiplier.value, renderingData);
            fullScreenPass.Setup(upscaleMaterial, "UpscalePass", renderingData);

            renderer.EnqueuePass(lowResPass);
            renderer.EnqueuePass(fullScreenPass);
        }

        protected override void Dispose(bool disposing)
        {
            lowResPass.Dispose();
            fullScreenPass.Dispose();
        }

        class LowResRenderPass : ScriptableRenderPass
        {
            private Material m_PassMaterial;
            private PassData m_PassData;
            private ProfilingSampler m_ProfilingSampler;
            private RTHandle m_CopiedColor;
            private int downSample;
            private float additionalIntensity;

            private RTHandle FogTexture;
            Texture2D additionalLightsTex;

            public void Setup(Material mat, string featureName, int dsample, float additionalInt, in RenderingData renderingData)
            {
                m_PassMaterial = mat;
                m_ProfilingSampler ??= new ProfilingSampler(featureName);
                downSample = dsample;
                additionalIntensity = additionalInt;

                var colorCopyDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                colorCopyDescriptor.depthBufferBits = (int)DepthBits.None;
                RenderingUtils.ReAllocateIfNeeded(ref m_CopiedColor, colorCopyDescriptor, name: "_FullscreenPassColorCopy");

                FogTexture = RTHandles.Alloc("tst", name: "tst");

                m_PassData ??= new PassData();
            }

            public void Dispose()
            {
                m_CopiedColor?.Release();
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                //cmd.GetTemporaryRT(Shader.PropertyToID(FogTexture.name), cameraTextureDescriptor);
                //cmd.GetTemporaryRT(Shader.PropertyToID(FogTexture.name), cameraTextureDescriptor);
                cmd.SetGlobalTexture("_FogTexture", Shader.PropertyToID(FogTexture.name));
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;

                RenderTextureDescriptor descriptor = cameraTargetDescriptor;
                descriptor.msaaSamples = 1;
                descriptor.depthBufferBits = 0;

                cameraTargetDescriptor = descriptor;
                //int downSample = 8;
                cameraTargetDescriptor.width /= downSample;
                cameraTargetDescriptor.height /= downSample;
                cameraTargetDescriptor.colorFormat = RenderTextureFormat.DefaultHDR;

                //camTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
                cmd.GetTemporaryRT(Shader.PropertyToID(FogTexture.name), cameraTargetDescriptor, FilterMode.Bilinear);
                //cmd.SetGlobalTexture("_FogTexture", Shader.PropertyToID(FogTexture.name));
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                m_PassData.effectMaterial = m_PassMaterial;
                m_PassData.profilingSampler = m_ProfilingSampler;
                m_PassData.copiedColor = m_CopiedColor;

                ExecutePass(m_PassData, ref renderingData, ref context);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(Shader.PropertyToID(FogTexture.name));
            }

            // RG friendly method
            private void ExecutePass(PassData passData, ref RenderingData renderingData, ref ScriptableRenderContext context)
            {
                //Set up additional lights support

                //Vars
                VisibleLight[] sceneLights = renderingData.lightData.visibleLights.ToArray();
                List<VisibleLight> additionalLights = new List<VisibleLight>();
                List<Vector3> additionalLightsPositions = new List<Vector3>();
                List<Vector3> additionalLightsVectors = new List<Vector3>();
                List<Color> additionalLightsColors = new List<Color>();
                List<float> additionalLightsRanges = new List<float>();
                List<float> additionalLightsAngles = new List<float>();

                //Get lights
                for (int i = 0; i < sceneLights.Length; i++)
                {
                    if (sceneLights[i].lightType != LightType.Directional && sceneLights[i].lightType != LightType.Rectangle)
                    {
                        additionalLights.Add(sceneLights[i]);
                        additionalLightsPositions.Add(sceneLights[i].light.transform.position);
                        additionalLightsColors.Add(sceneLights[i].finalColor * additionalIntensity);
                        additionalLightsRanges.Add(sceneLights[i].range);

                        if (sceneLights[i].lightType == LightType.Spot)
                        {
                            additionalLightsAngles.Add(sceneLights[i].spotAngle);
                        }
                        else
                        {
                            additionalLightsAngles.Add(-1);
                        }

                        additionalLightsVectors.Add(sceneLights[i].light.transform.forward);
                    }
                }

                //Create Textures to pass to material
                additionalLightsTex = new Texture2D(512, 10, TextureFormat.RGBAFloat, false);
                additionalLightsTex.filterMode = FilterMode.Point;

                //Bake data into textures
                for (int x = 0; x < additionalLightsColors.Count; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (y == 0)
                        {
                            additionalLightsTex.SetPixel(x, y, additionalLightsColors[x]);
                        }
                        else if (y == 1)
                        {
                            additionalLightsTex.SetPixel(x, y, new Color(additionalLightsPositions[x].x, additionalLightsPositions[x].y, additionalLightsPositions[x].z, additionalLightsRanges[x]));
                        }
                        else if (y == 2)
                        {
                            additionalLightsTex.SetPixel(x, y, new Color(additionalLightsVectors[x].x, additionalLightsVectors[x].y, additionalLightsVectors[x].z, additionalLightsAngles[x]));
                        }
                    }
                }

                additionalLightsTex.Apply();

                //Rest of the render pass

                var rendererMaterial = passData.effectMaterial;
                var profilingSampler = passData.profilingSampler;

                if (rendererMaterial == null)
                {
                    // should not happen as we check it in feature
                    return;
                }

                if (renderingData.cameraData.isPreviewCamera)
                {
                    return;
                }

                //CommandBuffer cmd = renderingData.commandBuffer;
                CommandBuffer cmd = CommandBufferPool.Get();
                var cameraData = renderingData.cameraData;

                cmd.SetGlobalTexture("_AdditionalLightsTex", additionalLightsTex);
                cmd.SetGlobalFloat("_AdditionalLightCounts", additionalLightsColors.Count);
                rendererMaterial.SetFloat("_AdditionalCount", additionalLightsColors.Count);
                //cmd.SetGlobalTexture("_AdditionalPosInt", additionalLightsPosIntTex);

                using (new ProfilingScope(cmd, profilingSampler))
                {
                    //CoreUtils.SetRenderTarget(cmd, cameraData.renderer.GetCameraColorBackBuffer(cmd));
                    //CoreUtils.SetRenderTarget(cmd, cameraData.renderer.cameraColorTargetHandle);
                    CoreUtils.SetRenderTarget(cmd, FogTexture);
                    CoreUtils.DrawFullScreen(cmd, rendererMaterial);
                    context.ExecuteCommandBuffer(cmd);

                    cmd.SetGlobalTexture("_FogTexture", Shader.PropertyToID(FogTexture.name));

                    cmd.Clear();
                }
            }

            private class PassData
            {
                internal Material effectMaterial;
                public ProfilingSampler profilingSampler;
                public RTHandle copiedColor;
            }
        }

        //The Render Pass Class - poetry
        class FullScreenRenderPass : ScriptableRenderPass
        {
            private Material m_PassMaterial;
            private PassData m_PassData;
            private ProfilingSampler m_ProfilingSampler;
            private RTHandle m_CopiedColor;

            public void Setup(Material mat, string featureName, in RenderingData renderingData)
            {
                //Initialize Variables
                m_PassMaterial = mat;
                m_ProfilingSampler ??= new ProfilingSampler(featureName);

                var colorCopyDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                colorCopyDescriptor.depthBufferBits = (int)DepthBits.None;
                RenderingUtils.ReAllocateIfNeeded(ref m_CopiedColor, colorCopyDescriptor, name: "_FullscreenPassColorCopy");

                m_PassData ??= new PassData();
            }

            public void Dispose()
            {
                m_CopiedColor?.Release();
            }


            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                //Initialize Variables
                m_PassData.effectMaterial = m_PassMaterial;
                m_PassData.profilingSampler = m_ProfilingSampler;
                m_PassData.copiedColor = m_CopiedColor;

                //Destroys the universe
                ExecutePass(m_PassData, ref renderingData, ref context);
            }

            // RG friendly method
            private static void ExecutePass(PassData passData, ref RenderingData renderingData, ref ScriptableRenderContext context)
            {
                //Initialize Variables
                var passMaterial = passData.effectMaterial;
                var copiedColor = passData.copiedColor;
                var profilingSampler = passData.profilingSampler;

                //Null checks
                if (passMaterial == null)
                {
                    // should not happen as we check it in feature
                    return;
                }

                if (renderingData.cameraData.isPreviewCamera)
                {
                    return;
                }

                //Get Command Buffer
                CommandBuffer cmd = CommandBufferPool.Get();
                var cameraData = renderingData.cameraData;

                //Run the render pass
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
                public ProfilingSampler profilingSampler;
                public RTHandle copiedColor;
            }
        }

    }

}
