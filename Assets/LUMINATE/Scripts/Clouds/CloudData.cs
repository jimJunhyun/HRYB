using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GapperGames
{
    [CreateAssetMenu(fileName = "new Cloud Data", menuName = "LUMINATE/New Cloud Data")]
    public class CloudData : ScriptableObject
    {
        public CloudData(float noiseScale, float coverageScale, bool invertNoise, Vector3 noiseWeights, Color color, float dayBrightness, float eveningBrightness, float dayMinDarkness, float eveningMinDarkness, float density, float anisotropy, float scattering, float heightFalloff, float thickness, float speed, Vector3 position, Vector3 bounds)
        {
            this.noiseScale = noiseScale;
            this.coverageScale = coverageScale;
            this.invertNoise = invertNoise;
            this.noiseWeights = noiseWeights;
            this.color = color;
            this.dayBrightness = dayBrightness;
            this.eveningBrightness = eveningBrightness;
            this.dayMinDarkness = dayMinDarkness;
            this.eveningMinDarkness = eveningMinDarkness;
            this.density = density;
            this.anisotropy = anisotropy;
            this.scattering = scattering;
            this.heightFalloff = heightFalloff;
            this.thickness = thickness;
            this.speed = speed;
            this.position = position;
            this.bounds = bounds;
        }

        [Header("Noise")]
        public float noiseScale = 10;
        public float coverageScale = 5;
        public bool invertNoise = true;
        public Vector3 noiseWeights = new Vector3(5, 3, 0.5f);

        [Header("Color")]
        public Color color = Color.white;
        public float dayBrightness = 5;
        public float eveningBrightness = 15;
        [Range(0, 1)] public float dayMinDarkness = 0.5f;
        [Range(0, 1)] public float eveningMinDarkness = 0.1f;

        [Header("Lighting")]
        [Range(0, 1)] public float density = 0.5f;
        [Range(0, 1)] public float anisotropy = 0.8f;
        [Range(0, 1)] public float scattering = 0.25f;

        [Header("Shape")]
        [Range(0.01f, 1)] public float heightFalloff = 0.2f;
        [Range(-1, 1)] public float thickness = 0.2f;
        public float speed = 1;
        public Vector3 position = new Vector3(0, 1500, 0);
        public Vector3 bounds = new Vector3(1000000, 500, 1000000);

        [Header("Fog")]
        [Range(0, 1)] public float fogDensity = 0.1f;
        [Range(0, 1)] public float fogStrength = 0.5f;
    }

}
