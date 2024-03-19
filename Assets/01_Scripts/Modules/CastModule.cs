
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Preparation
{
	public Func<float> Timefunc;

	public Action<Transform> onPrepComp;

	public bool cancelable = true;

	public Preparation(Action<Transform> act, Func<float> tFunc, bool canceling = true)
	{
		Timefunc = tFunc;
		onPrepComp = act;
		cancelable = canceling;
	}
}

public class CastModule : Module
{
    public Dictionary<string, Preparation> nameCastPair = new Dictionary<string, Preparation>();


	protected Coroutine ongoing;
	protected string curName;

	public ModuleController castModuleStat = new ModuleController(false);

	public void Cast(string name)
	{
		if (ongoing == null)
		{
			curName = name;
			ongoing = StartCoroutine(DelCast(nameCastPair[name]));
		}
	}

	public virtual void CastCancel()
	{
		if (ongoing != null && nameCastPair[curName].cancelable)
		{
			StopCoroutine(ongoing);
		}
	}

	protected virtual IEnumerator DelCast(Preparation p)
	{
		if (!castModuleStat.Paused)
		{
			float t = 0;
			float waitSec = p.Timefunc();
			while (waitSec * castModuleStat.Speed > t)
			{
				t += Time.deltaTime;
				yield return null;
			}
			p.onPrepComp?.Invoke(transform);
			ongoing = null;
			curName = null;
		}
	}

	public override void ResetStatus()
	{
		CastCancel();
		castModuleStat.CompleteReset();
		base.ResetStatus();
	}

	public void SetNoCastState(ControlModuleMode mode, bool stat)
	{
		castModuleStat.Pause(mode, stat);
		if (castModuleStat.Paused)
		{
			CastCancel();
		}
	}
}
