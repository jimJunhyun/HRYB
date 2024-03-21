using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

public enum StatEffID
{
	Knockback,
	Immune,
	Blind,
	Slow,
	Bind,

	EnhanceIce,
	EnhanceFire,
	Stun,

	FoxBewitched,
	SpeedUp,

	Bleeding,

	Max
}

public enum StatEffApplyMethod
{
	NoOverwrite,
	Overwrite,
	AddDuration,
	AddPower,
	AddDurationAndPower,
	Stackable,

}

public struct StatusEffect
{
    public string name;
    public string desc;

    public Action<Actor, Actor, float> onApplied;//당한놈, 건놈
    public Action<Actor, float> onUpdated;
    public Action<Actor, float> onEnded;

	public StatEffApplyMethod method;

    public StatusEffect(string n, string d, StatEffApplyMethod mtd, Action<Actor, Actor, float> app, Action<Actor, float> upd, Action<Actor, float> end)
	{
        name = n;
        desc = d;
		method = mtd;
        onApplied = app;
        onUpdated = upd;
        onEnded = end;
	}

	public override bool Equals(object obj)
	{
		return obj is StatusEffect effect &&
			name == effect.name;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(name);
	}
}


public class StatusEffects
{

	public StatVfxDictionary effDict;
	public Hashtable idStatEffPairs = new Hashtable();

	Action<Actor, Compose> enhanceIceAction;

	//Compose lastEnhancedSkill = null;

