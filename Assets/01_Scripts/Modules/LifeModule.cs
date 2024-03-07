using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum DamageType
{
	DirectHit, //일반적 공격
	DotDamage, //지속시간동안 매 틱마다 지정된 피해
	NoEvadeHit, //필중공격, 회피 불가
	Continuous, //지속시간동안 지정된 만큼 변함. 매 틱마다 적용
}

public struct AppliedStatus
{
	public StatusEffect eff;
	public float dur;

	public AppliedStatus(StatusEffect e, float d)
	{
		eff = e;
		dur = d;
	}

	public override bool Equals(object obj)
	{
		return obj is AppliedStatus status &&
			   eff.Equals(status.eff);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(eff);
	}
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

	
	internal List<AppliedStatus> appliedDebuff = new List<AppliedStatus>();

	public virtual bool isDead
	{
		get => yy.white <= 0;
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
			if(Mathf.Abs((diff = initYinYang.white - yy.white)) > regenThreshold)
			{
				regenOn = true;
				yy.white += TotalRegenSpeed.white * Time.deltaTime;
			}
			if(Mathf.Abs((diff = initYinYang.black - yy.black)) > regenThreshold)
			{
				regenOn = true;
				yy.black += TotalRegenSpeed.black * Time.deltaTime;
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

	public Action<Actor> ApplyStatus(StatusEffect eff, Actor applier, float power, float dur, out int effIndex)
	{
		int idx = appliedDebuff.FindIndex(item => item.eff.Equals(eff));

		if(eff.method == StatEffApplyMethod.Overwrite)
		{
			RemoveStatEff(idx);
		}
		if (idx == -1 || eff.method == StatEffApplyMethod.Stackable || eff.method == StatEffApplyMethod.Overwrite)
		{
			appliedDebuff.Add( new AppliedStatus(eff, dur));
			eff.onApplied.Invoke(GetActor(), applier, power);
			Action<Actor> updAct = (self) => { eff.onUpdated(self, power);};
			GetActor().updateActs += updAct;
			effIndex = idx;

			return updAct;
		}
		else
		{
			switch (eff.method)
			{
				case StatEffApplyMethod.AddDuration:
					{
						AppliedStatus stat = appliedDebuff[idx];
						stat.dur += dur;
						appliedDebuff[idx] = stat;
					}
					break;
				case StatEffApplyMethod.AddPower:
					{
						//?????????????
					}
					break;
				case StatEffApplyMethod.AddDurationAndPower:
					{
						AppliedStatus stat = appliedDebuff[idx];
						stat.dur += dur;
						//?????????????
						appliedDebuff[idx] = stat;
					}
					break;
				case StatEffApplyMethod.Overwrite:
				case StatEffApplyMethod.Stackable:
				case StatEffApplyMethod.NoOverwrite:
				default:
					break;
			}
			

			effIndex = -1;
			return null;
		}
	}

	public void EndStaus(StatusEffect eff, Action<Actor> myUpdateAct, float power)
	{

		Debug.Log($"update act count : {GetActor().updateActs.GetInvocationList().Length}");
		int idx = appliedDebuff.FindIndex(item=>item.eff.Equals(eff));
		if (idx != -1 && appliedDebuff.Remove(appliedDebuff[idx]))
		{
			Debug.Log($"{eff.name}사라짐");
			GetActor().updateActs -= myUpdateAct;

			eff.onEnded.Invoke(GetActor(), power);
		}

		
	}

	public void RemoveStatEff(int idx)
	{
		Debug.Log(name + " EffectCount : " + appliedDebuff.Count);
		Debug.Log("스탯제거중...");
		AppliedStatus stat = appliedDebuff[idx];
		stat.dur = 0;
		appliedDebuff[idx] = stat;
	}

	public void RemoveAllStatEff(StatEffID id)
	{
		for (int i = 0; i < appliedDebuff.Count; i++)
		{
			if (((StatusEffect)GameManager.instance.statEff.idStatEffPairs[id]).Equals(appliedDebuff[i].eff))
			{
				AppliedStatus stat = appliedDebuff[i];
				stat.dur = 0;
				appliedDebuff[i] = stat;
			}
		}
	}

	protected virtual void DamageYYBase(YinYang data)
	{
		DecreaseYY(data.black, YYInfo.Black);
		DecreaseYY(data.white, YYInfo.White);
	}

	public virtual void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0)
	{
		YinYang data = new YinYang(black, white);
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
				incPerSec.black = data.black / dur;
				incPerSec.white = data.white / dur;
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
		StopAllCoroutines();
		GetActor().anim.SetDieTrigger();
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
