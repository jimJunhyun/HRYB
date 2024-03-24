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
	float spawnSec;

	public void SetInfo(float time, YinYang dmg, bool isOnce, float checkGap, float checkDur)
	{
		lifetime = time;
		yy = dmg;
		this.isOnce = isOnce;
		calcGap= checkGap;
		calcRemainSec = checkDur;

		first=  true;
		prevCalcSec = 0;
		checking =  false;

		spawnSec = Time.time;
	}

	private void Update()
	{
		if(!checking && Time.time - prevCalcSec >= calcGap)
		{
			if(first || !isOnce)
			{
				checking = true;
				prevCalcSec = Time.time;
			}
			if (first)
				first = false;
		}
		if(checking && Time.time - prevCalcSec >= calcRemainSec)
		{
			checking = false;
			prevCalcSec = Time.time;
		}
		if(Time.time - spawnSec >= lifetime)
		{
			Returner();
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (checking)
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
