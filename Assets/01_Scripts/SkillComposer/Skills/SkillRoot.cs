using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum SkillUseType
{
	Active,
	ActiveConsumable,
	Passive,
}


[CreateAssetMenu(menuName = "Skills/Root")]
public class SkillRoot : Composite
{
	public SkillUseType useType;
	public float castTime;
	public float cooldown;

	SkillSlotInfo mySlotInfo;
	public SkillSlotInfo MySlotInfo { private get => mySlotInfo; set => mySlotInfo = value;}

	public override void Operate(Actor self)
	{
		Vector3 dir = Camera.main.transform.forward;
		dir.y = 0;
		self.transform.rotation = Quaternion.LookRotation(dir);
		base.Operate(self);
	}

	public override void Disoperate(Actor self)
	{
		base.Disoperate(self);
	}

	
}
