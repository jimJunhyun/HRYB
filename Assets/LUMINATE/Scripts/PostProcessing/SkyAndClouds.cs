using GapperGames;
using System;
using UnityEditor.Rendering;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenuForRenderPipeline("GapperGames/Sky and Clouds", typeof(UniversalRenderPipeline)), RequireComponent(typeof(RenderSky))]
    public sealed partial class SkyAndClouds : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter enabled = new BoolParameter(false);

        //public CompositingSettings compositing;
        //public SkySettings sky;
        //public CloudSettings clouds;
        public CompositingSettingsParameter compositing = new CompositingSettingsParameter(new CompositingSettings());
        public CloudSettingsParameter clouds = new CloudSettingsParameter(new CloudSettings());
        public SkySettingsParameter sky = new SkySettingsParameter(new SkySettings());

        /// <inheritdoc/>
        public bool IsActive() => enabled.value;

        /// <inheritdoc/>
        public bool IsTileCompatible() => false;
    }

    [Serializable]
    public sealed class CloudPresetParameter : VolumeParameter<CloudType>
    {
        /// <summary>
        /// Creates a new <see cref="CloudPresetParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public CloudPresetParameter(CloudType value, bool overrideState = false) : base(value, overrideState) { }
    }

    [Serializable]
    public class CompositingSettings
    {
        [Header("Performance")]
        [Range(0.01f, 1f)] public float renderScale = 0.4f;
        [Header("Blurring")]
        [Range(0f, 0.05f)] public float blurRadius = 0.0025f;
        [Range(0.01f, 20f)] public float blurStrength = 0.01f;
        [Range(1, 10)] public int blurIterations = 1;
        [Header("Alpha")]
        [Range(0, 1)] public float alphaStep = 0;
        [Range(0, 1)] public float alphaSoftness = 0.35f;
        [Header("Brightness")]
        public float cloudBrightness = 0.3f;
        public float skyBrightness = 0.2f;
    }

    [Serializable]
    public class SkySettings
    {
        [Header("Sun")]
        [Range(0, 1)] public float sunSize = 0.1f;
        public float sunIntensity = 1000;
        public float starIntensity = 5;

        [Header("Rayleigh Scattering")]
        public Vector3 lightWavelengths = new Vector3(680, 550, 440);
        public float densityPower = 8;
        public float DayScattering = 1;
        public float EveningScattering = 8;

        [Header("Colour")]
        public Color dayColor = Color.white;
        public Color eveningColor = new Color(1, 0.509f, 0.509f);
        [Range(0, 2)] public float saturation = 1.5f;

        [Header("Day to Evening Gradient")]
        public float eveningThreshold = 3;
    }

    [Serializable]
    public sealed class CloudSettings
    {
        [Header("Quality")]
        public CloudQuality quality = CloudQuality.Medium;
        [GapperGamesConditionalHide(nameof(quality), 4)] public int steps = 30;
        [GapperGamesConditionalHide(nameof(quality), 4)] public int stepSize = 80;

        [Header("Cloud Types")]
        public CloudType cloudLayerLow = CloudType.Sparse;
        public CloudType cloudLayerHigh = CloudType.HighAltitude;

        public bool cloudShadows = false;

        [GapperGamesConditionalHide(nameof(cloudLayerLow), 5)] public CloudData cloudDataLow;
        [GapperGamesConditionalHide(nameof(cloudLayerHigh), 5)] public CloudData cloudDataHigh;
    }

    [Serializable]
    public sealed class CloudSettingsParameter : VolumeParameter<CloudSettings>
    {
        /// <summary>
        /// Creates a new <see cref="CloudSettingsParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public CloudSettingsParameter(CloudSettings value, bool overrideState = false) : base(value, overrideState) { }
    }

    [Serializable]
    public sealed class SkySettingsParameter : VolumeParameter<SkySettings>
    {
        /// <summary>
        /// Creates a new <see cref="SkySettingsParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public SkySettingsParameter(SkySettings value, bool overrideState = false) : base(value, overrideState) { }
    }

    [Serializable]
    public sealed class CompositingSettingsParameter : VolumeParameter<CompositingSettings>
    {
        /// <summary>
        /// Creates a new <see cref="CompositingSettingsParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public CompositingSettingsParameter(CompositingSettings value, bool overrideState = false) : base(value, overrideState) { }
    }
}
