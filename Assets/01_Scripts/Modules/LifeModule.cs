using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class LifeModule : Module
{
	public const float IMMUNETIME = 0.3f;

	public bool isImmune = false;

	public YinYang initYinYang;
	public YinYang initAdequity;
	public float initSoul;

	//[HideInInspector]
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

	public Action _dieEvent;
	public Action _hitEvent;

	internal Dictionary<StatusEffect, float> appliedDebuff = new Dictionary<StatusEffect, float>();

	public virtual bool isDead
	{
		get => yy.yinAmt * 2 <= yy.yangAmt || yy.yangAmt * 2 <=yy.yinAmt || yy.yangAmt + yy.yinAmt > maxSoul;
	}

	protected bool regenOn = false;
	float diff;

	public virtual void Awake()
	{
		maxSoul = initSoul;
	}

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
		//foreach (var item in appliedDebuff)
		//{
		//	Debug.Log(name + " Effefct : " + item.Key.name + " For " + item.Value);
		//}
	}

	public void AddYY(float amt, YYInfo to)
	{
		yy[((int)to)] += amt * adequity[((int)to)];
		if (isDead)
		{
			OnDead();
		}
	}

	public Action<Actor> ApplyStatus(StatusEffect eff, Actor applier, float power, float dur)
	{
		if (!appliedDebuff.ContainsKey(eff))
		{
			appliedDebuff.Add(eff, dur);
			eff.onApplied.Invoke(GetActor(), applier, power);
			Action<Actor> updAct = (self) => { eff.onUpdated(self, power);};
			GetActor().updateActs += updAct;
			return updAct;
		}
		else
		{
			appliedDebuff[eff] += dur;
			return null;
		}
	}

	public void EndStaus(StatusEffect eff, Action<Actor> myUpdateAct, float power)
	{

		Debug.Log($"update act count : {GetActor().updateActs.GetInvocationList().Length}");

		if (appliedDebuff.Remove(eff))
		{
			Debug.Log($"{eff.name}사라짐");
			GetActor().updateActs -= myUpdateAct;

			eff.onEnded.Invoke(GetActor(), power);
		}

		
	}

	public void RemoveStatEff(StatEffID id)
	{
		Debug.Log(name + " EffectCount : " + appliedDebuff.Count);
		foreach (var item in appliedDebuff)
		{
			Debug.Log("스탯제거중...");
			if(id == (StatEffID)GameManager.instance.statEff.idStatEffPairs[item.Key])
			{
				Debug.Log("발견, 지속시간 0으로//");
				appliedDebuff.Remove(item.Key);
				appliedDebuff.Add(item.Key, 0);
				break;
			}
		}
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
			StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
		}

		if(hit== true)
		{
			_hitEvent?.Invoke();
		}

	}

	public virtual void AddYY(YinYang data, float spd, bool isNegatable = false, bool hit = false)
	{
		if(!(isNegatable && isImmune))
		{ 
			StartCoroutine(DelAddYYWX(data, (spd * TotalApplySpeed)));
			GetActor().anim.SetHitTrigger();
			StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
		}

		if (hit == true)
		{
			_hitEvent?.Invoke();
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
		Debug.LogError($"{gameObject.name} : 사망");
		StopAllCoroutines();
		GetActor().anim.SetDieTrigger();
		_dieEvent?.Invoke();
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
