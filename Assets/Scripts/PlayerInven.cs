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

	public bool isFull()
	{
		return number >= info.maxStack;
	}

	public int AddAmt(int amt)
	{
		number += amt;
		number %= info.maxStack;
		if(number == 0)
			number = info.maxStack;
		return amt - number;
	}

	public int SubtAmt(int amt)
	{
		number -= amt;
		if(number <= 0)
		{
			amt = -number;
			number = 0;
			return amt;
		}
		return 0;
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

	public List<int> Contains(Item info)
	{
		List<int> res = new List<int>();
		for (int i = 0; i < data.Count; i++)
		{
			if(!this[i].isEmpty() && this[i].info == info)
			{
				res.Add(i);
			}
		}
		return res;
	}

	public void AddCapacity(int amt)
	{
		data.Capacity += amt;
	}

	public int Add(InventoryItem item)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].isEmpty())
			{
				data[i] = item;
				++Count;
				return i;
			}
		}
		return -1;
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

	public int AddItem(Item data, int num = 1)
	{
		List<int> idxes;
		if ((idxes = inven.Contains(data)).Count > 0)
		{
			for (int i = 0; i < idxes.Count; i++)
			{
				int amt = inven[idxes[i]].AddAmt(num);
				if(amt != num)
				{
					Debug.Log($"{inven[idxes[i]].info.myName}, {inven[idxes[i]].number}개로 변경, 위치 {i}");
				}
				num = amt;
				if (num == 0)
				{
					return 0;
				}
			}
			int rep = num / data.maxStack + 1;

			for(int i = 0; i < rep; ++i)
			{
				int cnt = num - data.maxStack > 0 ? data.maxStack : num;
				num -= cnt;
				if(inven.Count < cap)
				{
					InventoryItem item = new InventoryItem(data, cnt);
					int idx = inven.Add(item);

					Debug.Log($"{item.info.myName}, {item.number}개, 새로 추가됨, 위치 : {idx}"); //갯수가 음수로 나오는 문제 발견,
				}
				else
				{
					return num;
				}
			}
		}
		else
		{
			int sets = num / data.maxStack + 1;
			for (int i = 0; i < sets; ++i)
			{
				int cnt = num - data.maxStack > 0 ? num - data.maxStack : num;
				num -= cnt;
				if (inven.Count < cap)
				{
					InventoryItem item = new InventoryItem(data, cnt);
					int idx = inven.Add(item);
					Debug.Log($"{item.info.myName}, {item.number}개, 새로 추가됨, 위치 : {idx}");
				}
				else
				{
					return num;
				}
			}
		}
		Debug.Log($"{data.myName}, {num} 만큼은 더이상 추가할 수 없음.");
		return num;
	}

	public int AddItem(Item data, int to, int num)
	{
		if(inven[to].isEmpty())
		{
			inven.Add(new InventoryItem(data, num), to);
			return 0;
		}
		else if(inven[to].info == data)
		{
			return inven[to].AddAmt(num);
		}
		return -1;
	}

	public bool RemoveItem(Item data, int num = 1)
	{
		List<int> idxes;
		if ((idxes = inven.Contains(data)).Count > 0)
		{
			int sum = 0;
			for (int i = 0; i < idxes.Count; i++)
			{
				sum += inven[idxes[i]].number;
			}

			if(sum >= num)
			{
				for (int i = 0; i < idxes.Count; i++)
				{
					num = inven[idxes[i]].SubtAmt(num);
					if(inven[idxes[i]].number == 0)
					{
						inven.Remove(idxes[i]);
					}
					if(num == 0)
					{
						break;
					}
				}
				return true;
			}
			Debug.Log("아이템 숫자 부족.");
			return false;
		}
		Debug.Log("아이템 없음.");
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
				int leftover;
				if ((leftover = AddItem(inven[from].info, to, num)) >= 0)
				{
					RemoveItem(from, num - leftover);
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
