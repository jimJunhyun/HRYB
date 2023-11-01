using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcessType //손질은 사실상 제작이다.
{
    None = -1,

    
    Roast,
}

public class PreProcess
{
	protected YinyangItem info;
    public ProcessType type;
    public string prefix;

    protected PreProcess(ProcessType t, YinyangItem itemInfo)
	{
		info = itemInfo;
        type = t;
		switch (t)
		{
			case ProcessType.None:
				break;
			case ProcessType.Roast:
				prefix = "작";
				break;
		}
	}
}


public class Roast : PreProcess
{
	public YinyangWuXing Decreased
	{
		get => new YinyangWuXing(info.initDec + (effSec - 1) * info.decPerSec);
	}

	bool roasted = false;

	float effSec = 0;

	Coroutine ongoing;

	public Roast(YinyangItem itemInfo) : base(ProcessType.Roast, itemInfo)
	{

	}

	public void StartProcess()
	{
		ongoing = GameManager.instance.StartCoroutine(DelProcess());
	}

	public void EndProcess()
	{
		GameManager.instance.StopCoroutine(ongoing);
	}

	IEnumerator DelProcess()
	{
		while (true)
		{
			yield return GameManager.instance.waitSec;
			effSec += 1;
			Debug.Log($"감소량 : {Decreased}");
			if(info.onUse != null)
			{
				if (effSec > info.onUse.removeTime && !roasted)
				{
					info.onUse.DeleteSpecial();
					roasted = true;
					prefix = "소";
					Debug.Log("태움.");
				}
			}
			
		}
	}
	public override bool Equals(object obj)
	{
		Debug.Log("!!");
		return obj is Roast;
	}

	public override int GetHashCode()
	{
		return 0;
	}
}