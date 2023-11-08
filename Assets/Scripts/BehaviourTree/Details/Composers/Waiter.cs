using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : INode
{
	float delTime;

	bool ready = false;
	bool readyResets = true;
	Coroutine ongoing;

	public Waiter(float time, bool isResetReady = true)
	{
		delTime = time;
		readyResets = isResetReady;
	}


	public NodeStatus Examine()
	{
		if(ready)
		{
			if (readyResets)
			{
				ready = false;
			}
			return NodeStatus.Sucs;
		}
		else
		{
			if(ongoing == null)
				ongoing = GameManager.instance.StartCoroutine(Delayer());
			return NodeStatus.Fail;
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
		ready = true;
		ongoing = null;
	}
}
