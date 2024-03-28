using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerForm
{
	//None,
	Magic,
	Yoho,
	
}

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
		
		if(number > info.maxStack)
		{
			int overs = number - info.maxStack;
			number = info.maxStack;
			return overs;
		}
		else
		{
			Debug.Log(number);
			return 0;
		}
		
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

	public ItemAmountPair ToPair()
	{
		return new ItemAmountPair(info, number);
	}
}

public struct InvenSkill
{
	public SkillRoot info;
	public int number;

	public bool isEmpty()
	{
		return info == null || number == 0;
	}

	public override bool Equals(object obj)
	{
		return obj is InvenSkill skill &&
			   EqualityComparer<SkillRoot>.Default.Equals(info, skill.info);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(info);
	}

	public InvenSkill(SkillRoot info, int num)
	{
		this.info = info;
		number = num;
	}

	public static InvenSkill Empty
	{
		get=>new InvenSkill(null, 0);
	}

	public static bool operator ==(InvenSkill lft, InvenSkill rht)
	{
		if(lft.info == null)
			return rht.info == null;
		if(rht.info == null)
			return lft.info == null;
		return lft.info == rht.info;
	}

	public static bool operator !=(InvenSkill lft, InvenSkill rht)
	{
		if (lft.info == null)
			return rht.info != null;
		if (rht.info == null)
			return lft.info != null;
		return lft.info != rht.info;
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
	int quickSize = 5;

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

	public int SumContains(Item info)
	{
		int sum = 0;
		for (int i = 0; i < data.Count; i++)
		{
			if (!this[i].isEmpty() && this[i].info == info)
			{
				sum += this[i].number;
			}
		}
		return sum	;
	}

	public void AddCapacity(int amt)
	{
		data.Capacity += amt;
		for (int i = 0; i < amt; i++)
		{
			data.Add(new InventoryItem(null, 0));
		}
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
		--Count;
	}
}

public class SkillInventory
{
	public SkillInventory()
	{
		data = new HashSet<InvenSkill>();
	}
	HashSet<InvenSkill> data;

	public void AddSkill(SkillRoot info, int cnt = 1)
	{
		foreach (var item in data)
		{
			if(item.info == info)
			{
				InvenSkill sk = item;
				data.Remove(item);
				sk.number += cnt;
				data.Add(sk);
				break;
			}
		}
	}

	public void RemoveSkill(SkillRoot info, int cnt = 1)
	{
		foreach (var item in data)
		{
			if (item.info == info)
			{
				InvenSkill sk = item;
				data.Remove(item);
				if(sk.number > cnt)
				{
					sk.number -= cnt;
					data.Add(sk);
				}
				break;
			}
		}
	}
}

public class PlayerInven : MonoBehaviour
{
    public Inventory inven;
	public SkillInventory skInven = new SkillInventory();
    public int cap = 21;

	public string swapEffectName;
	public Vector3 swapEffectRot;
	public Vector3 swapEffectScale;

	public float changeCool;

	bool clickWood = false;
	bool clickFire = false;
	bool clickEarth = false;
	bool clickMetal = false;
	bool clickWater = false;

	public PlayerForm stat = PlayerForm.Magic;
	public int curHolding = 0;
	PlayerAnimActions animActions;

	float prevChange;
	
	public ItemAmountPair CurHoldingItem 
	{ 
		get 
		{
			if(curHolding == -1)
				return ItemAmountPair.Empty;
			return inven[curHolding].ToPair();
		}
		set
		{
			if(curHolding != -1)
			{
				inven[curHolding] = value.ToInven();
			}
		}
	}
	public bool HoldingYinYangItem
	{
		get => CurHoldingItem != ItemAmountPair.Empty && CurHoldingItem.info is YinyangItem;
	}
	public bool isFull { get => inven.Count >= cap;}

	private void Awake()
	{
		inven = new Inventory(cap);
		animActions = GetComponentInChildren<PlayerAnimActions>();
	}

	private void Start()
	{
		animActions.ChangeFormTo(stat);
		(GameManager.instance.pActor.cast as PlayerCast).ChangeSkillSlotTo(stat);
	}

