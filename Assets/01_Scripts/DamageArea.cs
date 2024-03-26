using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : DamageObject
{
	public bool isOnce;
    public float calcGap; //n초 간격으로
	public float calcRemainSec; //n초간 판정


	float prevCalcSec = 0;
	bool first = true;
	bool checking;

	float lifetime;

	Collider col;

	float initTime;

	private void Awake()
	{
		col = GetComponent<Collider>();
	}

	public void SetInfo(float time, YinYang dmg, bool isOnce, float checkGap, float checkDur, Actor self, List<StatusEffectApplyData> stat)
	{
		lifetime = time;
		yy = dmg;
		this.isOnce = isOnce;
		calcGap= checkGap;
		calcRemainSec = checkDur;

		first=  true;
		prevCalcSec = 0;
		checking =  false;
		owner = self;
		statData = stat;

		initTime = Time.time;
	}

	private void Update()
	{
		if(!checking && Time.time - prevCalcSec >= calcGap)
		{
			if(first || !isOnce)
			{
				checking = true;
				col.enabled = true;
				prevCalcSec = Time.time;
			}
			if (first)
				first = false;
		}
		if(checking && Time.time - prevCalcSec >= calcRemainSec)
		{
			col.enabled = false;
			checking = false;
			prevCalcSec = Time.time;
		}

		if(Time.time - initTime >= lifetime)
		{
			Returner();
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (checking && other.transform != owner.transform)
		{
			base.OnTriggerEnter(other);
		}
	}

	public override void Damage(LifeModule to)
	{
		to.DamageYY(yy, DamageType.NoEvadeHit);
	}

	public void Returner()
	{
		PoolManager.ReturnObject(gameObject);

	}
}
