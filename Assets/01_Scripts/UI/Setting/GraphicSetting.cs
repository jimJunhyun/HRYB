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

public class GraphicSetting : MonoBehaviour, ISettings
{

	private GraphicSet set;
	private GraphicSet previousSet;

	private readonly string fileName = "GraphicSetting";

	private bool notSaved;

	private void Awake()
	{
		Load();
	}

	private void OnEnable()
	{
		previousSet = set;
	}

	public void Save()
	{
		JsonManager<GraphicSet>.SaveJson(set, fileName);
		previousSet = set;
		notSaved = false;
	}

	public void Load()
	{
		if (JsonManager<GraphicSet>.LoadJson(fileName, out set))
		{
			Option = set.Option;
			Resolution = set.Resolution;
			ScreenMode = set.ScreenMode;
			VSync = set.VSync;
			MaxFPS = set.MaxFPS;
			AntiAliasing = set.AntiAliasing;
			Shadow = set.Shadow;
			Effect = set.Effect;
		}
	}

	public void Revert()
	{
		Option = previousSet.Option;
		Resolution = previousSet.Resolution;
		ScreenMode = previousSet.ScreenMode;
		VSync = previousSet.VSync;
		MaxFPS = previousSet.MaxFPS;
		AntiAliasing = previousSet.AntiAliasing;
		Shadow = previousSet.Shadow;
		Effect = previousSet.Effect;
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void Open()
	{
		gameObject.SetActive(true);
	}

	public EGraphicOption Option
	{
		get { return set.Option; }
		set
		{
			set.Option = value;
			QualitySettings.SetQualityLevel((int)set.Option);
			notSaved = true;
		}
	}

	public Vector2 Resolution
	{
		get { return set.Resolution; }
		set
		{
			set.Resolution = value;
			Screen.SetResolution((int)set.Resolution.x, (int)set.Resolution.y, set.ScreenMode);
			notSaved = true;
		}
	}

	public FullScreenMode ScreenMode
	{
		get { return set.ScreenMode; }
		set
		{
			set.ScreenMode = value;
			Screen.SetResolution((int)set.Resolution.x, (int)set.Resolution.y, set.ScreenMode);
			notSaved = true;
		}
	}

	public bool VSync
	{
		get { return set.VSync; }
		set
		{
			set.VSync = value;
			QualitySettings.vSyncCount = set.VSync ? 1 : 0;
			notSaved = true;
		}
	}

	/// <summary>
	/// 최대 프레임레이트를 정함. (단 VSync가 True일 경우 무시함.)
	/// </summary>
	public int MaxFPS
	{
		get { return set.MaxFPS; }
		set
		{
			set.MaxFPS = value;

			Application.targetFrameRate = set.MaxFPS;
			notSaved = true;
		}
	}

	public int AntiAliasing
	{
		get { return set.AntiAliasing; }
		set
		{
			set.AntiAliasing = value;
			QualitySettings.antiAliasing = set.AntiAliasing;
			notSaved = true;
		}
	}

	public int Shadow
	{
		get { return set.Shadow; }
		set
		{
			set.Shadow = value;
			notSaved = true;
			if (set.Shadow == -1)
			{
				QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
				QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
			}
			else
			{
				QualitySettings.shadows = UnityEngine.ShadowQuality.All;
				QualitySettings.shadowResolution = (UnityEngine.ShadowResolution)set.Shadow;
			}

		}
	}

	public bool Effect
	{
		get { return set.Effect; }
		set
		{
			set.Effect = value;
			notSaved = true;
			//Do Something
		}
	}

	public bool NotSaved => notSaved;
}
