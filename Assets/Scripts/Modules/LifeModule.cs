using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifeModule : Module
{
	public const float IMMUNETIME = 0.3f;

	public bool isImmune = false;

	public YinyangWuXing yywx;

	public WuXing limitation;

	public YinyangWuXing adequity;

	public float maxSoul;

	public float regenMod = 1f;
	public float? fixedRegenMod = null;
	float baseRegen = 1f;
	public float TotalRegenSpeed { get => (fixedRegenMod == null ? regenMod : (float)fixedRegenMod) * baseRegen;}

	public float regenThreshold = 10f;

	public float applyMod = 1f;
	float baseApplySpeed = 1f;

	public float TotalApplySpeed { get => baseApplySpeed * applyMod;}


	HashSet<StatusEffect> appliedDebuff = new HashSet<StatusEffect>();

	public virtual bool isDead
	{
		get => yywx.yy.yinAmt * 2 <= yywx.yy.yangAmt || yywx.yy.yangAmt * 2 <= yywx.yy.yinAmt || yywx.yy.yangAmt + yywx.yy.yinAmt > maxSoul;
	}

	protected bool regenOn = false;
	float diff;

	public virtual void Update()
	{
		if(Mathf.Abs((diff = yywx.yy.yinAmt - yywx.yy.yangAmt)) > regenThreshold)
		{
			regenOn = true;
			if(diff > 0)
			{
				yywx.yy.yinAmt -= TotalRegenSpeed * Time.deltaTime;
				yywx.yy.yangAmt += TotalRegenSpeed * Time.deltaTime;
			}
			else
			{
				yywx.yy.yinAmt += TotalRegenSpeed * Time.deltaTime;
				yywx.yy.yangAmt -= TotalRegenSpeed * Time.deltaTime;
			}
		}
		else if (regenOn)
		{
			regenOn = false;
		}
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
		//&&&&&&&&&&&&&&&&&&&&&&&&&&이후 필히 수정 필요&&&&&&&&&&&&&&&&&
		switch (to)
		{
			case WXInfo.Wood:
				GameManager.instance.SetCamVFov(55 + 30 * (Mathf.Min(yywx.wx[((int)to)], 90) / 90));
				GetActor().sight.sightRange = 5 + 2.27f * (Mathf.Min(yywx.wx[((int)to)], 90) / 90);
				break;
			case WXInfo.Fire:
				GetActor().move.speedMod = 1 + 0.75f * (Mathf.Min(yywx.wx[((int)to)], 90) / 90);
				GetActor().atk.prepMod = 1 + 1 * (Mathf.Min(yywx.wx[((int)to)], 90) / 90);
				break;
			case WXInfo.Earth:
				regenMod = 1 + 4 * (Mathf.Min(yywx.wx[((int)to)], 90) / 90);
				break;
			case WXInfo.Metal:
				GetActor().atk.effSpeedMod = 1 + 1 * (Mathf.Min(yywx.wx[((int)to)], 90) / 90);
				break;
			case WXInfo.Water:
				GetActor().cast.castMod = 1 + 1 * (Mathf.Min(yywx.wx[((int)to)], 90) / 90);
				break;
		}


		if (!appliedDebuff.Contains(eff) && yywx.wx[((int)to)] > limitation[((int)to)])
		{
			ApplyStatus(eff);
		}
		else if(appliedDebuff.Contains(eff) && yywx.wx[((int)to)] < limitation[((int)to)])
		{
			EndStaus(eff);
		}
	}

	void ApplyStatus(StatusEffect eff)
	{
		appliedDebuff.Add(eff);
		eff.onApplied.Invoke(GetActor(), GetActor());
		GetActor().updateActs += eff.onUpdated;
	}

	void EndStaus(StatusEffect eff)
	{
		appliedDebuff.Remove(eff);
		eff.onEnded.Invoke(GetActor());
		GetActor().updateActs -= eff.onUpdated;
	}

	public virtual void AddYYWXBase(YinyangWuXing data)
	{
		AddYY(data.yy.yinAmt, YYInfo.Yin);
		AddYY(data.yy.yangAmt, YYInfo.Yang);

		AddWX(data.wx.woodAmt, WXInfo.Wood);
		AddWX(data.wx.fireAmt, WXInfo.Fire);
		AddWX(data.wx.earthAmt, WXInfo.Earth);
		AddWX(data.wx.metalAmt, WXInfo.Metal);
		AddWX(data.wx.waterAmt, WXInfo.Water);
	}

	public virtual void AddYYWX(YinyangWuXing data, bool isNegatable = false)
	{
		if(!(isNegatable && isImmune))
		{
			AddYYWXBase(data);
			GetActor().anim.SetHitTrigger();
			StartCoroutine(DelImmuner(IMMUNETIME));
		}
	}

	public virtual void AddYYWX(YinyangWuXing data, float spd, bool isNegatable = false)
	{
		if(!(isNegatable && isImmune))
		{ 
			StartCoroutine(DelAddYYWX(data, (spd * TotalApplySpeed)));
			GetActor().anim.SetHitTrigger();
			StartCoroutine(DelImmuner(IMMUNETIME));
		}
	}


	IEnumerator DelAddYYWX(YinyangWuXing data, float spd)
	{
		float curT = 0;
		YinyangWuXing incPerSec = YinyangWuXing.Zero;
		incPerSec.yy.yinAmt = data.yy.yinAmt / spd;
		incPerSec.yy.yangAmt = data.yy.yangAmt / spd;

		incPerSec.wx.woodAmt = data.wx.woodAmt / spd;
		incPerSec.wx.fireAmt = data.wx.fireAmt / spd;
		incPerSec.wx.earthAmt = data.wx.earthAmt / spd;
		incPerSec.wx.metalAmt = data.wx.metalAmt / spd;
		incPerSec.wx.waterAmt = data.wx.waterAmt / spd;
		while (curT < spd)
		{
			curT += Time.deltaTime;
			yield return null;
			AddYYWXBase(incPerSec * Time.deltaTime);
		}
	}

	IEnumerator DelImmuner(float dur)
	{
		ApplyStatus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)StatEffID.Immune)]);
		yield return new WaitForSeconds(dur);
		EndStaus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)StatEffID.Immune)]);
	}

	public virtual void OnDead()
	{
		StopAllCoroutines();
		GetActor().anim.SetDieTrigger();
		//PoolManager.ReturnObject(gameObject);
	}
}
