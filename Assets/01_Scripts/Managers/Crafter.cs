using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum CraftMethod
{
	None,
	Base,
	Medicine,
	Trimmer,


	All
}

public struct ItemAmountPair
{
    public Item info;
    public int num;

    public ItemAmountPair(string name, int cnt = 1)
	{
        info = Item.GetItem<Item>(name);
		if(info == null)
		{
			info = new Item(name, "", ItemType.None, 9999, null, -1, true);
		}
        num = cnt;
	}

	public ItemAmountPair(Item item, int cnt = 1)
	{
		info = item;
		num = cnt;
	}

	public static bool operator==(ItemAmountPair lft, ItemAmountPair rht)
	{
		return lft.info == rht.info && lft.num == rht.num;	
	}

	public static bool operator !=(ItemAmountPair lft, ItemAmountPair rht)
	{
		return lft.info != rht.info || lft.num != rht.num;
	}

	public static ItemAmountPair Empty
	{
		get => new ItemAmountPair(null as Item, 0);
	}

	public InventoryItem ToInven()
	{
		return new InventoryItem(info, num);
	}

	public override bool Equals(object obj)
	{
		return obj is ItemAmountPair amt && amt.info == info && amt.num == num;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(info, num);
	}

	public override string ToString()
	{
		return base.ToString();
	}
}

public struct Recipe
{
	public HashSet<ItemAmountPair> recipe;
	public HashSet<CraftMethod> requirement;
	public string category;

	public Recipe(HashSet<ItemAmountPair> rcp, HashSet<CraftMethod> req, string catg)
	{
		recipe = rcp;
		requirement = req;
		category = catg;
	}

	public static bool operator==(Recipe lft, Recipe rht)
	{
		if (lft.requirement == null || rht.requirement == null)
		{
			return lft.recipe.SetEquals(rht.recipe);
		}
		else
		{
			return lft.recipe.SetEquals(rht.recipe) && lft.requirement.SetEquals(rht.requirement);
		}
	}

	public static bool operator !=(Recipe lft, Recipe rht)
	{
		if (lft.requirement == null || rht.requirement == null)
		{
			return !lft.recipe.SetEquals(rht.recipe);
		}
		else
		{
			return (!lft.recipe.SetEquals(rht.recipe)) || (!lft.requirement.SetEquals(rht.requirement));
		}
	}

	public override string ToString()
	{
		string ret;

		StringBuilder sb;
		bool usingGlobal = GameManager.GetGlobalSB(out sb);

		sb.Append("(Ingredients : ");
		foreach (var item in recipe)
		{
			sb.Append($" [{item.info.MyName} : {item.num}] , ");
		}
		sb.Append(")(Requirements : ");
		if(requirement != null)
		{
			foreach (var item in requirement)
			{
				sb.Append($" [{item}] , ");
			}
		}
		sb.Append("), ->");
		sb.Append(category);
		ret = sb.ToString();

		GameManager.ReturnGlobalSB(usingGlobal);
		
		
		return ret;
	}

	public override bool Equals(object obj)
	{
		return obj is Recipe recipe && this == recipe;
	}

	public override int GetHashCode()
	{
		long hash1 = 17;
		foreach (var item in recipe)
		{
			hash1 += 31 * HashCode.Combine(item.info.MyName, item.num);
		}

		long hash2 = 31;
		if(requirement != null)
		{

			foreach (var item in requirement)
			{
				hash2 += 17 * HashCode.Combine(item.ToString());
			}
		}
		return HashCode.Combine(hash1, hash2);
	}
}


public class Crafter
{
	const int TRIMPARSEFROM = 6;
	const int RECIPEPARSEFROM = 2;
	const int ORIGINALNAME = 1;


	CraftMethod curMethod;
	public CraftMethod CurMethod { get => curMethod; set => curMethod = value;}

    public static Hashtable recipeItemTable = new Hashtable()
	{
		
		//{ new Recipe(new HashSet<ItemAmountPair>{ new ItemAmountPair("인삼", 2), new ItemAmountPair("녹각") }, new HashSet<CraftMethod>{  CraftMethod.Medicine}, "" ),new ItemAmountPair("도약탕") },
		
	};

