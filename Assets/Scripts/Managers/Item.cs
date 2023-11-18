using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
	None = -1,
	Solid,
	Liquid,
	Air,
}

public enum ItemRarity
{
	S,
	A,
	B,
	C,
	D,
	F,

	Medicine,
	Material,
}

public class Specials
{
	public System.Func<bool> onActivated;
	public float removeTime;

	public void DeleteSpecial()
	{
		onActivated = null;
	}

	public Specials(System.Func<bool> onAct, float removeT)
	{
		onActivated = onAct;
		removeTime = removeT;
	}
}

[System.Serializable]
public class Item // #################
{
    public static Hashtable nameDataHashT = new Hashtable()
	{
		{"나뭇가지".GetHashCode(), new Item("나뭇가지","나무에서 열리는 가지를 나뭇가지라고 부르더라", ItemType.Solid, 10, null, false) },
		{"인삼".GetHashCode(), new YinyangItem("인삼", "사실 인삼이나 산삼이나 거기서 거기다", ItemType.Solid, 5, null, false, new YinyangWuXing(1.0f, 1.0f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f), "삼") },
		{"물".GetHashCode(), new YinyangItem("물", "물로 보지 말랜다", ItemType.Liquid,  5, null, false, new YinyangWuXing(1.0f, 1.0f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f), "수")},
		{"밧줄".GetHashCode(), new Item("밧줄", "무언가에 매기 위해 맨 줄이다.", ItemType.Solid,  10, new Specials(()=>
		{
			return (GameManager.instance.pActor.atk as PlayerAttack).ThrowRope();
		}, Mathf.Infinity), false) },
	}; //같은 이름의 아이템을 같은 물건으로 취급하기 위해 사용.
    public int Id {get => MyName.GetHashCode();}
    protected string myName;
	public virtual string MyName
	{
		get => myName; set => myName = value;
	}

	public string desc;

    public Sprite icon;
	public ItemType itemType;

	public Specials onUse;

    public int maxStack;

	public ItemRarity rarity;

	

	public void SetRarity(ItemRarity val)
	{
		rarity = val;
		switch (val)
		{
			case ItemRarity.S:
				break;
			case ItemRarity.A:
				break;
			case ItemRarity.B:
				break;
			case ItemRarity.C:
				break;
			case ItemRarity.D:
				break;
			case ItemRarity.F:
				break;
			default:
				break;
		}
	}

	public Item(Item dat)
	{
		myName = dat.myName;
		desc = dat.desc;
		itemType = dat.itemType;
		maxStack = dat.maxStack;
		onUse = dat.onUse;
		icon = dat.icon;
	}

	public Item(string n, string d, ItemType iType, int max, Specials useFunc ,bool isNewItem)
	{
        myName = n;
		desc = d;
		if (isNewItem)
		{
			InsertToTable();
		}
		itemType = iType;
		maxStack = max;
		onUse = useFunc;
	}

	public virtual void InsertToTable()
	{
		if (!nameDataHashT.ContainsKey(Id))
		{
			Debug.Log($"새로운 아이템 : {MyName}");
			nameDataHashT.Add(Id, this);
		}
	}

	public override int GetHashCode()
	{
		return MyName.GetHashCode();
	}

	public static bool operator ==(Item left, Item right)
	{
		Debug.Log($"{left?.MyName} == {right?.MyName} --> {left?.MyName == right?.MyName }");
		return left?.MyName == right?.MyName ;
	}

    public static bool operator !=(Item left, Item right)
    {
		return left?.MyName != right?.MyName ;
    }

    public override bool Equals(object obj)
	{
		Debug.Log("???");
		return obj is Item i && i.MyName == MyName;
	}

	public virtual void Use()
	{
		if (onUse != null && onUse.onActivated != null && onUse.onActivated.Invoke())
		{

			GameManager.instance.pinven.RemoveItem(this);
		}
	}

	public void SetSprite(Sprite sp)
	{
		icon = sp;
	}
}

