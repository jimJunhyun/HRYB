using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public enum DetailParameter
{
	Sweet,
	Sour,
	Bitter,
	Salty,
	Spicy,
	Moist,
	Poison,

	Max
}

public class DetailAmount : Dictionary<DetailParameter, float>
{

	public DetailAmount()
	{
		for (int i = ((int)DetailParameter.Sweet); i < ((int)DetailParameter.Max); i++)
		{
			this.Add((DetailParameter)i , Mathf.Round(UnityEngine.Random.Range(0, 1) * 100) / 100f);
		}
	}

	public DetailAmount(float sw, float sr, float bt, float sa, float sp, float mo, float po)
	{
		this.Add(DetailParameter.Sweet, sw);
		this.Add(DetailParameter.Sour, sr);
		this.Add(DetailParameter.Bitter, bt);
		this.Add(DetailParameter.Salty, sa);
		this.Add(DetailParameter.Spicy, sp);
		this.Add(DetailParameter.Moist, mo);
		this.Add(DetailParameter.Poison, po);
	}

	public static DetailAmount Random
	{
		get
		{
			DetailAmount res = new DetailAmount();
			for (int i = ((int)DetailParameter.Sweet); i < ((int)DetailParameter.Max); i++)
			{
				res[(DetailParameter)i] =  Mathf.Round(UnityEngine.Random.Range(0f, 1f) * 100) / 100f;
				
			}
			return res;
		}
	}

	public static DetailAmount Empty
	{
		get
		{
			return new DetailAmount(0,0,0,0,0,0,0);
		}
	}
}



[System.Serializable]
public class YinyangItem : Item
{
	//YinYang data;
	//public YinYang yy
	//{
	//	get
	//	{
	//		return data;
	//	}
	//	set
	//	{
	//		data = value;
	//	}
	//}

	public HashSet<ProcessType> processes = new HashSet<ProcessType>();

	public DetailAmount detailParams = new DetailAmount();

	public override string MyName
	{
		get
		{
			StringBuilder sb;
			bool usingGlobal = GameManager.GetGlobalSB(out sb);

			if (processes.Contains(ProcessType.Stir))
			{
				sb.Append(PreProcess.STIRPREFIX);
			}
			if (processes.Contains(ProcessType.Fry))
			{
				sb.Append(PreProcess.FRYPREFIX);
			}
			if (processes.Contains(ProcessType.Burn))
			{
				sb.Append(PreProcess.BURNPREFIX);
			}
			if (processes.Contains(ProcessType.Mash))
			{
				sb.Append(PreProcess.MASHPREFIX);
			}
			sb.Append(base.MyName);
			string res = sb.ToString();
			sb.Clear();
			GameManager.ReturnGlobalSB(usingGlobal);

			return res;
		} 
		set => base.MyName = value;
	}

	//public float initDec;
	//public float decPerSec;

	public string nameAsChar;
	public bool usable;

	//public float applySpeed = 1f;
	
	//public virtual float ApplySpeed 
	//{
	//	get
	//	{
	//		return applySpeed;
	//	}
	//}

	public YinyangItem(YinyangItem item) : base(item)
	{
		//data = item.data;
		processes = new HashSet<ProcessType>(item.processes);
		if (item.nameAsChar == "")
		{
			nameAsChar = MyName[UnityEngine.Random.Range(0, MyName.Length)].ToString();
		}
		else
		{
			nameAsChar = item.nameAsChar;
		}
		detailParams = item.detailParams;
	}

    public YinyangItem(string name, string desc, ItemType iType, int max, Specials used, bool isNewItem, int price, DetailAmount det, bool isUsable, string ch = "") : base(name, desc, iType, max, used, price, isNewItem)
	{
		//data = yyData;
		if(ch == "")
		{
			nameAsChar = MyName[UnityEngine.Random.Range(0, MyName.Length)].ToString();
		}
		else
		{
			nameAsChar = ch;
		}
		detailParams = det;
		this.usable = isUsable;
	}

	public override void Use()
	{
		if (usable)
		{

			//GameManager.instance.pActor.life.DamageYY(yy, DamageType.Continuous, ApplySpeed);
			GameManager.instance.saver.pedia.UseItem(this);
			base.Use();
		}
	}

	public override int GetHashCode()
	{
		return MyName.GetHashCode();
	}

	public override int CompareTo(object obj)
	{
		if (obj is YinyangItem item)
		{
			if (originalName.CompareTo(item.originalName) == 0)
			{
				if(item.processes.Count == processes.Count)
				{
					return MyName.CompareTo(item.MyName);
				}
				else
					return processes.Count > item.processes.Count ? 1 : -1;
			}
			else
				return originalName.CompareTo(item.originalName);
		}
		return base.CompareTo(obj);
	}
}
