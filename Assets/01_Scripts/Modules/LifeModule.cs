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

public enum DamageChannel
{
	None,
	Normal,
	Bleeding,

}

[Serializable]
public struct AppliedStatus
{
	public StatusEffect eff;
	public float dur;

	public AppliedStatus(StatusEffect e, float d)
	{
		eff = e;
		dur = d;
	}

	public static AppliedStatus Empty
	{
		get => new AppliedStatus(new StatusEffect(), 0);
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

	public Action _dieEvent;
	public Action _hitEvent;
	
	internal Dictionary<string, AppliedStatus> appliedDebuff = new Dictionary<string, AppliedStatus>();

	internal Dictionary<int, List<Coroutine>> ongoingTickDamages = new Dictionary<int, List<Coroutine>>();

	//피격자, 공격자, 대미지
	public Action<Actor, Actor, YinYang> onNextDamaged; 

	public virtual bool isDead
	{
		get => yy.white <= 0;
	}

	public bool superArmor = false;

	protected bool regenOn = true;
	float diff;
	private bool _isFirstHit;

	public bool IsFirstHit => _isFirstHit;

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

	public Action<Actor> ApplyStatus(StatusEffect eff, Actor applier, float power, float dur, out string outGuid)
	{
		bool cont = appliedDebuff.ContainsValue(new AppliedStatus(eff, 0));
		string sameUid = null;
		if (cont)
		{
			foreach (var item in appliedDebuff)
			{
				if (item.Value.eff.Equals(eff))
				{
					sameUid = item.Key;
					break;
				}
			}
		}

		if(cont && eff.method == StatEffApplyMethod.Overwrite)
		{
			RemoveStatEff(sameUid);
		}
		if (!cont || eff.method == StatEffApplyMethod.Stackable || eff.method == StatEffApplyMethod.Overwrite)
		{
			Guid g = Guid.NewGuid();
			outGuid = g.ToString();
			appliedDebuff.Add(outGuid, new AppliedStatus(eff, dur));
			eff.onApplied.Invoke(GetActor(), applier, power);
			Action<Actor> updAct = (self) => { eff.onUpdated(self, power);};
			GetActor().updateActs += updAct;


			return updAct;
		}
		else
		{
			switch (eff.method)
			{
				case StatEffApplyMethod.AddDuration:
					{
						AppliedStatus stat = appliedDebuff[sameUid];
						stat.dur += dur;
						appliedDebuff[sameUid] = stat;
					}
					break;
				case StatEffApplyMethod.AddPower:
					{
						//?????????????
					}
					break;
				case StatEffApplyMethod.AddDurationAndPower:
					{
						AppliedStatus stat = appliedDebuff[sameUid];
						stat.dur += dur;
						//?????????????
						appliedDebuff[sameUid] = stat;
					}
					break;
				case StatEffApplyMethod.Overwrite:
				case StatEffApplyMethod.Stackable:
				case StatEffApplyMethod.NoOverwrite:
				default:
					break;
			}

			outGuid = null;
			return null;
		}
	}

	public void EndStaus(StatusEffect eff, Action<Actor> myUpdateAct, float power)
	{

		Debug.Log($"update act count : {GetActor().updateActs.GetInvocationList().Length}");
		
		if (appliedDebuff.ContainsValue(new AppliedStatus(eff, 0)))
		{
			foreach (var item in appliedDebuff)
			{
				if(item.Value.eff.Equals(eff))
				{
					Debug.Log($"{eff.name}사라짐");
					GetActor().updateActs -= myUpdateAct;
					eff.onEnded.Invoke(GetActor(), power);
					break;
				}


			}
		}

		
	}

	public void RemoveStatEff(string guid)
	{
		Debug.Log(name + " EffectCount : " + appliedDebuff.Count);
		Debug.Log("스d탯제거중...");
		AppliedStatus stat = appliedDebuff[guid];
		stat.dur = 0;
		appliedDebuff[guid] = stat;
	}

	public void RemoveAllStatEff(StatEffID id, int count = -1)
	{
		Dictionary<string, AppliedStatus> debuffCopy = new Dictionary<string, AppliedStatus>();
		foreach (var item in appliedDebuff)
		{
			if (((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)]).Equals(item.Value))
			{
				if(count > 0 || count == -1)
				{
					AppliedStatus stat = item.Value;
					stat.dur = 0;
					debuffCopy[item.Key] = stat;
					Debug.Log("지속시간 0으로 : " + stat.eff.name);
					--count;
				}
				
			}
			else
			{
				debuffCopy[item.Key] = item.Value;
			}
		}
	}