	public StatusEffects()
	{
		idStatEffPairs.Add(((int)StatEffID.Knockback), new StatusEffect("밀려남", "강력한 힘에 밀려납니다.",  StatEffApplyMethod.NoOverwrite, OnKnockbackActivated, OnKnockbackDebuffUpdated, OnKnockbackDebuffEnded));
		idStatEffPairs.Add(((int)StatEffID.Immune), new StatusEffect("무적", "세계의 비호를 받고 있습니다.", StatEffApplyMethod.NoOverwrite, OnImmuneActivated, OnImmuneUpdated, OnImmuneEnded));
		idStatEffPairs.Add(((int)StatEffID.Blind), new StatusEffect("실명", "눈 앞이 어두워집니다.", StatEffApplyMethod.NoOverwrite, OnBlindActivated, OnBlindUpdated, OnBlindEnded));
		idStatEffPairs.Add(((int)StatEffID.Slow), new StatusEffect("둔화", "움직임이 느려집니다.", StatEffApplyMethod.Stackable, OnSlowActivated, OnSlowUpdated, OnSlowEnded));
		idStatEffPairs.Add(((int)StatEffID.Bind), new StatusEffect("속박", "발이 묶여 이동할 수 없습니다.", StatEffApplyMethod.AddDuration, OnBindActivated, OnBindUpdated, OnBindEnded));
		idStatEffPairs.Add(((int)StatEffID.EnhanceIce), new StatusEffect("냉기", "다음 공격에 얼음의 힘을 부여합니다.", StatEffApplyMethod.NoOverwrite, OnEnhanceIceActivated, OnEnhanceIceUpdated, OnEnhanceIceEnded));
		idStatEffPairs.Add(((int)StatEffID.EnhanceFire), new StatusEffect("화상", "불로 인해 피해를 입습니다.", StatEffApplyMethod.NoOverwrite, OnEnhanceFireActivated, OnEnhanceFireUpdated, OnEnhanceFireEnded));
		idStatEffPairs.Add(((int)StatEffID.Stun), new StatusEffect("기절", "행동할 수 없습니다.", StatEffApplyMethod.AddDuration, OnStunActivated, OnStunUpdated, OnStunEnded));
		idStatEffPairs.Add(((int)StatEffID.FoxBewitched), new StatusEffect("여우홀림", "피해를 받으면, 여우를 빠르게 합니다.", StatEffApplyMethod.NoOverwrite, OnFoxBewitchedActivated, OnFoxBewitchedUpdated, OnFoxBewitchedEnded));
		idStatEffPairs.Add(((int)StatEffID.SpeedUp), new StatusEffect("신속", "움직임이 날래집니다.", StatEffApplyMethod.Stackable, OnSpeedUpActivated, OnSpeedUpUpdated, OnSpeedUpEnded));
		idStatEffPairs.Add(((int)StatEffID.Bleeding), new StatusEffect("출혈", "피가 누출됩니다.", StatEffApplyMethod.Stackable, OnBleedingActivated, OnBleedingUpdated, OnBleedingEnded));

		idStatEffPairs.Add(new StatusEffect("밀려남", "강력한 힘에 밀려납니다.", StatEffApplyMethod.NoOverwrite, OnKnockbackActivated, OnKnockbackDebuffUpdated, OnKnockbackDebuffEnded), ((int)StatEffID.Knockback));
		idStatEffPairs.Add(new StatusEffect("무적", "세계의 비호를 받고 있습니다.", StatEffApplyMethod.NoOverwrite, OnImmuneActivated, OnImmuneUpdated, OnImmuneEnded), ((int)StatEffID.Immune));
		idStatEffPairs.Add(new StatusEffect("실명", "눈 앞이 어두워집니다.", StatEffApplyMethod.NoOverwrite, OnBlindActivated, OnBlindUpdated, OnBlindEnded),((int)StatEffID.Blind));
		idStatEffPairs.Add(new StatusEffect("둔화", "움직임이 느려집니다.", StatEffApplyMethod.Stackable, OnSlowActivated, OnSlowUpdated, OnSlowEnded), ((int)StatEffID.Slow));
		idStatEffPairs.Add(new StatusEffect("속박", "발이 묶여 이동할 수 없습니다.", StatEffApplyMethod.AddDuration, OnBindActivated, OnBindUpdated, OnBindEnded), ((int)StatEffID.Bind));
		idStatEffPairs.Add(new StatusEffect("냉기", "다음 공격에 얼음의 힘을 부여합니다.", StatEffApplyMethod.NoOverwrite, OnEnhanceIceActivated, OnEnhanceIceUpdated, OnEnhanceIceEnded), ((int)StatEffID.EnhanceIce));
		idStatEffPairs.Add(new StatusEffect("화상", "불로 인해 피해를 입습니다.", StatEffApplyMethod.NoOverwrite, OnEnhanceFireActivated, OnEnhanceFireUpdated, OnEnhanceFireEnded), ((int)StatEffID.EnhanceFire));
		idStatEffPairs.Add(new StatusEffect("기절", "행동할 수 없습니다.", StatEffApplyMethod.AddDuration, OnStunActivated, OnStunUpdated, OnStunEnded), ((int)StatEffID.Stun));
		idStatEffPairs.Add(new StatusEffect("여우홀림", "피격시, 플레이어를 빠르게 합니다.", StatEffApplyMethod.NoOverwrite, OnFoxBewitchedActivated, OnFoxBewitchedUpdated, OnFoxBewitchedEnded), ((int)StatEffID.FoxBewitched));
		idStatEffPairs.Add(new StatusEffect("신속", "움직임이 날래집니다.", StatEffApplyMethod.Stackable, OnSpeedUpActivated, OnSpeedUpUpdated, OnSpeedUpEnded), ((int)StatEffID.SpeedUp));
		idStatEffPairs.Add(new StatusEffect("출혈", "피가 누출됩니다.", StatEffApplyMethod.Stackable, OnBleedingActivated, OnBleedingUpdated, OnBleedingEnded), ((int)StatEffID.Bleeding));


		effDict = Resources.Load<StatVfxDictionary>("StatEffList");
	}

