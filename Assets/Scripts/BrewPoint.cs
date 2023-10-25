using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

public class BrewPoint : CraftPoint
{
	public float brewMaxSec = 10f;

	public Medicines resultItem;

	WaitForSeconds waitSec = new WaitForSeconds(1.0f);

	public override void Process()
	{
		base.Process();
		Recipe myRecipe = new Recipe(holding, new HashSet<CraftMethod>() { CraftMethod.Medicine });
		if (Crafter.recipeItemTable.ContainsKey(myRecipe))
		{
			result.Add((ItemAmountPair)Crafter.recipeItemTable[myRecipe]);
			
		}
		else
		{
			resultItem = new Medicines(GetMedicineName(), ItemType.Liquid, 1, null, true);
			StartCoroutine(DelAddStat());
		}
	}

	public override void Stop()
	{
		base.Stop();
		if(resultItem == null)
		{
			foreach (var item in result)
			{
				GameManager.instance.pinven.AddItem(item.info, item.num);
				Debug.Log($"{item.info.myName} 획득");
			}
		}
		else
		{
			if (!GameManager.instance.pinven.isFull)
			{
				GameManager.instance.pinven.AddItem(resultItem, 1);
				Debug.Log($"{resultItem.myName} 획득");
			}
		}
	}

	public override string GetMedicineName()
	{
		StringBuilder s = new StringBuilder();
		foreach (var item in holding)
		{
			s.Append((item.info as YinyangItem).nameAsChar);
		}
		s.Append('탕');
		return s.ToString();
	}

	IEnumerator DelAddStat()
	{
		float iter = 0;
		while (iter < brewMaxSec)
		{
			yield return waitSec;
			foreach (var item in holding)
			{
				YinyangWuXing brewed = (item.info as YinyangItem).yywx * (1 / brewMaxSec) * Random.Range(0.9f, 1.1f);
				resultItem.yywx += brewed;
				
			}
		}
		
	}
}
