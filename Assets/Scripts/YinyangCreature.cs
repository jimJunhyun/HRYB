using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YinyangCreature : MonoBehaviour
{
	public YinyangWuXing yywx;

	public WuXing limitation;

	public YinyangWuXing adequity;

	public float maxSoul;

	List<WXInfo> appliedDebuff = new List<WXInfo>();

	public bool isDead
	{
		get => yywx.yy.yinAmt * 2 >= yywx.yy.yangAmt || yywx.yy.yangAmt * 2 >= yywx.yy.yinAmt || yywx.yy.yangAmt + yywx.yy.yinAmt > maxSoul;
	}

	public void AddYY(float amt, YYInfo to)
	{
		yywx.yy[((int)to)] += amt * adequity.yy[((int)to)];
		if (isDead)
		{
			Debug.Log("!!!!");
		}
	}
	
	public void AddWH(float amt, WXInfo to)
	{
		yywx.wx[((int)to)] += amt * adequity.wx[((int)to)];
		if (yywx.wx[((int)to)] > limitation[((int)to)])
		{
			appliedDebuff.Add(to);
		}
	}
}