	void OnKnockbackActivated(Actor self, Actor inflicter, float power)
	{
		Vector3 force = (self.transform.position - inflicter.transform.position);
		force.y = 0;
		self.move.forceDir += force.normalized * power;
		Debug.Log($"{self.name} knockback");
		if(self.anim is PlayerAnim panim)
		{
			GameManager.instance.pinp.DeactivateInput();
		}
	}
	void OnKnockbackDebuffUpdated(Actor self, float power)
	{
		
	}
	void OnKnockbackDebuffEnded(Actor self, float power)
	{
		if (self.anim is PlayerAnim panim)
		{
			GameManager.instance.pinp.ActivateInput();
		}
		Debug.Log("넉백끝");
	}

	void OnImmuneActivated(Actor self, Actor inflicter, float power)
	{
		self.life.isImmune = true;
	}
	void OnImmuneUpdated(Actor self, float power)
	{
		
	}
	void OnImmuneEnded(Actor self, float power)
	{
		self.life.isImmune = false;
	}

	void OnBlindActivated(Actor self, Actor inflicter, float power)
	{
		self.sight.sightRange *= power;
	}
	void OnBlindUpdated(Actor self, float power)
	{

	}
	void OnBlindEnded(Actor self, float power)
	{
		self.sight.sightRange /= power;
	}

	void OnSlowActivated(Actor self, Actor inflicter, float power)
	{
		self.move.moveModuleStat.HandleSpeed(power, ModuleController.SpeedMode.Slow);
	}
	void OnSlowUpdated(Actor self, float power)
	{

	}
	void OnSlowEnded(Actor self, float power)
	{
		self.move.moveModuleStat.HandleSpeed(-power, ModuleController.SpeedMode.Slow);
	}

	void OnBindActivated(Actor self, Actor inflicter, float power)
	{
		self.move.moveModuleStat.Pause(ControlModuleMode.Status, true);
	}
	void OnBindUpdated(Actor self, float power)
	{

	}
	void OnBindEnded(Actor self, float power)
	{
		self.move.moveModuleStat.Pause(ControlModuleMode.Status, false);
	}

	void OnEnhanceIceActivated(Actor self, Actor inflicter, float power)
	{
		Debug.Log("얼음강화 사용됨");
		enhanceIceAction = GetEffectWithLevel(EnhanceIce, (int)power);
		(self.atk as PlayerAttack).onNextUse += ShowUseEnhanceIce;
		(self.atk as PlayerAttack).onNextSkill += enhanceIceAction;
		(self.atk as PlayerAttack).onNextHit += ShowHitEnhanceIce;
		//(self.atk as PlayerAttack).onNextUse += ShowEffect;
	}
	void OnEnhanceIceUpdated(Actor self, float power)
	{
		
	}
	void OnEnhanceIceEnded(Actor self, float power)
	{
		Debug.Log("!!!!!!!!!!!!!");
		(self.atk as PlayerAttack).onNextUse -= ShowUseEnhanceIce;
		(self.atk as PlayerAttack).onNextSkill -= enhanceIceAction;
		(self.atk as PlayerAttack).onNextHit -= ShowHitEnhanceIce;
		enhanceIceAction = null;
		//스킬 부여 효과 지우기?
	}

	void ShowUseEnhanceIce(GameObject obj)
	{
		Debug.Log("시전시?");
		ShowUseEffect(obj, StatEffID.EnhanceIce);
	}
	void ShowHitEnhanceIce(Vector3 pos)
	{
		Debug.Log("타격시?");
		ShowHitEffect(pos, StatEffID.EnhanceIce);
	}
	Action<Actor, Compose> GetEffectWithLevel(Action<Actor, Compose, int> act, int lv)
	{
		Debug.Log("GETTING LEVEL");
		return (a, c) => { act(a, c, lv); };
	}

	void OnEnhanceFireActivated(Actor self, Actor inflicter, float power)
	{
		Debug.Log("화염강화 사용됨");
	}
	void OnEnhanceFireUpdated(Actor self, float power)
	{
	}
	void OnEnhanceFireEnded(Actor self, float power)
	{
		Debug.Log("화상제거됨");
		//스킬 부여 효과 지우기?
	}