	public int AddItem(Item data, int num = 1)
	{
		List<int> idxes;
		switch (data.MyName)
		{
			case "녹각":
			case "녹제":
				GameManager.instance.qManager.NextIf(Quests.GETDEER);
				break;
			case "활":
				GameManager.instance.qManager.NextIf(Quests.GETBOW);
				break;
			case "밧줄":
				GameManager.instance.qManager.NextIf(Quests.GETROPE);
				break;
			case "녹용":
				GameManager.instance.qManager.NextIf(Quests.GETDEERHORN);
				break;
			case "도약탕":
				GameManager.instance.qManager.NextIf(Quests.GETJUMPMEDICINE);
				break;
			default:
				break;
		}
		if ((idxes = inven.Contains(data)).Count > 0)
		{
			for (int i = 0; i < idxes.Count; i++)
			{
				InventoryItem item = inven[idxes[i]];
				int amt = item.AddAmt(num);
				inven[idxes[i]] = item;
				if(amt != num)
				{
					Debug.Log($"{inven[idxes[i]].info.MyName}, {inven[idxes[i]].number}개로 변경, 위치 {i}");
				}
				num = amt;
				if (num == 0)
				{
					GameManager.instance.uiManager.UpdateInvenUI();
					return 0;
				}
			}
			int rep = num / data.maxStack + ((num % data.maxStack) == 0 ? 0 : 1);

			for(int i = 0; i < rep; ++i)
			{
				int cnt = num - data.maxStack > 0 ? data.maxStack : num;
				num -= cnt;
				if(inven.Count < cap)
				{
					InventoryItem item = new InventoryItem(data, cnt);
					int idx = inven.Add(item);

					Debug.Log($"{item.info.MyName}, {item.number}개, 새로 추가됨, 위치 : {idx}");
					GameManager.instance.uiManager.UpdateInvenUI();
				}
				else
				{
					Debug.Log($"{data.MyName}, {num} 만큼은 더이상 추가할 수 없음.");
					GameManager.instance.uiManager.UpdateInvenUI();
					return num;
				}
			}
		}
		else
		{
			int sets = num / data.maxStack + ((num % data.maxStack) == 0 ? 0 : 1);
			for (int i = 0; i < sets; ++i)
			{
				int cnt = num - data.maxStack > 0 ? data.maxStack : num;
				num -= cnt;
				if (inven.Count < cap)
				{
					InventoryItem item = new InventoryItem(data, cnt);
					int idx = inven.Add(item);
					Debug.Log($"{item.info.MyName}, {item.number}개, 새로 추가됨, 위치 : {idx}");
				}
				else
				{
					Debug.Log($"{data.MyName}, {num} 만큼은 더이상 추가할 수 없음.");
					GameManager.instance.uiManager.UpdateInvenUI();
					return num;
				}
			}
		}
		
		GameManager.instance.uiManager.UpdateInvenUI();
		Debug.Log($"{data.MyName}, {num} 만큼은 더이상 추가할 수 없음.");
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
			InventoryItem item = inven[to];
			int leftover = item.AddAmt(num);
			inven[to] = item;
			return leftover;
		}
		GameManager.instance.uiManager.UpdateInvenUI();
		return -1;
	}

	public bool RemoveItem(Item data, int num = 1)
	{
		List<int> idxes;
		if ((idxes = inven.Contains(data)).Count > 0)
		{
			int sum = inven.SumContains(data);

			if(sum >= num)
			{
				for (int i = 0; i < idxes.Count; i++)
				{
					InventoryItem item = inven[idxes[i]];
					num = item.SubtAmt(num);
					inven[idxes[i]] = item;
					if(inven[idxes[i]].number == 0)
					{
						inven.Remove(idxes[i]);
					}
					if(num == 0)
					{
						break;
					}
				}
				GameManager.instance.uiManager.UpdateInvenUI();
				return true;
			}
			Debug.Log("아이템 숫자 부족.");
			return false;
		}
		Debug.Log("아이템 없음.");
		return false;
	}

