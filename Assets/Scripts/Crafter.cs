using System.Collections;
using System.Collections.Generic;
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
}


public class Crafter
{

	CraftMethod curMethod;
	public CraftMethod CurMethod { get => curMethod; set => curMethod = value;}

    public static Hashtable itemAmtRecipeHash = new Hashtable()
	{
		{ new ItemAmountPair("밧줄"), new Recipe(new HashSet<ItemAmountPair>{ new ItemAmountPair("나뭇가지", 2) }, new HashSet<CraftMethod>{  CraftMethod.Base} )},
	};

	public static void AddRecipe(ItemAmountPair resItem, Recipe recipe)
	{
		itemAmtRecipeHash.Add(resItem, recipe);
	}

    public bool Craft(ItemAmountPair data)
	{
		Debug.Log(curMethod);
		if (itemAmtRecipeHash.ContainsKey(data))
		{
			Recipe recipe = (Recipe)itemAmtRecipeHash[data];
			if(recipe.requirement.Contains(curMethod))
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
