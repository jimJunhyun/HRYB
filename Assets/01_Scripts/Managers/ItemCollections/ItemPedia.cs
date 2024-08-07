using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedia
{
    public SortedDictionary<YinyangItem, ItemCollection> materialCollections = new SortedDictionary<YinyangItem, ItemCollection>();
    public SortedDictionary<Medicines,  ItemCollection> medicineCollections = new SortedDictionary<Medicines, ItemCollection>();

	public ItemPedia()
	{
		Init();
	}

	public void Init()
	{
		foreach (Item item in Item.nameDataHashT.Values)
		{
			if (item is YinyangItem yy && yy.processes.Count == 0)
			{
				if (item is Medicines m)
				{
					medicineCollections.Add(m, new ItemCollection(m, true));
				}
				else
				{
					materialCollections.Add(yy, new ItemCollection(yy, false));
				}
			}
		}

		//foreach (var item in materialCollections)
		//{
		//	Debug.Log($"{item.Key.originalName} : {item.Value.myItem.MyName}");
		//}
	}

	public void GotItem(YinyangItem data)
	{
		if (materialCollections.ContainsKey(data))
		{
			materialCollections[data].Discover();
		}
		else if(data is Medicines m)
		{
			if (medicineCollections.ContainsKey(m))
			{
				medicineCollections[m].Discover();
			}
		}
		else
		{
			materialCollections.Add(data, new ItemCollection(data, false));
		}
	}

	public void UseItem(YinyangItem data)
	{
		if (materialCollections.ContainsKey(data))
		{
			materialCollections[data].UsedWithData(data.processes);
		}
		else if (data is Medicines m)
		{
			if (medicineCollections.ContainsKey(m))
			{
				medicineCollections[m].UsedWithData(data.processes);
			}
		}
	}
}
