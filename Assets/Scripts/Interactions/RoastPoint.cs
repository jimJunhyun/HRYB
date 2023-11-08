using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RoastPoint : CraftPoint 
{
	protected override int maxAmt { get => 1; }

	List<Roast> ongoings = new List<Roast>();

	bool isPaused = false;

	bool isStarted = false;

	public override void AltInter()
	{
		if (processing || isPaused)
		{
			Stop();
		}
		else if (holding.Count > 0)
		{
			Process();
		}
	}

	public override void Process()
	{
		base.Process();
		foreach (var item in holding)
		{
			Roast r = new Roast(item);

			r.StartProcess();
			ongoings.Add(r);
			Debug.Log($"{item.info.MyName} 작업 시작");
		}
		isStarted = true;
	}

	public override void Stop()
	{
		isPaused = !isPaused;
		processing = !isPaused;
		for (int i = 0; i < ongoings.Count; i++)
		{
			ongoings[i].PauseProcess(isPaused);
		}
		Debug.Log($"일시정지 : {isPaused}");
		
	}

	public override void Put()
	{
		ItemAmountPair hold = GameManager.instance.pinven.CurHoldingItem;
		if (GameManager.instance.pinven.HoldingYinYangItem && !(hold.info as YinyangItem).processes.Contains(ProcessType.Roast) && hold.info.itemType == ItemType.Solid)
		{
			base.Put();
		}
		else
			Debug.Log("이미 구움./고체 아님");
	}

	public override void Pop()
	{
		if (isStarted)
		{
			Debug.Log(ongoings.Count + "개 뺌");
			for (int i = 0; i < ongoings.Count; ++i)
			{
				Roast r = ongoings[i];
				ItemAmountPair result = r.EndProcess();
				(result.info as YinyangItem).processes.Add(r.type);
				GameManager.instance.pinven.AddItem(result.info, result.num);
			}
			Initialize();
		}
		else
		{
			base.Pop();
		}
		
		
	}

	public override void Initialize()
	{
		isStarted = false;
		isPaused = false;
		ongoings.Clear();
		base.Initialize();
	}
}
