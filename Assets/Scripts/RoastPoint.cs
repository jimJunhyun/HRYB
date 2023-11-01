using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RoastPoint : CraftPoint //###################
{
	protected override int maxAmt { get => 1; }

	Stack<KeyValuePair<Roast, YinyangItem>> ongoings = new Stack<KeyValuePair<Roast, YinyangItem>>();

	public override void Process()
	{
		base.Process();
		foreach (var item in holding)
		{
			YinyangItem roastTo = (item.info) as YinyangItem;
			Roast r = new Roast(roastTo);
			
			if (!roastTo.processes.Contains(r))
			{
				r.StartProcess();
				ongoings.Push(new KeyValuePair<Roast, YinyangItem>(r, roastTo));
			}
			
		}
	}

	public override void Stop()
	{
		base.Stop();
		while(ongoings.Count > 0)
		{
			KeyValuePair<Roast, YinyangItem> r = ongoings.Pop();
			r.Key.EndProcess();
			r.Value.processes.Add(r.Key);
		}
	}
}
