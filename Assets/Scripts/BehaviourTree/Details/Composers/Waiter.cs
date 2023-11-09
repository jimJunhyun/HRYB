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
		Debug.Log("Load complete");
		ready = true;
		ongoing = null;
	}
}
