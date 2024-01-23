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

	EnhanceIce,
	EnhanceFire,

	Burn,

	Max
}

public struct StatusEffect
{
    public string name;
    public string desc;

    public Action<Actor, Actor, float> onApplied;//당한놈, 건놈
    public Action<Actor, float> onUpdated;
    public Action<Actor, float> onEnded;

    public StatusEffect(string n, string d, Action<Actor, Actor, float> app, Action<Actor, float> upd, Action<Actor, float> end)
	{
        name = n;
        desc = d;
        onApplied = app;
        onUpdated = upd;
        onEnded = end;
	}
}


public class StatusEffects
{

	public StatVfxDictionary effDict;
	public Hashtable idStatEffPairs = new Hashtable();

	public StatusEffects()
	{
		idStatEffPairs.Add(((int)StatEffID.Knockback), new StatusEffect("밀려남", "강력한 힘에 밀려납니다.", OnKnockbackActivated, OnKnockbackDebuffUpdated, OnKnockbackDebuffEnded));
		idStatEffPairs.Add(((int)StatEffID.Immune), new StatusEffect("무적", "어머니의 비호를 받고 있습니다.", OnImmuneActivated, OnImmuneUpdated, OnImmuneEnded));
		idStatEffPairs.Add(((int)StatEffID.Blind), new StatusEffect("실명", "눈 앞이 어두워집니다.", OnBlindActivated, OnBlindUpdated, OnBlindEnded));
		idStatEffPairs.Add(((int)StatEffID.Slow), new StatusEffect("둔화", "움직임이 느려집니다.", OnSlowActivated, OnSlowUpdated, OnSlowEnded));
		idStatEffPairs.Add(((int)StatEffID.EnhanceIce), new StatusEffect("냉기", "다음 공격에 얼음의 힘을 부여합니다.", OnEnhanceIceActivated, OnEnhanceIceUpdated, OnEnhanceIceEnded));

		idStatEffPairs.Add(new StatusEffect("밀려남", "강력한 힘에 밀려납니다.", OnKnockbackActivated, OnKnockbackDebuffUpdated, OnKnockbackDebuffEnded), ((int)StatEffID.Knockback));
		idStatEffPairs.Add(new StatusEffect("무적", "어머니의 비호를 받고 있습니다.", OnImmuneActivated, OnImmuneUpdated, OnImmuneEnded), ((int)StatEffID.Immune));
		idStatEffPairs.Add(new StatusEffect("실명", "눈 앞이 어두워집니다.", OnBlindActivated, OnBlindUpdated, OnBlindEnded),((int)StatEffID.Blind));
		idStatEffPairs.Add(new StatusEffect("둔화", "움직임이 느려집니다.", OnSlowActivated, OnSlowUpdated, OnSlowEnded), ((int)StatEffID.Slow));
		idStatEffPairs.Add(new StatusEffect("냉기", "다음 공격에 얼음의 힘을 부여합니다.", OnEnhanceIceActivated, OnEnhanceIceUpdated, OnEnhanceIceEnded), ((int)StatEffID.EnhanceIce));


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
		self.move.speedMod -= power;
	}
	void OnSlowUpdated(Actor self, float power)
	{

	}
	void OnSlowEnded(Actor self, float power)
	{
		self.move.speedMod += power;
	}

	void OnEnhanceIceActivated(Actor self, Actor inflicter, float power)
	{
		(self.atk as PlayerAttack).onNextHits += EnhanceIce;
	}
	void OnEnhanceIceUpdated(Actor self, float power)
	{
		
	}
	void OnEnhanceIceEnded(Actor self, float power)
	{
		(self.atk as PlayerAttack).onNextHits -= EnhanceIce;
		//스킬 부여 효과 지우기???????????????/
	}

	List<string> ShowEffect(GameObject effShower, Actor self, Actor target, StatEffID stat)
	{
		
		List<EffPosStrPair> objs = effDict.data[stat];
		Transform trailPos, whirlPos;
		List<string> hitEffs = new List<string>();
		trailPos = effShower.transform.Find("TrailPos");
		whirlPos = effShower.transform.Find("WhirlPos");
		for (int i = 0; i < objs.Count; i++)
		{
			switch (objs[i].effPos)
			{
				case EffectPoses.Trail:
					PoolManager.GetObject(objs[i].effPrefName, trailPos);
					break;
				case EffectPoses.Whirl:
					PoolManager.GetObject(objs[i].effPrefName, whirlPos);
					break;
				case EffectPoses.Hit:
					hitEffs.Add(objs[i].effPrefName);
					break;
				default:
					break;
			}
		}
		return hitEffs;
	}

	List<string> EnhanceIce(GameObject effShower, Actor self, Actor target, Compose skInfo)
	{
		Debug.Log("얼음공격 : " + skInfo.tags.ToString());
		if(skInfo.tags.ContainsTag(SkillTags.AttackEnhancable))
		{
			Debug.Log("얼음공격으로 강화디ㅗㅁ.");
			if((skInfo is AttackBase atk))
			{
				atk.statEff.Add(new StatusEffectApplyData(StatEffID.Slow, 15 /*(레벨에 따른 변화)*/, 5));
				self.life.RemoveStatEff(StatEffID.EnhanceIce);

				return ShowEffect(effShower, self, target, StatEffID.EnhanceIce);
			}
		}
		return null;
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
		Action<Actor> updateAct = to.life.ApplyStatus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)], by, power, dur);
		if(updateAct != null)
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
			while(to.life.appliedDebuff[(StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)]] < 0)
				yield return null;
			while(t < to.life.appliedDebuff[(StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)]])
			{
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
