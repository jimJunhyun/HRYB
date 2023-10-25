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
    public Hashtable idStatEffPairs = new Hashtable();
    int id = 0;

	public StatusEffects()
	{
		idStatEffPairs.Add(id++, new StatusEffect("목 과다", "목이 상한치를 넘어섰습니다!", OnWoodDebuffActivated, OnWoodDebuffUpdated, OnWoodDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("화 과다", "화가 상한치를 넘어섰습니다!", OnFireDebuffActivated, OnFireDebuffUpdated, OnFireDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("토 과다", "토가 상한치를 넘어섰습니다!", OnEarthDebuffActivated, OnEarthDebuffUpdated, OnEarthDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("금 과다", "금이 상한치를 넘어섰습니다!", OnMetalDebuffActivated, OnMetalDebuffUpdated, OnMetalDebuffEnded));
		idStatEffPairs.Add(id++, new StatusEffect("수 과다", "수가 상한치를 넘어섰습니다!", OnWaterDebuffActivated, OnWaterDebuffUpdated, OnWaterDebuffEnded));
	}

    void OnWoodDebuffActivated(Actor self, Actor inflicter)
	{
        self.life.regenMod = 0.5f;
	}

    void OnWoodDebuffUpdated(Actor self)
    {

    }

    void OnWoodDebuffEnded(Actor self)
    {
		self.life.regenMod = 1f;
	}

    void OnFireDebuffActivated(Actor self, Actor inflicter)
    {
        self.atk.effSpeedMod = 2f;
		
    }

    void OnFireDebuffUpdated(Actor self)
    {

    }

    void OnFireDebuffEnded(Actor self)
    {
        self.atk.effSpeedMod = 1f;
		
	}

    void OnEarthDebuffActivated(Actor self, Actor inflicter)
    {
        self.atk.prepMod = 0.5f;
		self.cast.castMod = 0.5f;
	}

    void OnEarthDebuffUpdated(Actor self)
    {

    }

    void OnEarthDebuffEnded(Actor self)
    {
		self.atk.prepMod = 1f;
		self.cast.castMod = 1f;
	}

    void OnMetalDebuffActivated(Actor self, Actor inflicter)
    {
        self.sight.sightRange *= 0.5f;
        GameManager.instance.CalcCamVFov(-20);
    }

    void OnMetalDebuffUpdated(Actor self)
    {

    }

    void OnMetalDebuffEnded(Actor self)
    {
        self.sight.sightRange *= 2f;
        GameManager.instance.CalcCamVFov(20);
    }

    void OnWaterDebuffActivated(Actor self, Actor inflicter)
    {
        self.move.speed *= 0.5f;
        self.atk.atkGap *= 2;
    }

    void OnWaterDebuffUpdated(Actor self)
    {

    }

    void OnWaterDebuffEnded(Actor self)
    {
        self.move.speed *= 2f;
        self.atk.atkGap *= 0.5f;
    }
}
