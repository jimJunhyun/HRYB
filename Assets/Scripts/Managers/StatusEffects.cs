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

    public Action<Actor, Actor> onApplied;//당한놈, 건놈
    public Action<Actor> onUpdated;
    public Action<Actor> onEnded;

    public StatusEffect(string n, string d, Action<Actor, Actor> app, Action<Actor> upd, Action<Actor> end)
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

	void OnKnockbackActivated(Actor self, Actor inflicter)
	{
		self.move.forceDir += (self.transform.position - inflicter.transform.position).normalized * 9;
	}
	void OnKnockbackDebuffUpdated(Actor self)
	{

	}
	void OnKnockbackDebuffEnded(Actor self)
	{
		
	}

	void OnImmuneActivated(Actor self, Actor inflicter)
	{
		self.life.isImmune = true;
	}
	void OnImmuneUpdated(Actor self)
	{
		
	}
	void OnImmuneEnded(Actor self)
	{
		self.life.isImmune = false;
	}

	void OnBlindActivated(Actor self, Actor inflicter)
	{
		self.sight.sightRange *= 0.5f;
	}
	void OnBlindUpdated(Actor self)
	{

	}
	void OnBlindEnded(Actor self)
	{
		self.life.isImmune = false;
	}

	public static void ApplyStat(Actor to, StatEffID id, float dur)
	{
		GameManager.instance.StartCoroutine(DelApplier(to, id, dur));
	}

	static IEnumerator DelApplier(Actor to, StatEffID id, float dur)
	{
		to.life.ApplyStatus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)]);
		yield return new WaitForSeconds(dur);
		to.life.EndStaus((StatusEffect)GameManager.instance.statEff.idStatEffPairs[((int)id)]);
	}

}
