using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifeModule : MonoBehaviour
{
	Actor self;

	public YinyangWuXing yywx;

	public WuXing limitation;

	public YinyangWuXing adequity;

	public float maxSoul;

	HashSet<StatusEffect> appliedDebuff = new HashSet<StatusEffect>();

	public virtual bool isDead
	{
		get => yywx.yy.yinAmt * 2 <= yywx.yy.yangAmt || yywx.yy.yangAmt * 2 <= yywx.yy.yinAmt || yywx.yy.yangAmt + yywx.yy.yinAmt > maxSoul;
	}

	private void Awake()
	{
		self = GetComponent<Actor>();
	}

	public void AddYY(float amt, YYInfo to)
	{
		yywx.yy[((int)to)] += amt * adequity.yy[((int)to)];
		if (isDead)
		{
			OnDead();
		}
	}

	public void AddWX(float amt, WXInfo to)
	{
		yywx.wx[((int)to)] += amt * adequity.wx[((int)to)];
		StatusEffect eff = ((StatusEffect)GameManager.instance.statEff.idStatEffPairs[(int)to]);
		if (yywx.wx[((int)to)] > limitation[((int)to)])
		{
			appliedDebuff.Add(eff);
			eff.onApplied.Invoke(self, self);
			self.updateActs += eff.onUpdated;
		}
		else if(appliedDebuff.Contains(eff))
		{
			appliedDebuff.Remove(eff);
			eff.onEnded.Invoke(self);
			self.updateActs -= eff.onUpdated;
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

	public void AddYYWX(YinyangWuXing data, float time)
	{
		StartCoroutine(DelAddYYWX(data, time));
	}


	IEnumerator DelAddYYWX(YinyangWuXing data, float t)
	{
		float curT = 0;
		YinyangWuXing incPerSec = new YinyangWuXing();
		incPerSec.yy.yinAmt = data.yy.yinAmt / t;
		incPerSec.yy.yangAmt = data.yy.yangAmt / t;

		incPerSec.wx.woodAmt = data.wx.woodAmt / t;
		incPerSec.wx.fireAmt = data.wx.fireAmt / t;
		incPerSec.wx.earthAmt = data.wx.earthAmt / t;
		incPerSec.wx.metalAmt = data.wx.metalAmt / t;
		incPerSec.wx.waterAmt = data.wx.waterAmt / t;

		while (curT < t)
		{
			curT += Time.deltaTime;
			yield return null;
			AddYY(incPerSec.yy.yinAmt * Time.deltaTime, YYInfo.Yin);
			AddYY(incPerSec.yy.yangAmt * Time.deltaTime, YYInfo.Yang);

			AddWX(incPerSec.wx.woodAmt * Time.deltaTime, WXInfo.Wood);
			AddWX(incPerSec.wx.fireAmt * Time.deltaTime, WXInfo.Fire);
			AddWX(incPerSec.wx.earthAmt * Time.deltaTime, WXInfo.Earth);
			AddWX(incPerSec.wx.metalAmt * Time.deltaTime, WXInfo.Metal);
			AddWX(incPerSec.wx.waterAmt * Time.deltaTime, WXInfo.Water);
		}
	}

	public virtual void OnDead()
	{
		StopAllCoroutines();
		PoolManager.ReturnObject(gameObject);
	}
}
