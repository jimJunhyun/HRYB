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

	public override string MyName 
	{
		get
		{
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < processes.Count; ++i)
			{
				sb.Append(processes[i].prefix);
			}
			sb.Append(myName);
			return sb.ToString();
		}
		set
		{
			myName = value;
		}
	}

	public float initDec;
	public float decPerSec;

	public List<PreProcess> processes = new List<PreProcess>(); //곱/제 연산의 추가를 고려하여 순서를 중요시.
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
			nameAsChar = MyName[UnityEngine.Random.Range(0, MyName.Length)];
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
		return MyName.GetHashCode();
	}
}
