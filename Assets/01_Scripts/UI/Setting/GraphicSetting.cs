using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum EGraphicOption
{
	Low,
	Medium,
	High
}

public enum EAntiAliasing
{
	x0,
	x2,
	x4,
	x8
}

public struct GraphicSet
{
	public EGraphicOption Option;
	public Vector2 Resolution;
	public FullScreenMode ScreenMode;
	public bool VSync;
	public int MaxFPS;
	public int AntiAliasing;
	public int Shadow;
	public bool Effect;
}

public class GraphicSetting : MonoBehaviour
{

	private GraphicSet Set;
	private readonly string fileName = "GraphicSetting";

	private void Awake()
	{
		JsonManager<GraphicSet>.LoadJson(fileName, out Set);

		Option = Set.Option;
		Resolution = Set.Resolution;
		ScreenMode = Set.ScreenMode;
		VSync = Set.VSync;
		MaxFPS = Set.MaxFPS;
		AntiAliasing = Set.AntiAliasing;
		Shadow = Set.Shadow;
		Effect = Set.Effect;
	}

	public void Save()
	{
		JsonManager<GraphicSet>.SaveJson(Set, fileName);
	}

	public EGraphicOption Option
	{
		get { return Set.Option; }
		set
		{
			Set.Option = value;
			QualitySettings.SetQualityLevel((int)Set.Option);
		}
	}

	public Vector2 Resolution
	{
		get { return Set.Resolution; }
		set
		{
			Set.Resolution = value;
			Screen.SetResolution((int)Set.Resolution.x, (int)Set.Resolution.y, Set.ScreenMode);
		}
	}

	public FullScreenMode ScreenMode
	{
		get { return Set.ScreenMode; }
		set
		{
			Set.ScreenMode = value;
			Screen.SetResolution((int)Set.Resolution.x, (int)Set.Resolution.y, Set.ScreenMode);
		}
	}

	public bool VSync
	{
		get { return Set.VSync; }
		set
		{
			Set.VSync = value;
			QualitySettings.vSyncCount = Set.VSync ? 1 : 0;
		}
	}

	/// <summary>
	/// 최대 프레임레이트를 정함. (단 VSync가 True일 경우 무시함.)
	/// </summary>
	public int MaxFPS
	{
		get { return Set.MaxFPS; }
		set
		{
			Set.MaxFPS = value;

			Application.targetFrameRate = Set.MaxFPS;
		}
	}

	public int AntiAliasing
	{
		get { return Set.AntiAliasing; }
		set
		{
			Set.AntiAliasing = value;
			QualitySettings.antiAliasing = Set.AntiAliasing;
		}
	}

	public int Shadow
	{
		get { return Set.Shadow; }
		set
		{
			Set.Shadow = value;
			if (Set.Shadow == -1)
			{
				QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
				QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
			}
			else
			{
				QualitySettings.shadows = UnityEngine.ShadowQuality.All;
				QualitySettings.shadowResolution = (UnityEngine.ShadowResolution)Set.Shadow;
			}

		}
	}

	public bool Effect
	{
		get { return Set.Effect; }
		set
		{
			Set.Effect = value;
			//Do Something
		}
	}
}
