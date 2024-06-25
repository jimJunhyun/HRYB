using GapperGames;
using System;
using UnityEditor.Rendering;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenuForRenderPipeline("GapperGames/Fog", typeof(UniversalRenderPipeline))]
    public sealed partial class Fog : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter enabled = new BoolParameter(false);

        [Header("Quality")]       
        public FogPresetParameter quality = new FogPresetParameter(FogPreset.Medium);

        [Header("Rendering")]
        public ClampedFloatParameter alphaMultiplier = new ClampedFloatParameter(0.5f, 0f, 1f);
        public MinFloatParameter additionalLightsIntensityMultiplier = new MinFloatParameter(1f, 0f);
        public ClampedFloatParameter jitter = new ClampedFloatParameter(0.15f, 0f, 1f);

        public ClampedIntParameter downsample = new ClampedIntParameter(1, 1, 32);
        public MinIntParameter steps = new MinIntParameter(125, 0);
        public MinFloatParameter stepSize = new MinFloatParameter(0.2f, 0f);

        [Header("Density")]
        public ClampedFloatParameter density = new ClampedFloatParameter(0.045f, 0f, 1f);
        public ClampedFloatParameter minDensity = new ClampedFloatParameter(0f, 0f, 1f);

        [Header("Color")]
        public ColorParameter fogColor = new ColorParameter(Color.white, false, false, true);
        public MinFloatParameter brightness = new MinFloatParameter(3f, 0f);
        public ClampedFloatParameter anisotropy = new ClampedFloatParameter(0.8f, 0f, 1f);

        [Header("Blur")]

        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.01f, 0f, 0.05f);
        public ClampedFloatParameter blurStrength = new ClampedFloatParameter(0.5f, 0.01f, 20f);
        public ClampedIntParameter blurIterations = new ClampedIntParameter(2, 0, 10);

        /// <inheritdoc/>
        public bool IsActive() => enabled.value;

        /// <inheritdoc/>
        public bool IsTileCompatible() => false;
    }

    [Serializable]
    public sealed class FogPresetParameter : VolumeParameter<FogPreset>
    {
        /// <summary>
        /// Creates a new <see cref="FogPresetParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public FogPresetParameter(FogPreset value, bool overrideState = false) : base(value, overrideState) { }
    }
}
