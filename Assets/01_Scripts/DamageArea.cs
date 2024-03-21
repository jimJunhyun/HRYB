using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : DamageObject
{
	public bool isOnce;
    public float calcGap;
	public float calcRemainSec;


	float prevCalcSec = 0;
	bool first = true;
	bool checking;

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
}
