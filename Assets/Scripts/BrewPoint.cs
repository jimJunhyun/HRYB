using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

public class BrewPoint : CraftPoint
{
	public float brewMaxSec = 10f;

	public Medicines resultItem;


	ItemAmountPair liquidNum = new ItemAmountPair();
	int waterHash = "물".GetHashCode();

	int liqMax = 5;
	int numPerWater = 2;

	new int maxAmt
	{
		get => numPerWater * liquidNum.num;
	}

	WaitForSeconds waitSec = new WaitForSeconds(1.0f);

	Coroutine ongoing;
	
	protected override void Awake()
	{
		base.Awake();
	}

	public override void Inter()
	{
		ItemAmountPair hold =  GameManager.instance.pinven.CurHoldingItem;
		if(liquidNum == ItemAmountPair.Empty && hold.info.itemType == ItemType.Liquid && hold.info.Id == waterHash)
		{

		}
		else if(hold.info.itemType == ItemType.Liquid && hold.info.Id == waterHash && liqMax > liquidNum.num)
		{
			liquidNum.info = hold.info;
			liquidNum.num += 1;
			insertOrder.Push(new ItemAmountPair(hold.info, hold.num));
		}
		else
		{
			base.Inter();
		}
	}

	public override void Pop()
	{
		ItemAmountPair info = insertOrder.Peek();
		if(info.info.Id == waterHash)
		{
			insertOrder.Pop();
			liquidNum.num -= 1;
			if(liquidNum.num == 0)
				liquidNum = ItemAmountPair.Empty;
		}
		else
		{
			base.Pop();
		}
	}

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
			resultItem = new Medicines(GetMedicineName(), ItemType.Liquid, 1, null, true, YinyangWuXing.Zero);
			ongoing = StartCoroutine(DelAddStat());
		}
	}

	public override void Stop()
	{
		if(ongoing != null)
		{
			StopCoroutine(ongoing);
		}
		base.Stop();
		if(resultItem == null)
		{
			foreach (var item in result)
			{
				GameManager.instance.pinven.AddItem(item.info, item.num);
				Debug.Log($"{item.info.myName} 획득, {(item.info as YinyangItem).yywx.ToString()}");
			}
		}
		else
		{
			if (!GameManager.instance.pinven.isFull)
			{
				GameManager.instance.pinven.AddItem(resultItem, 1);
				Debug.Log($"{resultItem.myName} 획득, {(resultItem).yywx.ToString()}");
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
				Debug.Log($"{item.info.myName} : {(item.info as YinyangItem).nameAsChar}");
				YinyangWuXing brewed = (item.info as YinyangItem).yywx * (1 / brewMaxSec) * Random.Range(0.9f, 1.1f) * item.num;
				resultItem.yywx += brewed;
			}
		}
		ongoing = null;
	}
}
