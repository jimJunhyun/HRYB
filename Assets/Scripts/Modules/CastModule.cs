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

public class CastModule : MonoBehaviour
{
    public Dictionary<string, Preparation> nameCastPair = new Dictionary<string, Preparation>();

	public float castMod = 1f;

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

	IEnumerator DelCast(Preparation p)
	{
		GameManager.instance.pinp.DeactivateInput();
		float t = 0;
		float waitSec = p.Timefunc();
		while(waitSec * castMod > t)
		{
			t += Time.deltaTime;
			yield return null;
		}
		p.onPrepComp?.Invoke(transform);
		GameManager.instance.pinp.ActivateInput();
		ongoing = null;
		curName = null;
	}

}
