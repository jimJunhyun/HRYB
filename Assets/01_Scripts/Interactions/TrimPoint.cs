using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrimPoint : CraftPoint
{
	protected override int maxAmt
	{
		get => 1;
	}

	public override void Process()
	{
		base.Process();
		bool suc = true;
		foreach (var item in holding)
		{
			suc &= (GameManager.instance.craftManager.TrimWithName(item.info.MyName));
		}
		if (suc)
		{
			Initialize();
		}

		base.Stop();
	}
}
