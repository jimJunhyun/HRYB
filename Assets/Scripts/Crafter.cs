using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemAmountPair
{
    public Item info;
    public int num;

    public ItemAmountPair(string name, int cnt)
	{
        info = Item.nameDataHashT[name] as Item;
        num = cnt;
	}
}


public class Crafter : MonoBehaviour
{
    public static Hashtable itemAmtRecipeHash;

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
				Debug.Log("¿œ∫Œ »πµÊ Ω«∆–");
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