	public static Hashtable recipeItemTableTrim = new Hashtable()
	{
		//{ new ItemAmountPair("녹각") , new HashSet<ItemAmountPair>{ new ItemAmountPair("녹용", 3) } },
		//{ new ItemAmountPair("산삼") , new HashSet<ItemAmountPair>{ new ItemAmountPair("잘린 산삼", 3) } },
	};

	public static YinyangItem GetItemWithProcess(string originalName, string afterName)
	{
		YinyangItem resItem = Item.GetItem<YinyangItem>(originalName);
		if (resItem == null)
		{
			resItem = new YinyangItem(originalName, "", ItemType.None, 9999, null, true, -1, DetailAmount.Empty, false);
		}
		resItem = new YinyangItem(resItem);
		if (afterName.Contains(PreProcess.STIRPREFIX))
		{
			resItem.processes.Add(ProcessType.Stir);
		}
		if (afterName.Contains(PreProcess.FRYPREFIX))
		{
			resItem.processes.Add(ProcessType.Fry);
		}
		if (afterName.Contains(PreProcess.BURNPREFIX))
		{
			resItem.processes.Add(ProcessType.Burn);
		}
		if (afterName.Contains(PreProcess.MASHPREFIX))
		{
			resItem.processes.Add(ProcessType.Mash);
		}
		resItem.InsertToTable();

		return resItem;
	}

	public static IEnumerator InitializeRecipe() //아이템이 이미 완전하다는 가정하에 ㅈ진행할거임.
	{
		SheetParser data;
		if (PreservedDataManager.instance)
		{
			data = new SheetParser(PreservedDataManager.instance.recipes, "B", "R"); //B3:R
		}
		else if (GameManager.instance)
		{
			data = new SheetParser(GameManager.instance.saver.recipes, "B", "R"); //B3:R
		}
		else
		{
			data = new SheetParser("https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/export?format=tsv&gid=1875583106&range=B3:R", "B", "R"); //B3:R
		}
		yield return new WaitUntil(() => data.inited);
		string originalName;
		string afterName;
		for (int i = 0; i < data.cardinality; i++)
		{
			HashSet<ItemAmountPair> recipeReq = new HashSet<ItemAmountPair>();
			ItemAmountPair resultItem = new ItemAmountPair(data.GetAttribute(i, 0), int.Parse(data.GetAttribute(i, 1)));
			for (int j = RECIPEPARSEFROM; j < data.attributeCount - 2; j+= 3)
			{
				originalName = data.GetAttribute(i, j);
				afterName = data.GetAttribute(i, j + 1);
				if(originalName.Length <= 0 || originalName == "")
					break;
				YinyangItem reqItem = GetItemWithProcess(originalName,afterName);

				ItemAmountPair item = new ItemAmountPair(reqItem.MyName, int.Parse(data.GetAttribute(i, j + 2)));
				recipeReq.Add(item);
			}
			Recipe recipe = new Recipe(recipeReq, null, "");

			AddRecipe(resultItem, recipe);
			Debug.Log($"{resultItem.info.MyName} * {resultItem.num} <== {recipe.ToString()}");
		}
	}

