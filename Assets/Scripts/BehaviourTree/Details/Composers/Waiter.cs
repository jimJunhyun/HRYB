using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : INode
{
	float delTime;

	bool ready = false;
	bool readyResets = true;

	NodeStatus defaultStat;

	Coroutine ongoing;


	public Waiter(float time, bool isResetReady = true, NodeStatus defStat = NodeStatus.Fail, bool countFromStart = false)
	{
		delTime = time;
		readyResets = isResetReady;
		defaultStat = defStat;
		if (countFromStart)
		{
			StartReady();

		}
	}

	public void StartReady()
	{
		GameManager.instance.StartCoroutine(Delayer());
	}

	public void ResetReady()
	{
		ready =false;
		GameManager.instance.StartCoroutine(Delayer());
	}


	public NodeStatus Examine()
	{
		if(ready)
		{
			if (readyResets)
			{
				ResetReady();
			}
			Debug.Log("Ready!");
			return NodeStatus.Sucs;
		}
		else
		{
			Debug.Log("Returned Default " + defaultStat);
			return defaultStat;
		}
	}

	IEnumerator Delayer()
	{
		float t = 0;
		while(t < delTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		Debug.Log("Load complete");
		ready = true;
		ongoing = null;
	}
}
