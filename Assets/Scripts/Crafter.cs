using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemAmountPair
{
    public Item info;
    public int num;

    public ItemAmountPair(string name, int cnt = 1)
	{
        info = Item.nameDataHashT[name] as Item;
		Debug.Log(info); // @@@@@@@@@@@@@@@@@@
        num = cnt;
	}
}


public class Crafter
{
    public static Hashtable itemAmtRecipeHash = new Hashtable()
	{
		{ new ItemAmountPair("¹åÁÙ"), new List<ItemAmountPair>{ new ItemAmountPair("³ª¹µ°¡Áö", 2) } },
	};

	public static void AddRecipe(ItemAmountPair resItem, List<ItemAmountPair> recipe)
	{
		if(resItem.num > 0 && recipe.Count > 0)
		{
			itemAmtRecipeHash.Add(resItem, recipe);
		}
	}

    public bool Craft(ItemAmountPair data)
	{
		if (itemAmtRecipeHash.ContainsKey(data))
		{
			List<ItemAmountPair> recipe = itemAmtRecipeHash[data] as List<ItemAmountPair>;

			for (int i = 0; i < recipe.Count; i++)
			{
				if(!GameManager.instance.pinven.RemoveItem(recipe[i].info, recipe[i].num))
				{
					return false;
				}
			}
			ItemAmountPair res = ((ItemAmountPair)Item.nameDataHashT[data]);
			if (GameManager.instance.pinven.AddItem(res.info, res.num) > 0)
			{
				Debug.Log("ÀÏºÎ È¹µæ ½ÇÆÐ");
				return true;
			}
			else
			{
				return true;
			}
		}

		return false;
	}
}
