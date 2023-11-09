using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ProcessType //손질은 사실상 제작이다.
{
    None = -1,

    
    Roast,
}

public class PreProcess
{
	protected ItemAmountPair info;
    public ProcessType type;
    public string prefix;

    protected PreProcess(ProcessType t, ItemAmountPair itemInfo)
	{
		if(itemInfo.info is YinyangItem)
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
		else
		{
			Debug.LogError("잘못된 아이템을 조리하려 하고 있다.");
		}
	}

	public virtual ItemAmountPair EndProcess()
	{

		StringBuilder sb = new StringBuilder();
		sb.Append(prefix);
		sb.Append(info.info.MyName);
		info.info.MyName = sb.ToString();
		sb.Clear();
		sb.Append(prefix);
		sb.Append((info.info as YinyangItem).nameAsChar);
		(info.info as YinyangItem).nameAsChar = sb.ToString();
		return info;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(type, info);
	}
}


public class Roast : PreProcess
{
	public YinyangWuXing Decreased
	{
		get => new YinyangWuXing((info.info as YinyangItem).initDec + (effSec - 1) * (info.info as YinyangItem).decPerSec);
	}


	bool roasted = false;

	float effSec = 0;

	Coroutine ongoing;

	public Roast() : base(ProcessType.Roast, ItemAmountPair.Empty)
	{

	}

	public Roast(ItemAmountPair itemInfo) : base(ProcessType.Roast, itemInfo)
	{

	}

	public void StartProcess()
	{
		ongoing = GameManager.instance.StartCoroutine(DelProcess());
	}

	public void PauseProcess(bool paused)
	{
		if (paused)
		{
			GameManager.instance.StopCoroutine(ongoing);
		}
		else
		{
			ongoing = GameManager.instance.StartCoroutine(DelProcess());
		}

	}

	public override ItemAmountPair EndProcess()
	{
		GameManager.instance.StopCoroutine(ongoing);
		ongoing = null;
		(info.info as YinyangItem).yywx -= Decreased;
		if (info.info.onUse != null && effSec > info.info.onUse.removeTime)
		{
			info.info.onUse.DeleteSpecial();
			roasted = true;
			prefix = "소";
			Debug.Log("태움.");
		}
		
		base.EndProcess();

		info.info.InsertToTable();

		return info;
	}

	IEnumerator DelProcess()
	{
		while (true)
		{
			yield return GameManager.instance.waitSec;
			effSec += 1;
			Debug.Log($"감소량 : {Decreased}");
			
		}
	}
	public override bool Equals(object obj)
	{
		Debug.Log("!!");
		return obj is Roast r && r.info == info;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}