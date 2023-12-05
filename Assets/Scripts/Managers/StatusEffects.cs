using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StatEffID
{
	Knockback,
	Immune,
	Blind,
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
    public Hashtable idStatEffPairs = new Hashtable();

	public StatusEffects()
	{
		idStatEffPairs.Add(((int)StatEffID.Knockback), new StatusEffect("밀려남", "강력한 힘에 밀려납니다.", OnKnockbackActivated, OnKnockbackDebuffUpdated, OnKnockbackDebuffEnded));
		idStatEffPairs.Add(((int)StatEffID.Immune), new StatusEffect("무적", "어머니의 비호를 받고 있습니다.", OnImmuneActivated, OnImmuneUpdated, OnImmuneEnded));
		idStatEffPairs.Add(((int)StatEffID.Blind), new StatusEffect("실명", "눈 앞이 어두워집니다.", OnBlindActivated, OnBlindUpdated, OnBlindEnded));
	}

	void OnKnockbackActivated(Actor self, Actor inflicter, float power)
	{
		self.move.forceDir += (self.transform.position - inflicter.transform.position).normalized * power;
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
		Action<Actor> updateAct = to.life.ApplyStatus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)], by, power);
		if(updateAct != null)
		{
			yield return new WaitForSeconds(dur);
			to.life.EndStaus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)], updateAct, power);
		}
		else
			yield return null;
	}

}
