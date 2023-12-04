using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ChargeRoot")]
public class ChargeRoot : SkillRoot
{
	public float secPerCharge;

	int curCharge = 0;
	float chargeStartSec;

	bool charging = false;

	Actor owner;

	public override void Disoperate(Actor self)
	{
		if (charging)
		{
			Debug.Log("Charge Ended");
			charging = false;
			base.Disoperate(self);
			owner = null;
		}
		
	}

	public override void Operate(Actor self)
	{
		if (!charging)
		{
			charging = true;
			chargeStartSec = Time.time;
			curCharge = 0;
			Debug.Log($"Charge Started, 1/{childs.Count}");
			childs[curCharge].Operate(owner);
			owner = self;
		}
	}

	public override void UpdateStatus()
	{
		if (charging && Time.time - chargeStartSec >= secPerCharge && curCharge < childs.Count - 1)
		{
			curCharge += 1;
			Debug.Log($"충전 {curCharge + 1}/{childs.Count}");
			childs[curCharge].Operate(owner);
			chargeStartSec = Time.time;
		}
		base.UpdateStatus();
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