	public static IEnumerator InitializeTrim() //일단됨.
	{
		SheetParser data;
		if (PreservedDataManager.instance)
		{
			data =  new SheetParser(PreservedDataManager.instance.trims, "B", "P"); //B3:P
		}
		else if (GameManager.instance)
		{
			data = new SheetParser(GameManager.instance.saver.trims, "B", "P"); //B3:P
		}
		else
		{
			data = new SheetParser("https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/export?format=tsv&gid=1525999156&range=B3:P", "B", "P"); //B3:P
		}
		yield return new WaitUntil(()=>data.inited);
		for (int i = 0; i < data.cardinality; i++)
		{
			HashSet<ItemAmountPair> res = new HashSet<ItemAmountPair>();
			for (int j = TRIMPARSEFROM + 1; j < data.attributeCount - 1; j += 2)
			{
				if(data.GetAttribute(i, j) == "")
				{
					break;
				}
				res.Add(new ItemAmountPair(data.GetAttribute(i, j), int.Parse(data.GetAttribute(i, j + 1))));
			}
			string originalName = data.GetAttribute(i, ORIGINALNAME);
			string afterName = data.GetAttribute(i, TRIMPARSEFROM);
			YinyangItem item = GetItemWithProcess(originalName, afterName);

			recipeItemTableTrim.Add(new ItemAmountPair(item), res);
		}

		//foreach (ItemAmountPair item in recipeItemTableTrim.Keys)
		//{
		//	Debug.Log($"{item.info.MyName}~");
		//	foreach (ItemAmountPair results in (HashSet<ItemAmountPair>)recipeItemTableTrim[item])
		//	{
		//		Debug.Log($"{results.info.MyName} : {results.num}");
		//	}
		//	Debug.Log($"~{item.info.MyName}");
		//}
	}


	public static void AddRecipe(ItemAmountPair resItem, Recipe recipe)
	{
		recipeItemTable.Add(recipe, resItem);
	}

	public bool TrimItem(string itemName)
	{
		ItemAmountPair info = new ItemAmountPair(itemName);
		if (recipeItemTableTrim.ContainsKey(info))
		{
			HashSet <ItemAmountPair> results = (HashSet<ItemAmountPair>)recipeItemTableTrim[info];

			foreach (var item in results)
			{
				if (GameManager.instance.pinven.AddItem(item.info, item.num) > 0)
				{
					Debug.Log("일부 획득실패");
				}
				GameManager.instance.uiManager.UpdateInvenUI();
			}

			return true;
		}
		return false;
	}

	public bool CraftWith(Recipe recipe)
	{
		Debug.Log(curMethod);
		foreach (Recipe item in recipeItemTable.Keys)
		{
			if(recipe == item)
				recipe = item;
		}
		if (recipeItemTable.ContainsKey(recipe))
		{
			ItemAmountPair result = (ItemAmountPair)recipeItemTable[recipe];
			if (true/*recipe.requirement.Contains(curMethod)*/)
			{
				foreach (ItemAmountPair items in recipe.recipe)
				{
					if (!GameManager.instance.pinven.RemoveItem(items.info, items.num))
					{
						Debug.Log("아이템 부족");
						return false;
					}
				}

				if(GameManager.instance.pinven.AddItem(result.info, result.num) > 0)
				{
					Debug.Log("일부 획득실패");
				}
				GameManager.instance.uiManager.UpdateInvenUI();
				return true;
			}
			Debug.Log("제작 요구 사항 부족");
			return false;
		}
		Debug.Log("레시피 없음.");
		return false;
	}

	public bool Craft(ItemAmountPair data)
	{
		Debug.Log(curMethod);
		if (recipeItemTable.ContainsValue(data))
		{
			Recipe recipe = new Recipe();
			Recipe[] keys = new Recipe[recipeItemTable.Count];
			ItemAmountPair[] values = new ItemAmountPair[recipeItemTable.Count];
			recipeItemTable.Keys.CopyTo(keys, 0);
			recipeItemTable.Values.CopyTo(values, 0);

			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == data)
				{
					recipe = keys[i];
					break;
				}
			}

			if (true /*recipe.requirement.Contains(curMethod)*/)
			{
				foreach (ItemAmountPair items in recipe.recipe)
				{
					if (!GameManager.instance.pinven.RemoveItemExamine(items.info, items.num))
					{
						Debug.Log("아이템 부족");
						return false;
					}
				}
				foreach (ItemAmountPair items in recipe.recipe)
				{
					GameManager.instance.pinven.RemoveItem(items.info, items.num);
				}

				if (GameManager.instance.pinven.AddItem(data.info, data.num) > 0)
				{
					Debug.Log("일부 획득실패");
				}
				GameManager.instance.uiManager.UpdateInvenUI();
				return true;
			}
			Debug.Log("제작 요구 사항 부족");
			return false;
		}
		Debug.Log("레시피 없음.");
		return false;
	}
}
