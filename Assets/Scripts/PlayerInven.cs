using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryItem
{
	public Item info;
	public int number;
	public InventoryItem(Item item, int num)
	{
		info = item;
		number = num;
	}

	public bool isEmpty()
	{
		return number == 0;
	}
}

public class Inventory
{
	public Inventory(int cap)
	{
		data = new List<InventoryItem>(cap);
		for (int i = 0; i < cap; i++)
		{
			data.Add(new InventoryItem(null, 0));
		}
	}

	List<InventoryItem> data;


	public InventoryItem this[int idx]
	{ 
		get 
		{ 
			return data[idx];
		} 
		set	
		{
			data[idx] = value; 
		}
	}

	public int Count { get; private set;}

	public int Contains(Item info)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if(!this[i].isEmpty() && this[i].info == info)
			{
				return i;
			}
		}
		return -1;
	}

	public void AddCapacity(int amt)
	{
		data.Capacity += amt;
	}

	public void Add(InventoryItem item)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].isEmpty())
			{
				data[i] = item;
				++Count;
				return;
			}
		}
	}

	public bool Add(InventoryItem item, int to)
	{
		if (data[to].isEmpty())
		{
			data[to] = item;
			++Count;
			return true;
		}
		return false;
	}

	public void Remove(int idx)
	{
		data[idx] = new InventoryItem(null, 0);
	}
}

public class PlayerInven : MonoBehaviour
{
    public Inventory inven;
    public int cap = 20;

	private void Awake()
	{
		inven = new Inventory(cap);
	}

	public bool AddItem(Item data, int num = 1)
	{
		int idx;
		if ((idx = inven.Contains(data)) != -1)
		{
			InventoryItem slotItem = inven[idx];
			if(slotItem.number + num <= slotItem.info.maxStack)
			{
				slotItem.number += num;
				inven[idx] = slotItem;
				Debug.Log($"{slotItem.info.myName}, {slotItem.number}개, 위치 : {idx}");
				return true;
			}
		}
		else
		{
			InventoryItem slotItem = new InventoryItem(data, num);
			if(inven.Count < cap)
			{
				inven.Add(slotItem);
				Debug.Log($"{slotItem.info.myName}, {slotItem.number}개, 새로 추가됨");
				return true;
			}
		}
		Debug.Log("추가 실패");
		return false;
	}

	public bool AddItem(Item data, int to, int num)
	{
		if(inven[to].isEmpty())
		{
			inven.Add(new InventoryItem(data, num), to);
			return true;
		}
		else if(inven[to].info == data)
		{
			InventoryItem slotItem = inven[to];
			if(slotItem.number + num <= slotItem.info.maxStack)
			{
				slotItem.number += num;
				inven[to] = slotItem;
				//????????
				return true;
			}

		}
		return false;
	}

	public bool RemoveItem(Item data, int num = 1)
	{
		int idx;
		if ((idx = inven.Contains(data)) != -1)
		{
			InventoryItem slotItem = inven[idx];
			if (slotItem.number - num >= 0)
			{
				slotItem.number -= num;
				if(slotItem.number == 0)
				{
					RemoveItem(idx);
					Debug.Log($"{slotItem.info.myName}, {slotItem.number}개, 위치 : {idx}, 제거됨.");
				}
				else
				{
					inven[idx] = slotItem;
					Debug.Log($"{slotItem.info.myName}, {slotItem.number}개, 위치 : {idx}");
				}

				return true;
			}
		}
		Debug.Log("제거 실패. 사유 : 아이템 없음");
		return false;
	}

	public bool RemoveItem(int from, int num = 1)
	{
		InventoryItem slotItem = inven[from];
		if (slotItem.number - num >= 0)
		{
			slotItem.number -= num;

			if(slotItem.number == 0)
			{
				inven.Remove(from);
			}
			else
			{
				inven[from] = slotItem;
			}
			
			return true;
		}
		return false;
	}

	public bool Move(int from, int to, int num)
	{
		if(!inven[from].isEmpty() && !inven[to].isEmpty() && inven[from].info != inven[to].info && num == inven[from].number)
		{
			Debug.Log($"{inven[from].info.myName}, {inven[from].number}개, {inven[to].info.myName}, {inven[to].number}개에서, ");
			Swap(from, to);
			Debug.Log($"{inven[from].info.myName}, {inven[from].number}개, {inven[to].info.myName}, {inven[to].number}개로 변경.");
			return true;
		}
		else
		{
			if ((!inven[from].isEmpty() && inven[to].isEmpty()) || (inven[from].info == inven[to].info))
			{
				Debug.Log($"{inven[from].info.myName}, {inven[from].number}개, {(inven[to].isEmpty() ? 0 : inven[to].info.myName)}, {(inven[to].isEmpty() ? 0 : inven[to].number)}개에서, ");
				if (AddItem(inven[from].info, to, num))
				{
					RemoveItem(from, num);
					Debug.Log($"{(inven[from].isEmpty() ? 0 : inven[from].info.myName)}, {(inven[from].isEmpty() ? 0 : inven[from].number)}개, {inven[to].info.myName}, {inven[to].number}개로 변경.");
					return true;
				}
				Debug.Log("목적지 꽉 참.");
			}
			Debug.Log($"목적지 주인 있음. {inven[to].info.myName}");
			return false;
		}
	}

	void Swap(int a, int b)
	{
		InventoryItem slotItem = inven[a];
		inven[a] = inven[b];
		inven[b] = slotItem;
	}
}