	public bool RemoveItemExamine(Item data, int num = 1)
	{
		if ((inven.Contains(data)).Count > 0)
		{
			int sum = inven.SumContains(data);

			if (sum >= num)
			{
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
			GameManager.instance.uiManager.UpdateInvenUI();
			return true;
		}
		return false;
	}

	public bool Move(int from, int to, int num)
	{
		if(!inven[from].isEmpty() && !inven[to].isEmpty() && inven[from].info != inven[to].info && num == inven[from].number)
		{
			Debug.Log($"{inven[from].info.MyName}, {inven[from].number}개, {inven[to].info.MyName}, {inven[to].number}개에서, ");
			Swap(from, to);
			Debug.Log($"{inven[from].info.MyName}, {inven[from].number}개, {inven[to].info.MyName}, {inven[to].number}개로 변경.");

			GameManager.instance.uiManager.UpdateInvenUI();
			return true;
		}
		else
		{
			if ((!inven[from].isEmpty() && inven[to].isEmpty()) || (inven[from].info == inven[to].info))
			{
				Debug.Log($"{(inven[from].isEmpty() ? 0 : inven[from].info.MyName)}, {inven[from].number}개, {(inven[to].isEmpty() ? 0 : inven[to].info.MyName)}, {(inven[to].isEmpty() ? 0 : inven[to].number)}개에서, ");
				int leftover;
				if ((leftover = AddItem(inven[from].info, to, num)) >= 0)
				{
					RemoveItem(from, num - leftover);
					Debug.Log($"{(inven[from].isEmpty() ? 0 : inven[from].info.MyName)}, {inven[from].number}개, {(inven[to].isEmpty() ? 0 : inven[to].info.MyName)}, {(inven[to].isEmpty() ? 0 : inven[to].number)}개로 변경.");
					GameManager.instance.uiManager.UpdateInvenUI();
					return true;
				}
				Debug.Log("목적지 꽉 참.");
			}
			Debug.Log($"목적지 주인 있음. {inven[to].info.MyName}");
			return false;
		}
	}

	public void ObtainWeapon()
	{
		//attackable = true;
		//(GameManager.instance.pActor.atk as PlayerAttack).ObtainBowRender();
	}

	public void SwitchHand(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if(Time.time - prevChange >= changeCool)
			{
				if ((GameManager.instance.pActor.move as PlayerMove).isGrounded
				&& !clickWood && !clickFire && !clickMetal && !clickWater && !clickEarth)
				{
					switch (stat)
					{
						case PlayerForm.Yoho:
							if ((GameManager.instance.pActor.atk is PlayerAttack atk) &&
								!atk.clickL && !atk.clickR)
							{
								stat = PlayerForm.Magic;
							}
							break;
						case PlayerForm.Magic:
							if ((GameManager.instance.pActor.atk is PlayerAttack tk) &&
								!tk.clickL && !tk.clickR)
							{
								stat = PlayerForm.Yoho;
							}
							break;
						default:
							break;
					}
					GameObject obj = PoolManager.GetObject(swapEffectName, transform, 1.5f);
					obj.transform.rotation = Quaternion.Euler(swapEffectRot);
					obj.transform.localScale = swapEffectScale;
					RefreshStat();
					prevChange = Time.time;
				}
			}
			
		}
	}

	public void RefreshStat()
	{
		(GameManager.instance.pActor.anim as PlayerAnim).SetChangeTrigger();
		(GameManager.instance.pActor.cast as PlayerCast).ChangeSkillSlotTo(stat);
		
	}

	public void Hold(PlayerForm stat, int idx)
	{
		// switch (stat)
		// {
		// 	//case HandStat.None:
		// 	//	curHolding = -1;
		// 	//	break;
		// 	case HandStat.Magic:
		// 		curHolding = -1;
		// 		break;
		// 	case HandStat.Item:
		// 		if(this.stat == stat)
		// 		{
		// 			curHolding = idx;
		// 		}
		// 		else
		// 		{
		// 			curHolding = 0;
		// 		}
		// 		break;
		// }
		// this.stat = stat;
		// GameManager.instance.uiManager.UpdateInvenUI();
	}

	public void UseHolding()
	{
		if (!inven[curHolding].isEmpty())
		{
			inven[curHolding].info.onUse.onActivated.Invoke();
		}
		GameManager.instance.uiManager.UpdateInvenUI();
	}

	public bool RemoveHolding(int amt = 1)
	{
		if(CurHoldingItem.num >= amt)
		{
			if (CurHoldingItem.num - amt <= 0)
			{
				CurHoldingItem = ItemAmountPair.Empty;
			}
			else
			{
				CurHoldingItem = new ItemAmountPair(CurHoldingItem.info, CurHoldingItem.num - amt);
			}
			GameManager.instance.uiManager.UpdateInvenUI();
			return true;
		}
		else
		{
			return false;
		}
		
	}

	public void AddSkill(SkillRoot info, int cnt = 1)
	{
		skInven.AddSkill(info, cnt);
	}

	public void RemoveSkill(SkillRoot info, int cnt = 1)
	{
		skInven.RemoveSkill(info, cnt);
	}

