using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum CraftMethod
{
	None,
	Base,
	Medicine,

}

public struct ItemAmountPair
{
    public Item info;
    public int num;

    public ItemAmountPair(string name, int cnt = 1)
	{
        info = Item.nameDataHashT[name.GetHashCode()] as Item;
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
}

public struct Recipe
{
	public HashSet<ItemAmountPair> recipe;
	public HashSet<CraftMethod> requirement;

	public Recipe(HashSet<ItemAmountPair> rcp, HashSet<CraftMethod> req)
	{
		recipe = rcp;
		requirement = req;
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
		StringBuilder sb = new StringBuilder();
		sb.Append("(Ingredients : ");
		foreach (var item in recipe)
		{
			sb.Append($" [{item.info.myName} : {item.num}] , ");
		}
		sb.Append(")(Requirements : ");
		foreach (var item in requirement)
		{
			sb.Append($" [{item}] , ");
		}
		sb.Append(')');
		return sb.ToString();
	}
}


public class Crafter
{

	CraftMethod curMethod;
	public CraftMethod CurMethod { get => curMethod; set => curMethod = value;}

    public static Hashtable recipeItemTable = new Hashtable()
	{
		{ new Recipe(new HashSet<ItemAmountPair>{ new ItemAmountPair("나뭇가지", 2) }, new HashSet<CraftMethod>{  CraftMethod.Base} ), new ItemAmountPair("밧줄")},
		{ new Recipe(new HashSet<ItemAmountPair>{ new ItemAmountPair("인삼", 2) }, new HashSet<CraftMethod>{  CraftMethod.Medicine} ), new ItemAmountPair(new Medicines("인삼탕", ItemType.Liquid, 1, null, true, new YinyangWuXing(10, 10, 10, 10, 10, 10, 10)))},
	};

	public static void AddRecipe(ItemAmountPair resItem, Recipe recipe)
	{
		recipeItemTable.Add(resItem, recipe);
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
			if (recipe.requirement.Contains(curMethod))
			{
				foreach (ItemAmountPair items in recipe.recipe)
				{
					if (!GameManager.instance.pinven.RemoveItem(items.info, items.num))
					{
						Debug.Log("아이템 부족");
						return false;
					}
				}
				if (GameManager.instance.pinven.AddItem(result.info, result.num) > 0)
				{
					Debug.Log("일부 획득 실패");
					return true;
				}
				else
				{
					return true;
				}
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
				if(values[i] == data)
				{
					recipe = keys[i];
					break;
				}
			}

			if (recipe.requirement.Contains(curMethod))
			{
				foreach (ItemAmountPair items in recipe.recipe)
				{
					if (!GameManager.instance.pinven.RemoveItem(items.info, items.num))
					{
						Debug.Log("아이템 부족");
						return false;
					}
				}
				if (GameManager.instance.pinven.AddItem(data.info, data.num) > 0)
				{
					Debug.Log("일부 획득 실패");
					return true;
				}
				else
				{
					return true;
				}
			}
			Debug.Log("제작 요구 사항 부족");
			return false;
		}
		Debug.Log("레시피 없음.");
		return false;
	}
}
