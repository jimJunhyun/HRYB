using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CraftPoint : InterPoint
{
	public HashSet<YinyangItem> holding = new HashSet<YinyangItem>();
	public HashSet<YinyangItem> result = new HashSet<YinyangItem>();
	Stack<YinyangItem> insertOrder = new Stack<YinyangItem>();

	public new UnityEvent onInter;
	public UnityEvent onAlternative;

	protected bool processing = false;

	private void Awake()
	{
		onInter.AddListener( NormalInter);
		onAltInter.AddListener(AltInter);
	}

	public virtual void NormalInter() //아이템 넣기, 프로세스 시작
	{
		if(GameManager.instance.pinven.curHolding == -1 || GameManager.instance.pinven.inven[GameManager.instance.pinven.curHolding].isEmpty())
		{
			Process();
		}
		else
		{
			YinyangItem info = Item.nameDataHashT[GameManager.instance.pinven.curHolding] as YinyangItem;
			if (info != null)
			{
				holding.Add(info);
				insertOrder.Push(info);
				Debug.Log($"ADDED {info.myName}");
			}
		}
		
	}

	public virtual void AltInter() //아이템 빼내기 또는 (프로세스 종료 (는 상속후 경우에서만 있다.))
	{
		YinyangItem top = insertOrder.Pop();
		if (GameManager.instance.pinven.AddItem(top) == 0)
		{
			holding.Remove(top);
			Debug.Log($"Removed {top.myName}");
		}

	}

	public virtual void Process()
	{
		processing = true;
	}
}
