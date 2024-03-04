using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum DamageType
{
	DirectHit, //일반적 공격
	DotDamage, //지속시간동안 매 틱마다 지정된 피해
	NoEvadeHit, //필중공격, 회피 불가
	Continuous, //지속시간동안 지정된 만큼 변함. 매 틱마다 적용
}

public class LifeModule : Module
{
	public const float IMMUNETIME = 0.3f;

	public bool isImmune = false;

	public YinYang initYinYang;
	public YinYang initAdequity;
	//public float initSoul;

	//[HideInInspector]
	public YinYang yy;

	protected YinYang adequity;

	//[HideInInspector]
	//public float maxSoul;

	public YinYang regenMod = YinYang.One;
	public YinYang fixedRegenMod = null;
	float baseRegen = 1f;
	public YinYang TotalRegenSpeed { get => (fixedRegenMod == null ? regenMod : fixedRegenMod) * baseRegen;}

	public float regenThreshold = 10f;

	public float applyMod = 1f;
	float baseApplySpeed = 1f;

	public float TotalApplySpeed { get => baseApplySpeed * applyMod;}

	public Action _dieEvent;
	public Action _hitEvent;

	internal Dictionary<StatusEffect, float> appliedDebuff = new Dictionary<StatusEffect, float>();

	public virtual bool isDead
	{
		get => yy.yangAmt <= 0;
	}

	protected bool regenOn = true;
	float diff;

	public virtual void Awake()
	{
		//maxSoul = initSoul;
	}

	public virtual void Update()
	{
		if (regenOn)
		{
			if(Mathf.Abs((diff = initYinYang.yangAmt - yy.yangAmt)) > regenThreshold)
			{
				regenOn = true;
				yy.yangAmt += TotalRegenSpeed.yangAmt * Time.deltaTime;
			}
			if(Mathf.Abs((diff = initYinYang.yinAmt - yy.yinAmt)) > regenThreshold)
			{
				regenOn = true;
				yy.yinAmt += TotalRegenSpeed.yinAmt * Time.deltaTime;
			}
		}
		//foreach (var item in appliedDebuff)
		//{
		//	Debug.Log(name + " Effefct : " + item.Key.name + " For " + item.Value);
		//}
	}

	void DecreaseYY(float amt, YYInfo to)
	{
		yy[((int)to)] -= amt * adequity[((int)to)];
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

	protected virtual void DamageYYBase(YinYang data)
	{
		DecreaseYY(data.yinAmt, YYInfo.Yin);
		DecreaseYY(data.yangAmt, YYInfo.Yang);
	}

	public virtual void DamageYY(float yin, float yang, DamageType type, float dur = 0, float tick = 0)
	{
		YinYang data = new YinYang(yin, yang);
		switch (type)
		{
			case DamageType.DirectHit:
				if (!(isImmune))
				{
					DamageYYBase(data);
					GetActor().anim.SetHitTrigger();
					StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
				}
				break;
			case DamageType.DotDamage:
			case DamageType.Continuous:
				StartCoroutine(DelDmgYYWX(data, dur, tick, type));
				break;
			case DamageType.NoEvadeHit:
				DamageYYBase(data);
				GetActor().anim.SetHitTrigger();
				StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
				break;
			default:
				break;
		}

	}

	public virtual void DamageYY(YinYang data, DamageType type, float dur = 0, float tick = 0)
	{
		switch (type)
		{
			case DamageType.DirectHit:
				if (!(isImmune))
				{
					DamageYYBase(data);
					GetActor().anim.SetHitTrigger();
					StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
					_hitEvent?.Invoke();
				}
				break;
			case DamageType.DotDamage:
			case DamageType.Continuous:
				StartCoroutine(DelDmgYYWX(data, dur, tick, type));
				break;
			case DamageType.NoEvadeHit:
				DamageYYBase(data);
				GetActor().anim.SetHitTrigger();
				StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
				_hitEvent?.Invoke();
				break;
			default:
				break;
		}
		
	}


	IEnumerator DelDmgYYWX(YinYang data, float dur, float tick, DamageType type)
	{
		float curT = 0;
		WaitForSeconds w = new WaitForSeconds(tick);
		switch (type)
		{
			case DamageType.DotDamage:
				while (curT < dur)
				{
					curT += Time.deltaTime;
					yield return w;
					DamageYYBase(data);
				}
				break;
			case DamageType.Continuous:
				YinYang incPerSec = YinYang.Zero;
				incPerSec.yinAmt = data.yinAmt / dur;
				incPerSec.yangAmt = data.yangAmt / dur;
				while (curT < dur)
				{
					curT += Time.deltaTime;
					yield return w;
					DamageYYBase(incPerSec * Time.deltaTime);
				}
				break;
			default:
				break;
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
		//maxSoul = initSoul;
		regenMod = YinYang.One;
		regenOn = true;
		regenThreshold =10;
		baseRegen = 1;
		isImmune = false;
		applyMod = 1;
		baseApplySpeed = 1;
		fixedRegenMod = null;


		GameManager.instance.uiManager.yinYangUI.RefreshValues();
	}
}
