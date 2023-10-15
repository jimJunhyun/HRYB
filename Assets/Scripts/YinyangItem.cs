using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class YinyangItem : Item
{
	public YinyangWuXing yywx;
	public List<PreProcess> processes;
	public char nameAsChar;

    public YinyangItem(string name, ItemType iType, StackType sType, int max, bool isLateInit, char ch = ' ') : base(name, iType, sType, max, isLateInit)
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
    
}
