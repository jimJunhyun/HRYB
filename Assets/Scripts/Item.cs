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

[System.Serializable]
public class Item
{
    public static Hashtable nameDataHashT = new Hashtable(); //같은 이름의 아이템을 같은 물건으로 취급하기 위해 사용.
    public int Id {get => myName.GetHashCode();}
    public string myName;

    public Sprite icon;
	public ItemType itemType;
    public StackType stackType;

    public int maxStack;

    public Item(string n, ItemType iType, StackType sType, int max)
	{
        myName = n;
		if (!nameDataHashT.ContainsKey(Id))
		{
			nameDataHashT.Add(Id, this);
		}
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
		return !(left != right);
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
