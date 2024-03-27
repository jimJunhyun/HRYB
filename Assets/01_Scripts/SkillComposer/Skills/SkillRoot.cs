using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum SkillUseType
{
	Active,
	ActiveConsumable,
	Passive,
}


[CreateAssetMenu(menuName = "Skills/Root")]
public class SkillRoot : Composite
{
	[FormerlySerializedAs("_skillType")] public WXInfo wx;
	
	public SkillUseType useType;
	public float castTime;
	public float cooldown;
	public Sprite skillIcon;

	public bool isSuperArmor;

	SkillSlotInfo mySlotInfo;
	public SkillSlotInfo MySlotInfo { private get => mySlotInfo; set => mySlotInfo = value;}

	public override void Operate(Actor self)
	{
		Vector3 dir = Camera.main.transform.forward;
		dir.y = 0;
		self.transform.rotation = Quaternion.LookRotation(dir);
		if (isSuperArmor)
		{
			self.life.superArmor = true;
		}
		base.Operate(self);
		//GameManager.instance.StartCoroutine(DelOperate(self));
		base.MyOperation(self);
	}

	public override void Disoperate(Actor self)
	{
		base.Disoperate(self);
	}

	protected override IEnumerator DelOperate(Actor self)
	{
		yield return base.DelOperate(self);
		if (self.atk is PlayerAttack atk)
		{
			Debug.Log("각종강화효과지우기");
			atk.HandleRemoveCall();
		}
		if (isSuperArmor)
		{
			Debug.Log("슈퍼아머끝");
			self.life.superArmor = false;
		}
	}
}