	protected virtual void DamageYYBase(YinYang data)
	{
		DecreaseYY(data.black, YYInfo.Black);
		DecreaseYY(data.white, YYInfo.White);
	}

	public virtual void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel= DamageChannel.None)
	{
		_isFirstHit = true;
		
		YinYang data = new YinYang(black, white);
		switch (type)
		{
			case DamageType.DirectHit:
				if (!(isImmune))
				{
					DamageYYBase(data);
					if (!superArmor)
					{
						GetActor().anim.SetHitTrigger();
					}
					StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
					onNextDamaged?.Invoke(GetActor(), attacker, data);
					if (GetActor()._ai != null)
					{
						GetActor()._ai.StartExamine();
					}
				}
				break;
			case DamageType.DotDamage:
			case DamageType.Continuous:
				ongoingTickDamages[(int)channel].Add(StartCoroutine(DelDmgYYWX(data, dur, tick, type)));
				break;
			case DamageType.NoEvadeHit:
				DamageYYBase(data);
				if (!superArmor)
				{
					GetActor().anim.SetHitTrigger();
				}
				StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
				onNextDamaged?.Invoke(GetActor(), attacker, data);
				break;
			default:
				break;
		}
	}

	public virtual void DamageYY(YinYang data, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{
		_isFirstHit = true;
		
		switch (type)
		{
			case DamageType.DirectHit:
				if (!(isImmune))
				{
					DamageYYBase(data);
					GetActor().anim.SetHitTrigger();
					StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
					onNextDamaged?.Invoke(GetActor(), attacker, data);
					_hitEvent?.Invoke();
					if (GetActor()._ai != null)
					{
						GetActor()._ai.StartExamine();
					}
				}
				break;
			case DamageType.DotDamage:
			case DamageType.Continuous:
				ongoingTickDamages[(int)channel].Add(StartCoroutine(DelDmgYYWX(data, dur, tick, type)));
				break;
			case DamageType.NoEvadeHit:
				DamageYYBase(data);
				GetActor().anim.SetHitTrigger();
				StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
				onNextDamaged?.Invoke(GetActor(), attacker, data);
				_hitEvent?.Invoke();
				break;
			default:
				break;
		}
	}

	public void StopDamagingFor(DamageChannel channel, int amt = 1)
	{
		for (int i = 0; i < amt; i++)
		{
			if(ongoingTickDamages.Count > 1)
			{
				ongoingTickDamages[((int)channel)].RemoveAt(ongoingTickDamages.Count - 1);
			}
			else
				break;
		}
	}

	protected IEnumerator DelDmgYYWX(YinYang data, float dur, float tick, DamageType type)
	{
		float curT = 0;
		WaitForSeconds w = new WaitForSeconds(tick);
		switch (type)
		{
			case DamageType.DotDamage:
				while (curT < dur)
				{
					if(dur > 0)
					{
						curT += Time.deltaTime;
					}
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

	private bool _isOneDie = false;
	
	public virtual void OnDead()
	{
//		Debug.LogError($"{gameObject.name} : 사망");
		if (_isOneDie == false)
		{
			_isOneDie = true;
			StopAllCoroutines();
			GetActor().anim.SetDieTrigger();
			_dieEvent?.Invoke();
			
		}

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
