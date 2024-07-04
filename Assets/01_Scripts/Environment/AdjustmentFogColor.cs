using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustmentFogColor : MonoBehaviour
{
	private Light _light;

	public Color OriginalFogColor = new Color(0.36f, 0.52f, 0.74f);

	private void Awake()
	{
		if(_light == null )
		{
			_light = GetComponent<Light>();
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (_light.intensity < 2)
		{
			RenderSettings.fogColor = OriginalFogColor * (_light.intensity * 0.2f);
		}
		else RenderSettings.fogColor = OriginalFogColor;
	}
}