	void OnStunActivated(Actor self, Actor inflicter, float power)
	{
		Debug.Log("기젌했다.");
		self.move.moveModuleStat.Pause(ControlModuleMode.Status, true);
		self.atk.attackModuleStat.Pause(ControlModuleMode.Status, true);
		self.cast.SetNoCastState(ControlModuleMode.Status, true);
	}
	void OnStunUpdated(Actor self, float power)
	{

	}
	void OnStunEnded(Actor self, float power)
	{
		Debug.Log("기젌풀했다.");
		self.move.moveModuleStat.Pause(ControlModuleMode.Status, false);
		self.atk.attackModuleStat.Pause(ControlModuleMode.Status, false);
		self.cast.SetNoCastState(ControlModuleMode.Status, false);
	}

	void OnFoxBewitchedActivated(Actor self, Actor inflicter, float power)
	{
		if(inflicter.atk is PlayerAttack atk)
		{
			self.life.onNextDamaged += Bewitched;
		}
	}
	void OnFoxBewitchedUpdated(Actor self, float power)
	{

	}
	void OnFoxBewitchedEnded(Actor self, float power)
	{
		self.life.onNextDamaged -= Bewitched;
	}

	void Bewitched(Actor self, Actor attacker, YinYang dmg)
	{
		if(attacker.move is PlayerMove)
		{
			StatusEffects.ApplyStat(attacker, attacker, StatEffID.SpeedUp, 3, 0.1f);
			GameManager.instance.foxfire.Accumulate(dmg);
		}
	}

	void OnSpeedUpActivated(Actor self, Actor inflicter, float power)
	{
		self.move.moveModuleStat.HandleSpeed(-power, ModuleController.SpeedMode.Slow);

	}
	void OnSpeedUpUpdated(Actor self, float power)
	{

	}
	void OnSpeedUpEnded(Actor self, float power)
	{
		self.move.moveModuleStat.HandleSpeed(power, ModuleController.SpeedMode.Slow);
	}

	void OnBleedingActivated(Actor self, Actor inflicter, float power)
	{
		self.life.DamageYY(0, self.life.yy.white * 0.04f, DamageType.DotDamage, -1, 1, null, DamageChannel.Bleeding);
	}
	void OnBleedingUpdated(Actor self, float power)
	{

	}
	void OnBleedingEnded(Actor self, float power)
	{
		self.life.StopDamagingFor(DamageChannel.Bleeding, 1);
	}


	void ShowUseEffect(GameObject effShower, StatEffID stat)
	{
		
		List<EffPosStrPair> objs = effDict.data[stat];
		Transform trailPos, whirlPos;
		//int iId = effShower.transform.GetInstanceID();
		//Debug.Log(effShower.transform.name + " id : " +effShower.transform.GetHashCode());
		//InstanceId 의 경우, 유니티 재시작마다 변경. 빌드후의 안정성을 보장하기 힘듬.
		trailPos = GameObject.Find($"{effShower.name}_TrailPos")?.transform;
		whirlPos = GameObject.Find($"{effShower.name}_WhirlPos")?.transform;
		for (int i = 0; i < objs.Count; i++)
		{
			switch (objs[i].effPos)
			{
				case EffectPoses.Trail:
					if(trailPos != null)
					{
						GameObject res= PoolManager.GetObject(objs[i].effPrefName, trailPos);
						Debug.Log(" 트레일생성 " + (res != null) + " " + res.transform.GetInstanceID());
					}
					break;
				case EffectPoses.Whirl:
					if(whirlPos != null)
					{
						PoolManager.GetObject(objs[i].effPrefName, whirlPos);
					}
					break;
				case EffectPoses.Hit:
				default:
					break;
			}
		}
	}

