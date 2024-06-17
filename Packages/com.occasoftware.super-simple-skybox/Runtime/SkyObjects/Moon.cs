using UnityEngine;

namespace OccaSoftware.SuperSimpleSkybox.Runtime
{
    [AddComponentMenu("OccaSoftware/Super Simple Skybox/Moon")]
    public class Moon : DirectionalLight
    {
        protected override void Update()
        {
            base.Update();
            Shader.SetGlobalVector(ShaderParams._MoonDirection, -transform.forward);
        }

		protected override void Rotate()
		{
			base.Rotate();
			if(IsFixedOnDay)
			{
				transform.eulerAngles = new Vector3(210.0f, 20.0f, 0.0f);
			}
		}
	}
}
