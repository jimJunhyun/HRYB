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

	bool already = false;

	float lifetime;

	public void SetInfo(float time, YinYang dmg, bool isOnce, float checkGap, float checkDur)
	{
		lifetime = time;
		yy = dmg;
		this.isOnce = isOnce;
		calcGap= checkGap;
		calcRemainSec = checkDur;

		already = false;
		first=  true;
		prevCalcSec = 0;
		checking =  false;
	}

	private void Update()
	{
		if(!checking && Time.time - prevCalcSec >= calcGap)
		{
			if(first || !isOnce)
			{
				checking = true;
				already = false;
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
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (checking && !already)
		{
			base.OnTriggerEnter(other);
			already = true;
		}
	}

	public override void Damage(LifeModule to)
	{
		to.DamageYY(yy, DamageType.NoEvadeHit);
	}
}
