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

	protected override int maxAmt
	{
		get => numPerWater * liquidNum.num;
	}

	

	Coroutine ongoing;
	
	protected override void Awake()
	{
		base.Awake();
	}

	public override void Inter()
	{
		if (!processing)
		{
			ItemAmountPair hold = GameManager.instance.pinven.CurHoldingItem;
			if (hold != ItemAmountPair.Empty)
			{
				if (liquidNum == ItemAmountPair.Empty && hold.info.itemType == ItemType.Liquid && hold.info.Id == waterHash && GameManager.instance.pinven.RemoveHolding(1))
				{
					liquidNum = new ItemAmountPair(hold.info, 1);
					insertOrder.Push(new ItemAmountPair(hold.info, 1));
					Debug.Log("물 삽입");
				}
				else if (hold.info.itemType == ItemType.Liquid && hold.info == liquidNum.info && liqMax > liquidNum.num && GameManager.instance.pinven.RemoveHolding(1))
				{
					liquidNum.num += 1;
					insertOrder.Push(new ItemAmountPair(hold.info, 1));
					Debug.Log($"물 추가, 갯수 {liquidNum.num}");
				}
				else if (hold != liquidNum)
				{
					Debug.Log("물이 아님.");
					base.Inter();
				}
				else
				{
					Debug.Log("물 최대치, 추가 불가.");
				}
			}
			else
			{
				Debug.Log("손 빔.");
				base.Inter();
			}
		}
		

	}

	public override void Pop()
	{
		ItemAmountPair info = insertOrder.Peek();
		if(info.info.Id == waterHash)
		{
			if(GameManager.instance.pinven.AddItem(info.info, info.num) == 0)
			{
				insertOrder.Pop();
				liquidNum.num -= 1;
				Debug.Log("물 뺌.");
				if (liquidNum.num == 0)
				{
					liquidNum = ItemAmountPair.Empty;
					Debug.Log("물 마름.");
				}
			}
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

		Debug.Log(myRecipe.ToString());

		foreach (Recipe item in Crafter.recipeItemTable.Keys)
		{
			Debug.Log($"{myRecipe} == {item} --> {myRecipe == item}");
		}

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
			yield return GameManager.instance.waitSec;
			foreach (var item in holding)
			{
				Debug.Log($"{item.info.myName} : {(item.info as YinyangItem).nameAsChar}");
				YinyangWuXing brewed = (item.info as YinyangItem).yywx * (1 / brewMaxSec) * Random.Range(0.9f, 1.1f) * item.num; //추출 공식은 어떻게 되는가?
				resultItem.yywx += brewed;
				//언제 능력이 더해지는가?
			}
		}
		ongoing = null;
	}
}
