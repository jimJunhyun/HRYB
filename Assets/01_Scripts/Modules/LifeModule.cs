using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;

public enum DamageType
{
	DirectHit, //일반적 공격
	DotDamage, //지속시간동안 매 틱마다 지정된 피해
	NoEvadeHit, //필중공격, 회피 불가
	Continuous, //지속시간동안 지정된 만큼 변함. 매 틱마다 적용
	NoHit,
}

public enum DamageChannel
{
	None,
	Normal,
	Bleeding,
	Fire,

	Max
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
	public const float IMMUNETIME = 0.0f;

	public bool isImmune = false;

	public float initBlack;
	public float initWhite;
	public YinYang yy;

	public float initAdeBlack;
	public float initAdeWhite;
	public YinYang adequity;
	//public float initSoul;

	//[HideInInspector]
	//public float maxSoul;

	public YinYang regenMod = YinYang.One;
	public YinYang fixedRegenMod = null;
	float baseRegen = 1f;
	public YinYang TotalRegenSpeed { get => (fixedRegenMod == null ? regenMod : fixedRegenMod) * baseRegen;}

	public float regenThreshold = 10f;

	//public float applyMod = 1f;
	//float baseApplySpeed = 1f;

	//public float TotalApplySpeed { get => baseApplySpeed * applyMod;}

	public Action _dieEvent;
	public Action _hitEvent;


	public Dictionary<StatEffID, float> _bufferDurations = new();

	internal Dictionary<string, AppliedStatus> appliedDebuff = new Dictionary<string, AppliedStatus>();

	internal Dictionary<int, List<Coroutine>> ongoingTickDamages = new Dictionary<int, List<Coroutine>>() ;

	//피격자, 공격자, 대미지
	public Action<Actor, Actor, YinYang> onNextDamaged;

	[Range(1, 100)]
	public int power; //기세
	public float powerExpReq;
	public float powerExp;

	public ModuleController	powerExpMod = new ModuleController(false);

	public virtual bool isDead
	{
		get => yy.white.Value <= 0;
	}

	public bool superArmor = false;

	protected bool regenOn = true;
	float diff;
	private bool _isFirstHit;

	public bool IsFirstHit => _isFirstHit;
	protected Coroutine _stopCoroutine;

	public virtual void Awake()
	{
		//maxSoul = initSoul;
		for (int i = ((int)DamageChannel.Normal); i < ((int)DamageChannel.Max); i++)
		{
			ongoingTickDamages.Add(i, new List<Coroutine>());
		}
		_hitEvent += () => { 
			if(_stopCoroutine == null)
			{
				_stopCoroutine = StartCoroutine(PlayWakeAgain(0.2f));  
			}
		};
		yy = new YinYang(initBlack, initWhite);

		adequity = new YinYang(initAdeBlack, initAdeWhite);
		//Debug.Log("INITADE : " + initAdeBlack + " : " + initAdeWhite);
		//Debug.Log("ADE : " + adequity.ToString());
	}
	protected virtual IEnumerator PlayWakeAgain(float t)
	{

		self.AI.StopExamine();
		yield return new WaitForSeconds(t); 
		self.AI.StartExamine();
	}

	public virtual void Update()
	{
		//if (regenOn)
		//{
		//	if(Mathf.Abs((diff = initYinYang.white - yy.white)) > regenThreshold)
		//	{
		//		regenOn = true;
		//		yy.white += TotalRegenSpeed.white * Time.deltaTime;
		//	}
		//	if(Mathf.Abs((diff = initYinYang.black - yy.black)) > regenThreshold)
		//	{
		//		regenOn = true;
		//		yy.black += TotalRegenSpeed.black * Time.deltaTime;
		//	}
		//}
		//foreach (var item in appliedDebuff)
		//{
		//	Debug.Log(name + " Effefct : " + item.Key.name + " For " + item.Value);
		//}

		//if(yy.white.Value > yy.white.MaxValue)
		//{
		//	yy.white = initYinYang.white;
		//}
		//if(yy.black > initYinYang.black)
		//{
		//	yy.black = initYinYang.black;
		//}
	}


