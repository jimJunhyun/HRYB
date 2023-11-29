using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifeModule : Module
{
	public const float IMMUNETIME = 0.3f;

	public bool isImmune = false;

	public YinYang initYinYang;
	public YinYang initAdequity;
	public float initSoul;

	[HideInInspector]
	public YinYang yy;

	protected YinYang adequity;

	[HideInInspector]
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
		get => yy.yinAmt * 2 <= yy.yangAmt || yy.yangAmt * 2 <=yy.yinAmt || yy.yangAmt + yy.yinAmt > maxSoul;
	}

	protected bool regenOn = false;
	float diff;


	public virtual void Update()
	{
		if(Mathf.Abs((diff = yy.yinAmt - yy.yangAmt)) > regenThreshold)
		{
			regenOn = true;
			if(diff > 0)
			{
				yy.yinAmt -= TotalRegenSpeed * Time.deltaTime;
				yy.yangAmt += TotalRegenSpeed * Time.deltaTime;
			}
			else
			{
				yy.yinAmt += TotalRegenSpeed * Time.deltaTime;
				yy.yangAmt -= TotalRegenSpeed * Time.deltaTime;
			}
		}
		else if (regenOn)
		{
			regenOn = false;
		}
	}

	public void AddYY(float amt, YYInfo to)
	{
		yy[((int)to)] += amt * adequity[((int)to)];
		if (isDead)
		{
			OnDead();
		}
	}

	public void ApplyStatus(StatusEffect eff)
	{
		appliedDebuff.Add(eff);
		eff.onApplied.Invoke(GetActor(), GetActor());
		GetActor().updateActs += eff.onUpdated;
	}

	public void EndStaus(StatusEffect eff)
	{
		appliedDebuff.Remove(eff);
		eff.onEnded.Invoke(GetActor());
		GetActor().updateActs -= eff.onUpdated;
	}

	public virtual void AddYYBase(YinYang data)
	{
		AddYY(data.yinAmt, YYInfo.Yin);
		AddYY(data.yangAmt, YYInfo.Yang);
	}

	public virtual void AddYY(YinYang data, bool isNegatable = false, bool hit = true)
	{
		if(!(isNegatable && isImmune))
		{
			AddYYBase(data);
			GetActor().anim.SetHitTrigger();
			StatusEffects.ApplyStat(GetActor(), StatEffID.Immune, IMMUNETIME);
		}
	}

	public virtual void AddYY(YinYang data, float spd, bool isNegatable = false, bool hit = false)
	{
		if(!(isNegatable && isImmune))
		{ 
			StartCoroutine(DelAddYYWX(data, (spd * TotalApplySpeed)));
			GetActor().anim.SetHitTrigger();
			StatusEffects.ApplyStat(GetActor(), StatEffID.Immune, IMMUNETIME);
		}
	}


	IEnumerator DelAddYYWX(YinYang data, float spd)
	{
		float curT = 0;
		YinYang incPerSec = YinYang.Zero;
		incPerSec.yinAmt = data.yinAmt / spd;
		incPerSec.yangAmt = data.yangAmt / spd;
		while (curT < spd)
		{
			curT += Time.deltaTime;
			yield return null;
			AddYYBase(incPerSec * Time.deltaTime);
		}
	}

	public virtual void OnDead()
	{
		StopAllCoroutines();
		GetActor().anim.SetDieTrigger();
		//PoolManager.ReturnObject(gameObject);
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		yy = new YinYang(initYinYang);
		adequity = initAdequity;
		maxSoul = initSoul;
		regenMod = 1;
		regenOn = false;
		regenThreshold =10;
		baseRegen = 1;
		isImmune = false;
		applyMod = 1;
		baseApplySpeed = 1;
		fixedRegenMod = null;


		GameManager.instance.uiManager.yinYangUI.RefreshValues();
	}
}
