using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class YinyangItem : Item
{
	YinyangWuXing data;
	public YinyangWuXing yywx
	{
		get
		{
			YinyangWuXing tmp = data;
			for (int i = 0; i < processes.Count; i++)
			{
				switch (processes[i].type)
				{
					case ProcessType.Roast:
						tmp -= ((Roast)processes[i]).Decreased;
						break;
					default:
						break;
				}
			}
			return tmp;
		}
		set
		{
			data = value;
		}
	}

	public float initDec;
	public float decPerSec;

	public List<PreProcess> processes = new List<PreProcess>();
	public char nameAsChar;

	public float applySpeed = 1f;
	
	public virtual float ApplySpeed 
	{
		get
		{
			return applySpeed;
		}
	}

    public YinyangItem(string name, ItemType iType, int max, Specials used, bool isLateInit, YinyangWuXing yyData, char ch = ' ') : base(name, iType, max, used, isLateInit)
	{
		data = yyData;
		data.isClampedZero = true;
		if(ch == ' ')
		{
			nameAsChar = myName[UnityEngine.Random.Range(0, myName.Length)];
		}
		else
		{
			nameAsChar = ch;
		}
	}

	public override void Use()
	{
		GameManager.instance.pLife.AddYYWX(yywx, ApplySpeed);
		base.Use();
	}

	public override int GetHashCode()
	{
		int hash1 = 13;
		for (int i = 0; i < processes.Count; i++)
		{
			hash1 += processes[i].GetHashCode();
		}
		hash1 = HashCode.Combine(hash1, myName);
		return hash1;
	}
}
