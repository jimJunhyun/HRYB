using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


[System.Serializable]
public class YinyangItem : Item
{
	YinyangWuXing data;
	public YinyangWuXing yywx
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}

	public HashSet<ProcessType> processes = new HashSet<ProcessType>();

	public float initDec;
	public float decPerSec;

	public string nameAsChar;

	public float applySpeed = 1f;
	
	public virtual float ApplySpeed 
	{
		get
		{
			return applySpeed;
		}
	}

	public YinyangItem(YinyangItem item) : base(item)
	{
		data = item.data;
		data.isClampedZero = true;
		if (item.nameAsChar == "")
		{
			nameAsChar = MyName[UnityEngine.Random.Range(0, MyName.Length)].ToString();
		}
		else
		{
			nameAsChar = item.nameAsChar;
		}
	}

    public YinyangItem(string name, string desc, ItemType iType, int max, Specials used, bool isNewItem, YinyangWuXing yyData, string ch = "") : base(name, desc, iType, max, used, isNewItem)
	{
		data = yyData;
		data.isClampedZero = true;
		if(ch == "")
		{
			nameAsChar = MyName[UnityEngine.Random.Range(0, MyName.Length)].ToString();
		}
		else
		{
			nameAsChar = ch;
		}
	}

	public override void Use()
	{
		GameManager.instance.pActor.life.AddYYWX(yywx, ApplySpeed);
		base.Use();
	}

	public override int GetHashCode()
	{
		return MyName.GetHashCode();
	}
}
