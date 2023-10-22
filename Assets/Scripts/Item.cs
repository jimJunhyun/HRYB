using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StackType
{
    numScale,
    litScale, 

}

public enum ItemType
{
	None = -1,
	Solid,
	Liquid,
	Air,
	Jelly,
}

public enum ItemRarity
{
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary,

	Medicine,
}

[System.Serializable]
public class Item
{
    public static Hashtable nameDataHashT = new Hashtable()
	{
		{"나뭇가지".GetHashCode(), new Item("나뭇가지", ItemType.Solid, StackType.numScale, ItemRarity.Common, 10, false) },
		{"인삼".GetHashCode(), new YinyangItem("인삼", ItemType.Solid, StackType.numScale, ItemRarity.Uncommon, 5, false, '삼') },
		{"물".GetHashCode(), new YinyangItem("물", ItemType.Solid, StackType.numScale, ItemRarity.Common, 10, false, '수')},
		{"밧줄".GetHashCode(), new Item("밧줄", ItemType.Solid, StackType.numScale, ItemRarity.Uncommon, 10, false) },
	}; //같은 이름의 아이템을 같은 물건으로 취급하기 위해 사용.
    public int Id {get => myName.GetHashCode();}
    public string myName;

    public Sprite icon;
	public ItemType itemType;
    public StackType stackType;

    public int maxStack;

	public ItemRarity rarity;

    public Item(string n, ItemType iType, StackType sType, ItemRarity grade, int max, bool isLateInit)
	{
        myName = n;
		if (isLateInit)
		{
			if (!nameDataHashT.ContainsKey(Id))
			{
				nameDataHashT.Add(Id, this);
			}
		}
		rarity = grade;
		itemType = iType;
		stackType = sType;
		maxStack = max;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Item left, Item right)
	{
		return left?.myName == right?.myName;
	}

    public static bool operator !=(Item left, Item right)
    {
		return left?.myName != right?.myName;
    }

    public override bool Equals(object obj)
	{
		bool res = (obj as Item)?.Id == this.Id;
		if ((obj as Item)?.Id == null)
		{
			return false;
		}
		return res;
	}
}
