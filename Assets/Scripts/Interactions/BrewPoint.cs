using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

public class BrewPoint : CraftPoint
{
	public float brewDefSec = 12;
	public float brewMaxSec;

	public Medicines resultItem;


	ItemAmountPair liquidNum = new ItemAmountPair();
	readonly int waterHash = "물".GetHashCode();

	int liqMax = 5;
	int numPerWater = 1;

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
		Recipe myRecipe = new Recipe(holding, new HashSet<CraftMethod>() { CraftMethod.Medicine }, null);

		Debug.Log(myRecipe.ToString());

		foreach (Recipe item in Crafter.recipeItemTable.Keys)
		{
			Debug.Log($"{myRecipe.GetHashCode()} == {item.GetHashCode()} --> {myRecipe.Equals(item)}");
		}

		if (Crafter.recipeItemTable.ContainsKey(myRecipe))
		{
			Debug.Log("FounD!");
			result.Add((ItemAmountPair)Crafter.recipeItemTable[myRecipe]);
		}
		else
		{
			Debug.Log("NO");
			resultItem = new Medicines(GetMedicineName(), "약이다.", ItemType.Liquid, 1, Item.removeOnComp, true, YinyangWuXing.Zero);
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
				Debug.Log($"{item.info.MyName} 획득, {(item.info as YinyangItem).yywx.ToString()}");
			}
		}
		else
		{
			if (!GameManager.instance.pinven.isFull)
			{
				GameManager.instance.pinven.AddItem(resultItem, 1);
				Debug.Log($"{resultItem.MyName} 획득, {(resultItem).yywx.ToString()}");
			}
		}
		Initialize();
		
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

	public override void Initialize()
	{
		liquidNum = ItemAmountPair.Empty;
		resultItem = null;
		ongoing = null;
		brewMaxSec = 0;
		base.Initialize();
	}

	IEnumerator DelAddStat()
	{
		float iter = 0;
		float varModifier = 1;
		switch (holding.Count)
		{
			case 1:
				varModifier = 1;
				break;
			case 2:
				varModifier = 0.83f;
				break;
			case 3:
				varModifier = 0.66f;
				break;
			case 4:
				varModifier = 0.49f;
				break;
			case 5:
				varModifier = 0.38f;
				break;
			default:
				break;
		}
		brewMaxSec = brewDefSec * count;
		while (iter < brewMaxSec)
		{
			yield return GameManager.instance.waitSec;
			foreach (var item in holding)
			{
				Debug.Log($"{item.info.MyName} : {(item.info as YinyangItem).nameAsChar}");
				float multDecMod = 0;
				for (int i = 1; i <= item.num; i++)
				{
					multDecMod += 1 / i;
				}
				YinyangWuXing brewed = (item.info as YinyangItem).yywx * (1 / brewMaxSec) * multDecMod * varModifier; //추출 공식은 어떻게 되는가?
				resultItem.yywx += brewed;
				//언제 능력이 더해지는가?
			}
		}
		ongoing = null;
	}
}
