using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		base.Operate(self);
	}

	public override void Disoperate(Actor self)
	{
		base.Disoperate(self);
	}

	
}
