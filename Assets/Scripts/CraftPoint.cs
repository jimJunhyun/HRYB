using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class CraftPoint : InterPoint
{
	public HashSet<ItemAmountPair> holding = new HashSet<ItemAmountPair>();
	public HashSet<ItemAmountPair> result = new HashSet<ItemAmountPair>();
	Stack<ItemAmountPair> insertOrder = new Stack<ItemAmountPair>();

	public new UnityEvent onInter;
	public UnityEvent onAlternative;

	protected bool processing = false;

	protected int maxAmt;

	private void Awake()
	{
		onInter.AddListener( NormalInter);
		onAltInter.AddListener(AltInter);
	}

	public virtual void NormalInter() //아이템 넣기, 빼기
	{
		if(GameManager.instance.pinven.curHolding == -1 && maxAmt > holding.Count)
		{
			ItemAmountPair info = GameManager.instance.pinven.CurHoldingItem;
			if (info != ItemAmountPair.Empty)
			{
				ItemAmountPair data;
				if ((data = holding.Where(item => item.info == info.info).FirstOrDefault()).info != null)
				{
					holding.Remove(data);
					holding.Add(new ItemAmountPair(data.info, data.num + 1));
				}
				else
				{
					holding.Add(new ItemAmountPair(info.info, 1));
					insertOrder.Push(info);
				}

				if(info.num - 1 == 0)
				{
					GameManager.instance.pinven.CurHoldingItem = ItemAmountPair.Empty;
				}
				else
				{
					GameManager.instance.pinven.CurHoldingItem = new ItemAmountPair(info.info, info.num - 1);
				}
				
				Debug.Log($"ADDED {info.info.myName}");
			}
		}
		else
		{
			if(insertOrder.Count > 0)
			{
				ItemAmountPair info = insertOrder.Peek();
				if (GameManager.instance.pinven.AddItem(info.info, info.num) == 0)
				{
					insertOrder.Pop();
					if ((info = holding.Where(item => item.info == info.info).FirstOrDefault()).info != null) // #######################
					{

					}
				}
			}
			
		}
	}

	public virtual void AltInter() //프로세스 시작, 종료
	{
		if (processing)
		{
			Stop();
		}
		else if(holding.Count > 0)
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
}
