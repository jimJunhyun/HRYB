using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Preparation
{
	public float prepTime;

	public Action<Transform> onPrepComp;

	public bool cancelable = true;

	public Preparation(Action<Transform> act, float t, bool canceling = true)
	{
		prepTime = t;
		onPrepComp = act;
		cancelable = canceling;
	}
}

public class CastModule : MonoBehaviour
{
    public Dictionary<string, Preparation> nameCastPair;

	public float castMod = 1f;

	Coroutine ongoing;
	public string curName;

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
		float t = 0;
		while(p.prepTime * castMod > t)
		{
			t += Time.deltaTime;
			yield return null;
		}
		p.onPrepComp?.Invoke(transform);
		ongoing = null;
		curName = null;
	}

}
