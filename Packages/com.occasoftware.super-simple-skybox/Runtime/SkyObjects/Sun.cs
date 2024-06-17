using System;
using UnityEngine;

namespace OccaSoftware.SuperSimpleSkybox.Runtime
{
	[AddComponentMenu("OccaSoftware/Super Simple Skybox/Sun")]
    public class Sun : DirectionalLight
    {
		[SerializeField]
		private Color _dayColor;

		[SerializeField]
		private Color _eveningColor;
		protected override void Update()
        {
            base.Update();
            Shader.SetGlobalVector(ShaderParams._SunDirection, -transform.forward);

			// adjust light color
			float dotProduct = Vector3.Dot(-transform.forward, Vector3.up);
			//float clampedDot = Mathf.Clamp((dotProduct + 0.9f), 0, 1);
			float topDot = (1 - Mathf.Clamp01(dotProduct)) * Mathf.Clamp01(Mathf.Sign(dotProduct));
			float bottomDot = (1 - Mathf.Clamp01(-dotProduct)) * Mathf.Clamp01(Mathf.Sign(-dotProduct));
			topDot = Mathf.Pow(Mathf.SmoothStep(0f, 0.9f, topDot), 5);
			bottomDot = Mathf.Pow(bottomDot, 5);

			//_light.intensity = Mathf.Lerp(0.1f, MaximumLightIntensity, Mathf.Pow(clampedDot, 5));
			_light.color = Color.Lerp(_dayColor, _eveningColor, topDot + bottomDot);

		}

		protected override void Rotate()
		{
			base.Rotate();
			if (IsFixedOnDay)
			{
				transform.eulerAngles = new Vector3(20.0f, 20.0f, 0.0f);
			}
		}
	}
}
