using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewPoint : CraftPoint
{
	public float brewRate = 10f;
	public override void Process()
	{
		base.Process();

		if(Crafter.itemAmtRecipeHash.ContainsValue(new Recipe(holding, new HashSet<CraftMethod>(){ CraftMethod.Medicine })))
		{

		}

	}

	public override void Stop()
	{
		base.Stop();
	}
}
