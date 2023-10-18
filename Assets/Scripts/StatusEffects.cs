using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public Hashtable idStatEffPairs;
    int id = 0;

	private StatusEffects()
	{
		idStatEffPairs.Add(id++, new StatusEffect("목 과다", "목이 상한치를 넘어섰습니다!", OnWoodDebuffActivated, OnWoodDebuffUpdated, OnWoodDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("화 과다", "화가 상한치를 넘어섰습니다!", OnFireDebuffActivated, OnFireDebuffUpdated, OnFireDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("토 과다", "토가 상한치를 넘어섰습니다!", OnEarthDebuffActivated, OnEarthDebuffUpdated, OnEarthDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("금 과다", "금이 상한치를 넘어섰습니다!", OnMetalDebuffActivated, OnMetalDebuffUpdated, OnMetalDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("수 과다", "수가 상한치를 넘어섰습니다!", OnWaterDebuffActivated, OnWaterDebuffUpdated, OnWaterDebuffEnded));
	}

    void OnWoodDebuffActivated(Actor self, Actor inflicter)
	{

	}

    void OnWoodDebuffUpdated(Actor self)
    {

    }

    void OnWoodDebuffEnded(Actor self)
    {

    }

    void OnFireDebuffActivated(Actor self, Actor inflicter)
    {
        
    }

    void OnFireDebuffUpdated(Actor self)
    {

    }

    void OnFireDebuffEnded(Actor self)
    {
        
    }

    void OnEarthDebuffActivated(Actor self, Actor inflicter)
    {

    }

    void OnEarthDebuffUpdated(Actor self)
    {

    }

    void OnEarthDebuffEnded(Actor self)
    {

    }

    void OnMetalDebuffActivated(Actor self, Actor inflicter)
    {

    }

    void OnMetalDebuffUpdated(Actor self)
    {

    }

    void OnMetalDebuffEnded(Actor self)
    {

    }

    void OnWaterDebuffActivated(Actor self, Actor inflicter)
    {
        self.move.speed *= 0.5f;
        //self.atk.
    }

    void OnWaterDebuffUpdated(Actor self)
    {

    }

    void OnWaterDebuffEnded(Actor self)
    {

    }
}
