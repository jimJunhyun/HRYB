using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class CraftPoint : InterPoint
{
	public HashSet<ItemAmountPair> holding = new HashSet<ItemAmountPair>();
	public HashSet<ItemAmountPair> result = new HashSet<ItemAmountPair>();
	protected Stack<ItemAmountPair> insertOrder = new Stack<ItemAmountPair>();

	protected bool processing = false;

	protected virtual int maxAmt{ get; set;}
	protected int count;

	

	//private void Awake()
	//{
	//	  onInter.AddListener( NormalInter);
	//	  onAltInter.AddListener(AltInter);
	//}

	public override void Inter() //아이템 넣기, 빼기
	{
		if (!processing)
		{
			if (GameManager.instance.pinven.CurHoldingItem != ItemAmountPair.Empty && count < maxAmt)
			{
				Debug.Log("넣음");
				Put();
			}
			else if (insertOrder.Count > 0)
			{
				Debug.Log("뺌");
				Pop();
			}
			else
			{
				Debug.Log($"아이템을 들고있지 않음 : {GameManager.instance.pinven.CurHoldingItem == ItemAmountPair.Empty} 최대치에 도달 {count >= maxAmt}");
			}
		}
		
	}

	public virtual void Put()
	{
		if(count < maxAmt)
		{
			ItemAmountPair info = GameManager.instance.pinven.CurHoldingItem;
			if (GameManager.instance.pinven.HoldingYinYangItem && GameManager.instance.pinven.RemoveHolding(1))
			{
				ItemAmountPair data;
				if ((data = holding.Where(item => item.info == info.info).FirstOrDefault()).info != null)
				{
					holding.Remove(data);
					holding.Add(new ItemAmountPair(data.info, data.num + 1));
					insertOrder.Push(new ItemAmountPair(info.info, 1));
					Debug.Log($"{data.info.MyName} 1개 추가됨. {data.num + 1}개");
					count += 1;
				}
				else
				{
					data = new ItemAmountPair(new YinyangItem(info.info as YinyangItem), 1);
					holding.Add(data);
					insertOrder.Push(data);
					Debug.Log($"{info.info.MyName} 1개 새로 추가됨.");
					count += 1;
				}

				Debug.Log($"ADDED {info.info.MyName}");
			}
			else
				Debug.Log("아이템이 없거나 넣을 수 없는 아이템.");
		}
		else
		{
			Debug.Log($"최대치 도달 : {maxAmt}");
		}
	}

	public virtual void Pop()
	{
		ItemAmountPair info = insertOrder.Peek();
		if (GameManager.instance.pinven.AddItem(info.info, info.num) == 0)
		{
			insertOrder.Pop();
			if ((info = holding.Where(item => item.info == info.info).FirstOrDefault()).info != null)
			{
				holding.Remove(info);
				if (info.num - 1 > 0)
				{
					holding.Add(new ItemAmountPair(info.info, info.num - 1));
				}
				Debug.Log($"아이템 뺌 : {info.info.MyName}, {info.num - 1}개.");
				count -= 1;
			}
			else
				 Debug.Log("넣은 아이템을 찾을 수 없음");
		}
		else
			Debug.Log("인벤 꽉참.");
	}

	public override void AltInter() //프로세스 시작, 종료
	{
		if (processing)
		{
			Stop();
		}
		else if (holding.Count > 0)
		{
			Process();
		}
	}

	public virtual void Process()
	{
		processing = true;
	}

	public virtual void Stop()
	{
		processing = false;
	}

	public virtual string GetMedicineName()
	{
		return "";
	}

	public virtual void Initialize()
	{
		processing = false;
		holding.Clear();
		result.Clear();
		insertOrder.Clear();
		count = 0;
	}
}
