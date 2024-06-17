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

	[Header("SuperArmor")]
	public bool isSuperArmor;
	public float _superArmorTime = 0;

	[Header("Slot")]
	SkillSlotInfo mySlotInfo;
	[Header("Mana")]
	public float _useMana = 0;

	protected Coroutine _superArmorCorutine = null;

	public SkillSlotInfo MySlotInfo { private get => mySlotInfo; set => mySlotInfo = value;}

	public override void Operate(Actor self)
	{
		if(GameManager.instance.camManager.curCamStat == CamStatus.Freelook)
		{
			Vector3 dir = Camera.main.transform.forward;
			dir.y = 0;
			self.transform.rotation = Quaternion.LookRotation(dir);

		}
		if (isSuperArmor)
		{
			if (_superArmorCorutine != null)
			{
				GameManager.instance.StopCoroutine(_superArmorCorutine);
			}
			_superArmorCorutine = GameManager.instance.StartCoroutine(isSuperArmorCO(self, _superArmorTime));
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
			//Debug.Log("각종강화효과지우기");
			atk.HandleRemoveCall();
		}
	}

	protected IEnumerator isSuperArmorCO(Actor self, float t)
	{
		self.life.superArmor = true;
		//Debug.LogError("슈퍼아머 돌입");
		yield return new WaitForSeconds(t);
		if (isSuperArmor)
		{
			self.life.superArmor = false;
			//Debug.LogError("슈퍼아머 방출");
		}
	}

	
}
