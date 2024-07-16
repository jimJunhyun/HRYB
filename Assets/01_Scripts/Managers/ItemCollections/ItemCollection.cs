using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection
{
    public Item myItem;
	public int discoverCount;
	public List<HashSet<ProcessType>> useCount;
	public const int MAXUSECOUNT = 5;

	public bool discovered => discoverCount > 0;
	public bool isMastered => useCount.Count >= MAXUSECOUNT;

	bool allowRepeat;

	List<Item> resultItems;
	public List<Item> ResultItems
	{
		get
		{
			if(resultItems.Count <= 0)
			{
				foreach (ItemAmountPair item in Crafter.recipeItemTableTrim.Keys)
				{
					if(item.info.originalName == myItem.originalName)
					{
						foreach (ItemAmountPair resItem in (HashSet<ItemAmountPair>)Crafter.recipeItemTableTrim[item])
						{
							resultItems.Add(resItem.info);
						}
					}
				}
			}

			return resultItems;
		}
	}

	public void Discover()
	{
		discoverCount += 1;
	}

	public void UsedWithData(HashSet<ProcessType> data)
	{
		
		if (allowRepeat || !useCount.Contains(data))
		{
			useCount.Add(data);
			GameManager.instance.pinven.AddExp(1000);
		}
	}

	public ItemCollection(bool medicineMode)
	{
		myItem = null;
		discoverCount = 0;
		allowRepeat = medicineMode;
		useCount = new List<HashSet<ProcessType>>(MAXUSECOUNT);
		resultItems = new List<Item>();
	}

	public ItemCollection(Item info, bool medicineMode)
	{
		myItem = info;
		discoverCount = 0;
		allowRepeat = medicineMode;
		useCount = new List<HashSet<ProcessType>>(MAXUSECOUNT);
		resultItems = new List<Item>();
	}

	public override bool Equals(object obj)
	{
		if(obj is ItemCollection col && col.myItem == myItem)
			return true;
		else if(obj is Item item && item == myItem)
			return true;
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(myItem);
	}


}
