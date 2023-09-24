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
    public static Hashtable nameHashT = new Hashtable();
    public int Id {get => myName.GetHashCode();}
    public string myName;

    public Sprite icon;
	public ItemType itemType;
    public StackType stackType;

    public int maxStack;

    public Item(string n, ItemType iType, StackType sType, int max)
	{
        myName = n;
		if (!nameHashT.ContainsKey(Id))
		{
			nameHashT.Add(Id, this);
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
