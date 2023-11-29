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
public class SkillRoot : Compose
{
	public Compose act;
	public SkillUseType useType;
	public float castTime;
	public float cooldown;

	WXInfo mySlotInfo;
	public WXInfo MySlotInfo { private get => mySlotInfo; set => mySlotInfo = value;}

	public override void Operate(Actor self)
	{
		act.Operate(self);
	}

	protected override void MyOperation(Actor self)
	{
		//Do Nothing
	}

	public override void Disoperate(Actor self)
	{
		act.Disoperate(self);
	}

	protected override void MyDisoperation(Actor self)
	{
		//Do Nothing
	}
}