	void Swap(int a, int b)
	{
		InventoryItem slotItem = inven[a];
		inven[a] = inven[b];
		inven[b] = slotItem;
	}

	public void OnPressOne(InputAction.CallbackContext context)
	{
		//if (stat == PlayerForm.Magic)
		//{
			if(GameManager.instance.pActor.atk.attackModuleStat.Paused && !clickWood)
				return;
			if (context.started && !clickWood)
			{
				clickWood = true;
				(GameManager.instance.pActor.cast as PlayerCast).SetSkillUse(SkillSlotInfo.One);
			}
			else if (context.canceled && clickWood)
			{
				clickWood = false;
				(GameManager.instance.pActor.cast as PlayerCast).ResetSkillUse(SkillSlotInfo.One);
			}
		//}
		// else if (context.started && stat == HandStat.Yoho)
		// {
		// 	Hold(HandStat.Item, 0);
		// 	UseHolding();
		// }
		
	}

	public void OnPressTwo(InputAction.CallbackContext context)
	{
		//if (stat == PlayerForm.Magic)
		//{
			if (GameManager.instance.pActor.atk.attackModuleStat.Paused && !clickFire)
				return;
			if (context.started && !clickFire)
			{
			Debug.Log("ㅁㅁㅁㅁㅁㅁㅁㅁ");
				clickFire = true;
				(GameManager.instance.pActor.cast as PlayerCast).SetSkillUse(SkillSlotInfo.Two);
			}
			else if (context.canceled && clickFire)
			{
			Debug.Log("ㅊㅊㅊㅊㅊㅊㅊㅊ");
				clickFire = false;
				(GameManager.instance.pActor.cast as PlayerCast).ResetSkillUse(SkillSlotInfo.Two);
			}
		//}
		// else if (stat == HandStat.Yoho && context.started)
		// {
		// 	Hold(HandStat.Yoho, 1);
		// 	UseHolding();
		// }

	}

	public void OnPressThree(InputAction.CallbackContext context)
	{
		//if (stat == PlayerForm.Magic)
		//{
			if (GameManager.instance.pActor.atk.attackModuleStat.Paused && !clickEarth)
				return;
			if (context.started && !clickEarth)
			{
				clickEarth = true;
				(GameManager.instance.pActor.cast as PlayerCast).SetSkillUse(SkillSlotInfo.Three);
			}
			else if (context.canceled && clickEarth)
			{
				clickEarth = false;
				(GameManager.instance.pActor.cast as PlayerCast).ResetSkillUse(SkillSlotInfo.Three);
			}
		//}
		// else if (stat == HandStat.Item && context.started)
		// {
		// 	Hold(HandStat.Item, 2);
		// 	UseHolding();
		// }
	}

	public void OnPressFour(InputAction.CallbackContext context)
	{
		// if (stat == HandStat.Item && context.started)
		// {
		// 	Hold(HandStat.Item, 3);
		// 	UseHolding();
		// }
	}

	public void OnPressQ(InputAction.CallbackContext context)
	{
		//if (stat == PlayerForm.Magic)
		//{
			if (GameManager.instance.pActor.atk.attackModuleStat.Paused && !clickMetal)
				return;
			if (context.started && !clickMetal)
			{
				clickMetal = true;
				(GameManager.instance.pActor.cast as PlayerCast).SetSkillUse(SkillSlotInfo.Q);
			}
			else if (context.canceled && clickMetal)
			{
				clickMetal = false;
				(GameManager.instance.pActor.cast as PlayerCast).ResetSkillUse(SkillSlotInfo.Q);
			}
		//}
	}

	public void OnPressFive(InputAction.CallbackContext context)
	{
		// if (stat == HandStat.Item && context.started)
		// {
		// 	Hold(HandStat.Item, 4);
		// 	UseHolding();
		// }
	}

	public void OnPressE(InputAction.CallbackContext context)
	{
		//if (stat == PlayerForm.Magic)
		//{
			if (GameManager.instance.pActor.atk.attackModuleStat.Paused && !clickWater)
				return;
			if (context.started && !clickWater)
			{
				clickWater = true;
				(GameManager.instance.pActor.cast as PlayerCast).SetSkillUse(SkillSlotInfo.E);
			}
			else if (context.canceled && clickWater)
			{
				clickWater = false;
				(GameManager.instance.pActor.cast as PlayerCast).ResetSkillUse(SkillSlotInfo.E);
			}
		//}
	}
}
