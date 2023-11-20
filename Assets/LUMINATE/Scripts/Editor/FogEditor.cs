using GapperGames;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(Fog))]
    sealed class FogEditor : VolumeComponentEditor
    {
        SerializedDataParameter enabled;
        SerializedDataParameter quality;
        SerializedDataParameter downsample;
        SerializedDataParameter steps;
        SerializedDataParameter stepSize;

        private int customDownsample = 1;
        private int customSteps = 125;
        private float customStepSize = 0.2f;
        private bool setCustom;
        private bool wasCustom;

        SerializedDataParameter alphaMultiplier;
        SerializedDataParameter additionalLightsIntensityMultiplier;
        SerializedDataParameter jitter;

        SerializedDataParameter density;
        SerializedDataParameter minDensity;

        SerializedDataParameter fogColor;
        SerializedDataParameter brightness;
        SerializedDataParameter anisotropy;

        SerializedDataParameter blurRadius;
        SerializedDataParameter blurStrength;
        SerializedDataParameter blurIterations;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<Fog>(serializedObject);

            enabled = Unpack(o.Find(x => x.enabled));
            quality = Unpack(o.Find(x => x.quality));
            downsample = Unpack(o.Find(x => x.downsample));
            steps = Unpack(o.Find(x => x.steps));
            stepSize = Unpack(o.Find(x => x.stepSize));

            alphaMultiplier = Unpack(o.Find(x => x.alphaMultiplier));
            additionalLightsIntensityMultiplier = Unpack(o.Find(x => x.additionalLightsIntensityMultiplier));
            jitter = Unpack(o.Find(x => x.jitter));

            density = Unpack(o.Find(x => x.density));
            minDensity = Unpack(o.Find(x => x.minDensity));

            fogColor = Unpack(o.Find(x => x.fogColor));
            brightness = Unpack(o.Find(x => x.brightness));
            anisotropy = Unpack(o.Find(x => x.anisotropy));

            blurRadius = Unpack(o.Find(x => x.blurRadius));
            blurStrength = Unpack(o.Find(x => x.blurStrength));
            blurIterations = Unpack(o.Find(x => x.blurIterations));
        }

        public override void OnInspectorGUI()
        {
            Fog fog = (Fog)target;

            //Create Property Fields
            PropertyField(enabled);
            PropertyField(quality);

            //Custom Quality Presets Stuff
            if(fog.quality == FogPreset.Custom)
            {
                if(setCustom)
                {
                    fog.downsample.value = customDownsample;
                    fog.steps.value = customSteps;
                    fog.stepSize.value = customStepSize;
                    setCustom = false;
                }
                PropertyField(downsample);
                PropertyField(steps);
                PropertyField(stepSize);
                wasCustom = true;
            }
            else if (fog.quality == FogPreset.Ultra)
            {
                if (wasCustom)
                {
                    customDownsample = fog.downsample.value;
                    customSteps = fog.steps.value;
                    customStepSize = fog.stepSize.value;
                }
                fog.downsample.value = 1;
                fog.steps.value = 500;
                fog.stepSize.value = 0.05f;
                setCustom = true;
                wasCustom = false;
            }
            else if (fog.quality == FogPreset.High)
            {
                if(wasCustom)
                {
                    customDownsample = fog.downsample.value;
                    customSteps = fog.steps.value;
                    customStepSize = fog.stepSize.value;
                }
                fog.downsample.value = 1;
                fog.steps.value = 250;
                fog.stepSize.value = 0.1f;
                setCustom = true;
                wasCustom = false;
            }
            else if (fog.quality == FogPreset.Medium)
            {
                if (wasCustom)
                {
                    customDownsample = fog.downsample.value;
                    customSteps = fog.steps.value;
                    customStepSize = fog.stepSize.value;
                }
                fog.downsample.value = 2;
                fog.steps.value = 125;
                fog.stepSize.value = 0.2f;
                setCustom = true;
                wasCustom = false;
            }
            else if (fog.quality == FogPreset.Low)
            {
                if (wasCustom)
                {
                    customDownsample = fog.downsample.value;
                    customSteps = fog.steps.value;
                    customStepSize = fog.stepSize.value;
                }
                fog.downsample.value = 4;
                fog.steps.value = 80;
                fog.stepSize.value = 0.5f;
                setCustom = true;
                wasCustom = false;
            }

            PropertyField(alphaMultiplier);
            PropertyField(additionalLightsIntensityMultiplier);
            PropertyField(jitter);

            PropertyField(density);
            PropertyField(minDensity);

            PropertyField(fogColor);
            PropertyField(brightness);
            PropertyField(anisotropy);

            PropertyField(blurRadius);
            PropertyField(blurStrength);
            PropertyField(blurIterations);
        }
    }
}
