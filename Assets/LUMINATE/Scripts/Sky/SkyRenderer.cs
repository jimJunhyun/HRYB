using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GapperGames
{
    public enum CloudQuality
    {
        Ultra,
        High,
        Medium,
        Low,
        Custom
    }

    public enum CloudType
    {
        Cloudy,
        Sparse,
        Stormy,
        Overcast,
        HighAltitude,
        Custom,
        None
    }

    public class SkyRenderer : ScriptableRendererFeature
    {
        private CloudData cloudy;
        private CloudData sparse;
        private CloudData stormy;
        private CloudData overcast;
        private CloudData highAltitude;
        private CloudData custom1;
        private CloudData custom2;
        private CloudData currentCloudData;
        private CloudData currentCloudData1;

        private CompositingSettings compositing;
        private SkySettings sky;
        private CloudSettings clouds;

        private CloudRenderPass renderPass;
        private Material upscaleMaterial;
        private Material skyMaterial;
        private LayerMask skyLayer;

        public static bool isEnabled = false;

        public override void Create()
        {
            string[] layerNames = new string[1];
            layerNames[0] = "UI";
            skyLayer = LayerMask.GetMask(layerNames);

            #region GetCloudData
            if (cloudy == null)
            {
                cloudy = (CloudData)Resources.Load("Cloud Profiles/Cloudy");
            }
            if (highAltitude == null)
            {
                highAltitude = (CloudData)Resources.Load("Cloud Profiles/HighAltitude");
            }
            if (overcast == null)
            {
                overcast = (CloudData)Resources.Load("Cloud Profiles/Overcast");
            }
            if (stormy == null)
            {
                stormy = (CloudData)Resources.Load("Cloud Profiles/Stormy");
            }
            if (sparse == null)
            {
                sparse = (CloudData)Resources.Load("Cloud Profiles/Sparse");
            }
            #endregion

            #region GetSkyMaterials
            if (upscaleMaterial == null)
            {
                upscaleMaterial = (Material)Resources.Load("Rendering/SkyUpscaler");
            }

            if (skyMaterial == null)
            {
                skyMaterial = (Material)Resources.Load("Rendering/SkyRenderer");
            }
            #endregion

            if (upscaleMaterial != null)
            {
                renderPass = new CloudRenderPass(upscaleMaterial);
                renderPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            }
        }

        //Updates cloud material values
        void UpdateClouds()
        {
            if (upscaleMaterial != null)
            {
                upscaleMaterial.SetFloat("_Blur_Radius", compositing.blurRadius);
                upscaleMaterial.SetFloat("_Blur_Strength", compositing.blurStrength);
                upscaleMaterial.SetFloat("_Iterations", compositing.blurIterations);
            }

            RenderSky.renderScale = compositing.renderScale;
            RenderSky.skyLayer = skyLayer;

            custom1 = clouds.cloudDataLow;
            custom2 = clouds.cloudDataHigh;

            #region SetCloudProfiles

            if (clouds.cloudLayerLow == CloudType.Cloudy)
            {
                currentCloudData = cloudy;
            }
            else if (clouds.cloudLayerLow == CloudType.Sparse)
            {
                currentCloudData = sparse;
            }
            else if (clouds.cloudLayerLow == CloudType.Stormy)
            {
                currentCloudData = stormy;
            }
            else if (clouds.cloudLayerLow == CloudType.HighAltitude)
            {
                currentCloudData = highAltitude;
            }
            else if (clouds.cloudLayerLow == CloudType.Overcast)
            {
                currentCloudData = overcast;
            }
            else if (clouds.cloudLayerLow == CloudType.Custom && custom1 != null)
            {
                currentCloudData = custom1;
            }

            if (clouds.cloudLayerHigh == CloudType.Cloudy)
            {
                currentCloudData1 = cloudy;
            }
            else if (clouds.cloudLayerHigh == CloudType.Sparse)
            {
                currentCloudData1 = sparse;
            }
            else if (clouds.cloudLayerHigh == CloudType.Stormy)
            {
                currentCloudData1 = stormy;
            }
            else if (clouds.cloudLayerHigh == CloudType.HighAltitude)
            {
                currentCloudData1 = highAltitude;
            }
            else if (clouds.cloudLayerHigh == CloudType.Overcast)
            {
                currentCloudData1 = overcast;
            }
            else if (clouds.cloudLayerHigh == CloudType.Custom && custom2 != null)
            {
                currentCloudData1 = custom2;
            }

            #endregion

            if (skyMaterial != null && currentCloudData != null)
            {
                if (clouds.cloudLayerLow == CloudType.None)
                {
                    skyMaterial.DisableKeyword("_CLOUDS");
                }
                else
                {
                    skyMaterial.EnableKeyword("_CLOUDS");
                    skyMaterial.SetFloat("_Fog_Density", currentCloudData.fogDensity);
                    skyMaterial.SetFloat("_Fog_Strength", currentCloudData.fogStrength);

                    skyMaterial.SetFloat("_Noise_Scale", currentCloudData.noiseScale);
                    skyMaterial.SetFloat("_Coverage_Scale", currentCloudData.coverageScale);
                    skyMaterial.SetInt("_Invert_Noise", currentCloudData.invertNoise ? 1 : 0);
                    skyMaterial.SetVector("_Noise_Weights", currentCloudData.noiseWeights);

                    skyMaterial.SetColor("_Albedo", currentCloudData.color);
                    skyMaterial.SetFloat("_Day_Brightness", currentCloudData.dayBrightness);
                    skyMaterial.SetFloat("_Evening_Brightness", currentCloudData.eveningBrightness);
                    skyMaterial.SetFloat("_Day_Min_Darkness", currentCloudData.dayMinDarkness);
                    skyMaterial.SetFloat("_Evening_Min_Darkness", currentCloudData.eveningMinDarkness);

                    skyMaterial.SetFloat("_Density", currentCloudData.density);
                    skyMaterial.SetFloat("_Anisotropy", currentCloudData.anisotropy);
                    skyMaterial.SetFloat("_Scattering", currentCloudData.scattering);

                    skyMaterial.SetFloat("_Speed", currentCloudData.speed);
                    skyMaterial.SetFloat("_Height_Falloff", currentCloudData.heightFalloff);
                    skyMaterial.SetFloat("_Thickness", currentCloudData.thickness);
                    skyMaterial.SetVector("_Position", currentCloudData.position);
                    skyMaterial.SetVector("_Bounds", currentCloudData.bounds);
                }

                if (clouds.cloudLayerHigh == CloudType.None)
                {
                    skyMaterial.DisableKeyword("_CLOUDS_LAYER");
                }
                else
                {
                    skyMaterial.EnableKeyword("_CLOUDS_LAYER");
                    skyMaterial.SetFloat("_Fog_Density_1", currentCloudData1.fogDensity);
                    skyMaterial.SetFloat("_Fog_Strength_1", currentCloudData1.fogStrength);

                    skyMaterial.SetFloat("_Noise_Scale_1", currentCloudData1.noiseScale);
                    skyMaterial.SetFloat("_Coverage_Scale_1", currentCloudData1.coverageScale);
                    skyMaterial.SetInt("_Invert_Noise_1", currentCloudData1.invertNoise ? 1 : 0);
                    skyMaterial.SetVector("_Noise_Weights_1", currentCloudData1.noiseWeights);

                    skyMaterial.SetColor("_Albedo_1", currentCloudData1.color);
                    skyMaterial.SetFloat("_Day_Brightness_1", currentCloudData1.dayBrightness);
                    skyMaterial.SetFloat("_Evening_Brightness_1", currentCloudData1.eveningBrightness);
                    skyMaterial.SetFloat("_Day_Min_Darkness_1", currentCloudData1.dayMinDarkness);
                    skyMaterial.SetFloat("_Evening_Min_Darkness_1", currentCloudData1.eveningMinDarkness);

                    skyMaterial.SetFloat("_Density_1", currentCloudData1.density);
                    skyMaterial.SetFloat("_Anisotropy_1", currentCloudData1.anisotropy);
                    skyMaterial.SetFloat("_Scattering_1", currentCloudData1.scattering);

                    skyMaterial.SetFloat("_Speed_1", currentCloudData1.speed);
                    skyMaterial.SetFloat("_Height_Falloff_1", currentCloudData1.heightFalloff);
                    skyMaterial.SetFloat("_Thickness_1", currentCloudData1.thickness);
                    skyMaterial.SetVector("_Position_1", currentCloudData1.position);
                    skyMaterial.SetVector("_Bounds_1", currentCloudData1.bounds);
                }

                #region SetCloudQuality

                int steps = 0;
                int stepSize = 0;
                if (clouds.quality == CloudQuality.Ultra)
                {
                    steps = 100;
                    stepSize = 20;
                }
                else if (clouds.quality == CloudQuality.High)
                {
                    steps = 60;
                    stepSize = 40;
                }
                else if (clouds.quality == CloudQuality.Medium)
                {
                    steps = 30;
                    stepSize = 80;
                }
                else if (clouds.quality == CloudQuality.Low)
                {
                    steps = 20;
                    stepSize = 120;
                }
                else if (clouds.quality == CloudQuality.Custom)
                {
                    steps = clouds.steps;
                    stepSize = clouds.stepSize;
                }
                skyMaterial.SetFloat("_Steps", steps);
                skyMaterial.SetFloat("_Step_Size", stepSize);

                #endregion

                skyMaterial.SetFloat("_SunSize", sky.sunSize);
                skyMaterial.SetFloat("_SunIntensity", sky.sunIntensity);
                skyMaterial.SetFloat("_Star_Brightness", sky.starIntensity);
                skyMaterial.SetVector("_Wavelengths", sky.lightWavelengths);
                skyMaterial.SetColor("_DayColour", sky.dayColor);
                skyMaterial.SetColor("_EveningColour", sky.eveningColor);
                skyMaterial.SetFloat("_DayScatterStrength", sky.DayScattering);
                skyMaterial.SetFloat("_EveningScatterStrength", sky.EveningScattering);
                skyMaterial.SetFloat("_EveningThreshold", sky.eveningThreshold);
                skyMaterial.SetFloat("_DensityPower", sky.densityPower);
                skyMaterial.SetFloat("_Saturation", sky.saturation);
                skyMaterial.SetFloat("_Clouds_Brightness", compositing.cloudBrightness);
                skyMaterial.SetFloat("_Sky_Brightness", compositing.skyBrightness);

                skyMaterial.SetFloat("_Alpha_Step", compositing.alphaStep);
                skyMaterial.SetFloat("_Alpha_Softness", compositing.alphaSoftness);
            }
        }

        //Adds render passes, but you already knew that.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            SkyAndClouds sac = VolumeManager.instance.stack.GetComponent<SkyAndClouds>();
            isEnabled = sac.IsActive();

            if (!isEnabled) { return; }

            compositing = sac.compositing.value;
            clouds = sac.clouds.value;
            sky = sac.sky.value;

            UpdateClouds();
            renderer.EnqueuePass(renderPass);
        }
    }

}