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

	public virtual float castMod{ get;set;} = 1;

	Coroutine ongoing;
	protected string curName;

	public void Cast(string name)
	{
		if (ongoing == null)
		{
			curName = name;
			ongoing = StartCoroutine(DelCast(nameCastPair[name]));
		}
	}

	public void CastCancel()
	{
		if (ongoing != null && nameCastPair[curName].cancelable)
		{
			StopCoroutine(ongoing);
		}
	}

	protected virtual IEnumerator DelCast(Preparation p)
	{
		float t = 0;
		float waitSec = p.Timefunc();
		while(waitSec * castMod > t)
		{
			t += Time.deltaTime;
			yield return null;
		}
		p.onPrepComp?.Invoke(transform);
		ongoing = null;
		curName = null;
	}

}
