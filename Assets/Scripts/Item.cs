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
	public System.Action onActivated;
	public float removeTime;
}

[System.Serializable]
public class Item
{
    public static Hashtable nameDataHashT = new Hashtable()
	{
		{"나뭇가지".GetHashCode(), new Item("나뭇가지", ItemType.Solid, 10, null, false) },
		{"인삼".GetHashCode(), new YinyangItem("인삼", ItemType.Solid, 5, null, false, new YinyangWuXing(1.0f, 1.0f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f), '삼') },
		{"물".GetHashCode(), new YinyangItem("물", ItemType.Liquid,  5, null, false, new YinyangWuXing(1.0f, 1.0f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f), '수')},
		{"밧줄".GetHashCode(), new Item("밧줄", ItemType.Solid,  10, null, false) },
	}; //같은 이름의 아이템을 같은 물건으로 취급하기 위해 사용.
    public int Id {get => myName.GetHashCode();}
    public string myName;

    public Sprite icon;
	public ItemType itemType;

	public System.Action onUse;

    public int maxStack;

	public ItemRarity rarity;

	float modifier = 1f;

	public void SetRarity(ItemRarity val)
	{
		rarity = val;
		switch (val)
		{
			case ItemRarity.S://배수 정하기
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

	public Item(string n, ItemType iType, int max, System.Action useFunc ,bool isLateInit) // ######################################
	{
        myName = n;
		if (isLateInit)
		{
			if (!nameDataHashT.ContainsKey(Id))
			{
				nameDataHashT.Add(Id, this);
			}
		}
		itemType = iType;
		maxStack = max;
		onUse = useFunc;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Item left, Item right)
	{
		return left?.myName == right?.myName && left?.rarity == right?.rarity;
	}

    public static bool operator !=(Item left, Item right)
    {
		return left?.myName != right?.myName || left?.rarity != right?.rarity;
    }

    public override bool Equals(object obj)
	{
		bool res = (obj as Item)?.Id == this.Id && (obj as Item).rarity == this.rarity;
		if ((obj as Item)?.Id == null)
		{
			return false;
		}
		return res;
	}

	public virtual void Use()
	{
		onUse.Invoke();
	}
}

