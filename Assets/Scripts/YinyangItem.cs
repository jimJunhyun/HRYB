using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class YinyangItem : Item
{
	public YinyangWuXing yywx;
	public List<PreProcess> processes;
	public char nameAsChar;

	public float applySpeed = 1f;
	
	public virtual float ApplySpeed 
	{
		get
		{
			return applySpeed;
		}
	}

    public YinyangItem(string name, ItemType iType, int max, System.Action used, bool isLateInit, char ch = ' ') : base(name, iType, max, used, isLateInit)
	{
		if(ch == ' ')
		{
			nameAsChar = myName[Random.Range(0, myName.Length)];
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
}
