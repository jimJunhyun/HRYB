using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeRoot : SkillRoot
{
	public float secPerCharge;

	int curCharge 
	{
		get => (int)((Time.time - chargeStartSec) / secPerCharge);
	}
	float chargeStartSec;

	bool charging = false;

	public override void Disoperate(Actor self)
	{
		charging = false;
		base.Disoperate(self);
	}

	public override void Operate(Actor self)
	{
		if (Time.time - chargeStartSec >= composeDel && charging)
		{
			Debug.Log($"충전 {curCharge + 1}/{childs.Count}");
			childs[curCharge].Operate(self);
			chargeStartSec = Time.time;
		}
	}

	public override void UpdateStatus()
	{
		
	}

	protected override void MyDisoperation(Actor self)
	{
		//Do nothing
	}

	protected override void MyOperation(Actor self)
	{
		//Do nothing
	}
}
