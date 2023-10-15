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
		get => yywx.yy.yinAmt * 2 <= yywx.yy.yangAmt || yywx.yy.yangAmt * 2 <= yywx.yy.yinAmt || yywx.yy.yangAmt + yywx.yy.yinAmt > maxSoul;
	}

	public void AddYY(float amt, YYInfo to)
	{
		yywx.yy[((int)to)] += amt * adequity.yy[((int)to)];
		if (isDead)
		{
			Debug.Log("!!!!");
		}
	}
	
	public void AddWX(float amt, WXInfo to)
	{
		yywx.wx[((int)to)] += amt * adequity.wx[((int)to)];
		if (yywx.wx[((int)to)] > limitation[((int)to)])
		{
			appliedDebuff.Add(to);
		}
	}

	public void AddYYWX(YinyangWuXing data)
	{
		AddYY(data.yy.yinAmt, YYInfo.Yin);
		AddYY(data.yy.yangAmt, YYInfo.Yang);

		AddWX(data.wx.woodAmt, WXInfo.Wood);
		AddWX(data.wx.fireAmt, WXInfo.Fire);
		AddWX(data.wx.earthAmt, WXInfo.Earth);
		AddWX(data.wx.metalAmt, WXInfo.Metal);
		AddWX(data.wx.waterAmt, WXInfo.Water);
	}
}