	void ShowHitEffect(Vector3 pos, StatEffID stat)
	{
		List<EffPosStrPair> objs = effDict.data[stat];
		//int iId = effShower.transform.GetInstanceID();
		//Debug.Log(effShower.transform.name + " id : " +effShower.transform.GetHashCode());
		//InstanceId 의 경우, 유니티 재시작마다 변경. 빌드후의 안정성을 보장하기 힘듬.
		for (int i = 0; i < objs.Count; i++)
		{
			switch (objs[i].effPos)
			{
				case EffectPoses.Hit:
					PoolManager.GetObject(objs[i].effPrefName, pos, Quaternion.identity);
					break;
				case EffectPoses.Trail:
				case EffectPoses.Whirl:
				default:
					break;
			}
		}
	}

	void EnhanceIce(Actor self, Compose skInfo, int power)
	{
		if(skInfo.tags.ContainsTag(SkillTags.Enhancable, SkillTags.Attack))
		{
			Debug.Log(skInfo.name + " 얼음공격으로 강화");
			if((skInfo is Leaf atk))
			{
				if(power >= 6)
				{
					atk.statEff.Add(new StatusEffectApplyData(StatEffID.Bind, 0, 5));
				}
				else
				{
					atk.statEff.Add(new StatusEffectApplyData(StatEffID.Slow, (10 + 5 * (power * (power - 1) / 2)) * 0.01f, 5)); //더하긴 했는데, 언제 지우지?
					//lastEnhancedSkill = atk;
				}
				Debug.Log("REMOVING STAT : " + self.life.name);
				if(self.atk is PlayerAttack pa)
				{
					pa.AddRemoveCall(StatEffID.EnhanceIce);
				}
			}
		}
	}

	void EnhanceFire(Actor self, Compose skInfo, int power)
	{
		Debug.Log("화염공격 : " + skInfo.tags.ToString());
		if (skInfo.tags.ContainsTag(SkillTags.Enhancable))
		{
			Debug.Log("화염공격으로 강화디ㅗㅁ.");
			if ((skInfo is AttackBase atk))
			{
				
				//lastEnhancedSkING STAT : " + self.life.name);
				self.life.RemoveAllStatEff(StatEffID.EnhanceFire);
			}
		}
	}

	public static void ApplyStat(Actor to, Actor by, StatEffID id, float dur, float pow = 1)
	{
		GameManager.instance.StartCoroutine(DelApplier(to, by, id, dur, pow));
	}

	static IEnumerator DelApplier(Actor to, Actor by, StatEffID id, float dur, float power)
	{
		if(id == StatEffID.Knockback)
		{
			power /= dur;
			power *= GameManager.instance.forceResistance;
		}
		Action<Actor> updateAct = to.life.ApplyStatus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)], by, power, dur, out string guid);
		Debug.Log($"{updateAct != null} && {guid != null} => {updateAct != null && guid != null}");
		if (updateAct != null && guid != null)
		{
			List<GameObject> effs = new List<GameObject>();
			for (int i = 0; i < GameManager.instance.statEff.effDict.data[id].Count; i++)
			{
				if(GameManager.instance.statEff.effDict.data[id][i].effPos == EffectPoses.Applied)
				{
					GameObject eff = PoolManager.GetObject(GameManager.instance.statEff.effDict.data[id][i].effPrefName, to.transform);
					if (eff)
					{
						VisualEffect vfx = eff.GetComponent<VisualEffect>();
						vfx.Reinit();
						vfx.Play();
						effs.Add(eff);
					}
				}
			}
			
			float t = 0;
			while(to.life.appliedDebuff[guid].dur < 0)
			{
				//Debug.Log( id + " : INFITITE : " + to.life.appliedDebuff[guid].dur);
				yield return null;
			}
			while(t < to.life.appliedDebuff[guid].dur)
			{
				//Debug.Log(id + " : TIMEPASSING");
				yield return null;
				t += Time.deltaTime;
			}
			to.life.EndStaus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)], updateAct, power);
			if (effs.Count > 0)
			{
				for (int i = 0; i < effs.Count; i++)
				{
					PoolManager.ReturnObject(effs[i]);
				}
			}
		}
		else
		{
			
			yield return null;
		}
	}

}