	public void GetPowerExp(float amt)
	{
		if (!powerExpMod.Paused)
		{
			powerExp += amt * powerExpMod.Speed;
			if(powerExp >= powerExpReq)
			{
				power += 1;
				powerExp -= powerExpReq;
				powerExpReq = 500 + (500 * (int)(power / 10)); //모종의 식
			}
		}
	}
	protected virtual void DecreaseYY(float amt, YYInfo to, DamageChannel chn = DamageChannel.Normal)
	{
		
		yy.white.Value -= amt * adequity[((int)to)];
		if (isDead)
		{
			OnDead();
			StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, 10);
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
			//foreach (var item in appliedDebuff)
			//{
			//	Debug.Log($"before naturla removal ==> {item.Key} : {item.Value}");
			//}

			Dictionary<string, AppliedStatus> statCopy = new Dictionary<string, AppliedStatus>(appliedDebuff);
			foreach (var item in appliedDebuff)
			{
				if(item.Value.eff.Equals(eff))
				{
					Debug.Log($"{eff.name}사라짐");
					GetActor().updateActs -= myUpdateAct;
					eff.onEnded.Invoke(GetActor(), power);
					statCopy.Remove(item.Key);
					break;
				}

			}
			appliedDebuff = statCopy;

			//foreach (var item in appliedDebuff)
			//{
			//	Debug.Log($"After Natural Removal ==> {item.Key} : {item.Value}");
			//}
		}
		
	}

	public void EndStaus(string guid, Action<Actor> myUpdateAct, float power)
	{
		Debug.Log($"update act count : {GetActor().updateActs.GetInvocationList().Length}");

		if (appliedDebuff.ContainsKey(guid))
		{
			//foreach (var item in appliedDebuff)
			//{
			//	Debug.Log($"before naturla removal ==> {item.Key} : {item.Value}");
			//}

			GetActor().updateActs -= myUpdateAct;
			appliedDebuff[guid].eff.onEnded.Invoke(GetActor(), power);
			appliedDebuff.Remove(guid);

			//foreach (var item in appliedDebuff)
			//{
			//	Debug.Log($"After Natural Removal ==> {item.Key} : {item.Value}");
			//}
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
		Dictionary<string, AppliedStatus> debuffCopy = new Dictionary<string, AppliedStatus>(appliedDebuff);
		//foreach (var item in appliedDebuff)
		//{
		//	Debug.Log($"Before Force Removal ==> {item.Key} : {item.Value}");
		//}

		foreach (var item in appliedDebuff)
		{
			if (((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)]).Equals(item.Value.eff))
			{
				if(count > 0 || count == -1)
				{
					AppliedStatus stat = item.Value;
					stat.dur = 0;
					debuffCopy[item.Key] = stat;
					Debug.Log("지속시간 0으로 : " + stat.eff.name);
					if(count > 0)
						--count;
				}
				
			}
		}
		appliedDebuff = debuffCopy;

		//foreach (var item in appliedDebuff)
		//{
		//	Debug.Log($"After Force Removal ==> {item.Key} : {item.Value}");
		//}
	}

	protected virtual void DamageYYBase(YinYang data, DamageChannel chn = DamageChannel.Normal)
	{
		DecreaseYY(data.black.Value, YYInfo.Black, chn);
		DecreaseYY(data.white.Value, YYInfo.White, chn);
	}

	public virtual void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel= DamageChannel.None)
	{
		DamageYY(new YinYang(black, white), type, dur, tick, attacker, channel);
	}

	public virtual void DamageYY(YinYang data, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{
		_isFirstHit = true;
		if (attacker != null)
		{
			float mod = (Mathf.Max(1, attacker.life.power) / Mathf.Max(1, power));
			data = data * mod;
		}

		switch (type)
		{
			case DamageType.DirectHit:
				if (!(isImmune))
				{
					
					DamageYYBase(data);
					if (!superArmor)
					{
						GetActor().anim.SetHitTrigger();
						_hitEvent?.Invoke();
					}
					StatusEffects.ApplyStat(GetActor(), attacker, StatEffID.Immune, IMMUNETIME);
					onNextDamaged?.Invoke(GetActor(), attacker, data);
				}
				break;
			case DamageType.DotDamage:
			case DamageType.Continuous:
				//
				ongoingTickDamages[((int)channel)].Add(StartCoroutine(DelDmgYYWX(data, dur, tick, type, channel)));
				break;
			case DamageType.NoEvadeHit:
				DamageYYBase(data);
				if (!superArmor)
				{
					GetActor().anim.SetHitTrigger();
				}
				StatusEffects.ApplyStat(GetActor(), attacker, StatEffID.Immune, IMMUNETIME);
				onNextDamaged?.Invoke(GetActor(), attacker, data);
				_hitEvent?.Invoke();
				break;
			case DamageType.NoHit:
				if (!(isImmune))
				{
					DamageYYBase(data);
					StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
					onNextDamaged?.Invoke(GetActor(), attacker, data);
				}
				break;
			default:
				break;
		}
		if (attacker == GameManager.instance.pActor)
		{

			GameManager.instance.recentDamage = data.white.Value;
			GameManager.instance.recentDamageType = type;
			GameManager.instance.recentEnemy = GetActor();
		}
		Debug.Log($"{self.actorName} HP : {yy.white.Value} / {yy.white.MaxValue}");
	}

	public void StopDamagingFor(DamageChannel channel, int amt = 1)
	{
		for (int i = 0; i < amt; i++)
		{
			if(ongoingTickDamages.Count > 1)
			{
				int lastIdx = ongoingTickDamages[((int)channel)].Count - 1;
				if (ongoingTickDamages[((int)channel)][lastIdx] != null)
				{
					StopCoroutine(ongoingTickDamages[((int)channel)][lastIdx]);
				}
				ongoingTickDamages[((int)channel)].RemoveAt(lastIdx);
				Debug.Log("STOPPED : " +channel);
			}
			else
				break;
		}
	}

	protected IEnumerator DelDmgYYWX(YinYang data, float dur, float tick, DamageType type, DamageChannel channel = DamageChannel.None)
	{
		float curT = 0;
		WaitForSeconds w = new WaitForSeconds(tick);
		bool isInf = dur < 0;
		Debug.Log("DAMAGING : " + channel);
		switch (type)
		{
			case DamageType.DotDamage:
				while (curT < dur || dur < 0)
				{
					if(dur > 0)
					{
						curT += Time.deltaTime;
					}
					yield return w;
					DamageYYBase(data, channel);
					Debug.Log("DAMAGED OF" + channel + ": " + data.white);
				}
				break;
			case DamageType.Continuous:
				YinYang incPerSec = new YinYang(data.black.Value / dur, data.white.Value / dur);
				while (curT < dur || dur < 0)
				{
					if(dur >= 0)
					{
						curT += Time.deltaTime;
					}
					yield return w;
					DamageYYBase(incPerSec * Time.deltaTime, channel);
				}
				break;
			default:
				break;
		}
		if (!isInf && channel != DamageChannel.None)
		{
			StopDamagingFor(channel);
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
			//GetActor().anim.SetBoolModify("Die",true);
			GetActor().anim.SetDieTrigger();
			_dieEvent?.Invoke();

			foreach (var item in appliedDebuff)
			{
				RemoveAllStatEff((StatEffID)GameManager.instance.statEff.idStatEffPairs[item.Value.eff]);
			}
				
			GameManager.instance.qManager.InvokeOnChanged(CompletionAct.DefeatTarget, GetActor().actorName);
			
		}

		//PoolManager.ReturnObject(gameObject);
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		yy.black.ResetCompletely();
		yy.white.ResetCompletely();
		adequity.black.ResetCompletely();
		adequity.white.ResetCompletely();
		//maxSoul = initSoul;
		regenMod = YinYang.One;
		regenOn = true;
		regenThreshold =10;
		baseRegen = 1;
		isImmune = false;
		//applyMod = 1;
		//baseApplySpeed = 1;
		fixedRegenMod = null;
		

		GameManager.instance.uiManager.yinYangUI.RefreshValues();
	}

	public virtual void SetAdequity(float t)
	{
		adequity = new YinYang(t, t);
	}
}
